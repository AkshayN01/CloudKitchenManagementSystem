using AutoMapper;
using CKMS.Contracts.DBModels;
using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DBModels.OrderService;
using CKMS.Contracts.DTOs;
using CKMS.Contracts.DTOs.Analytics.Request;
using CKMS.Contracts.DTOs.Order.Request;
using CKMS.Contracts.DTOs.Order.Response;
using CKMS.Interfaces.Repository;
using CKMS.Interfaces.Storage;
using CKMS.Library.Generic;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = CKMS.Contracts.DBModels.OrderService.Order;

namespace CKMS.OrderService.Blanket
{
    public class OrderBlanket
    {
        private readonly Interfaces.Storage.IRedis _Redis;
        private readonly IMapper _Mapper;
        private readonly IOrderUnitOfWork _OrderUnitOfWork;
        public OrderBlanket(IOrderUnitOfWork orderUnitOfWork, IMapper mapper, Interfaces.Storage.IRedis redis)
        {
            _Redis = redis;
            _Mapper = mapper;
            _OrderUnitOfWork = orderUnitOfWork;
        }
        #region " Customer "
        //API to add order to cart
        public async Task<HTTPResponse> AddToCart(OrderPayload payload)
        {

            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                if (payload.Items == null || payload.Items.Count == 0)
                    return APIResponse.ConstructExceptionResponse(retVal, "Empty cart");

                //order is already present in the 
                if (!String.IsNullOrEmpty(payload.OrderId))
                {
                    Guid _OrderId = new Guid(payload.OrderId);
                    Contracts.DBModels.OrderService.Order? _Order = await _OrderUnitOfWork.OrderRepository.GetByGuidAsync(_OrderId);
                    if (_Order == null)
                        return APIResponse.ConstructExceptionResponse(retVal, "Invalid Order Id");

                    if (_Order.CustomerId.ToString() != payload.CustomerId)
                        return APIResponse.ConstructExceptionResponse(retVal, "Customer Id doesn't match");

                    if (_Order.KitchenId.ToString() != payload.KitchenId)
                        return APIResponse.ConstructExceptionResponse(retVal, "Kitchen Id doesn't match");

                    (bool success1, Double TotalAmount1, List<AnalyticsOrderItemPayload>? auditItemPayload1, message) = await AddItemsToOrder(payload, _OrderId, _Order.KitchenId.ToString());
                    if (success1)
                    {
                        _Order.GrossAmount = TotalAmount1;
                        _Order.NetAmount = TotalAmount1;
                        //check if there was any discount applied
                        DiscountUsage? discountUsage = await _OrderUnitOfWork.IDiscountUsageRepository.GetDiscountUsageByOrderIdAsync(_OrderId);
                        if(discountUsage != null)
                        {
                            Discount? discount = await _OrderUnitOfWork.IDicountRepository.GetByGuidAsync(discountUsage.DiscountId);
                            if (discount.DiscountType == (int)DiscountType.Percentage)
                            {
                                _Order.NetAmount = _Order.GrossAmount - ((discount.DiscountValue * _Order.GrossAmount) / 100);
                            }
                            else if (discount.DiscountType == (int)DiscountType.FixedAmount)
                            {
                                _Order.NetAmount = _Order.GrossAmount - discount.DiscountValue;
                            }
                        }

                        _OrderUnitOfWork.OrderRepository.Update(_Order);

                        _Order.DiscountUsage = discountUsage;
                        //await AddDataToAuditTable(_Order, "", "", auditItemPayload1);
                        await _OrderUnitOfWork.CompleteAsync();


                        data = true;
                        retVal = 1;
                        return APIResponse.ConstructHTTPResponse(data, retVal, message);
                    }
                    else
                        return APIResponse.ConstructExceptionResponse(retVal, message);
                }

                Guid userId = new Guid(payload.CustomerId);
                String RedisCustomerKey = $"{_Redis.CustomerKey}:{userId}";
                //check user exists or not
                var userData = await _Redis.HashGetAll(RedisCustomerKey);
                if (userData == null)
                    return APIResponse.ConstructExceptionResponse(retVal, $"Invalid user id found: {userId}");

                HashEntry username = userData.FirstOrDefault(x => x.Name.StartsWith("Name"));

                //check if valid Address
                Guid AddressId = new Guid(payload.Address);
                HashEntry? address = userData.FirstOrDefault(x => x.Name.StartsWith("Address:" + payload.Address));
                if(address == null || !address.HasValue)
                    return APIResponse.ConstructExceptionResponse(retVal, $"Invalid user address id found: {AddressId}");

                //verify kitchen id is valid
                Guid kitchenId = new Guid(payload.KitchenId);
                bool isKitchenIdExists = await _Redis.Has($"{_Redis.KitchenKey}:{payload.KitchenId}");
                if (!isKitchenIdExists)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                if (payload.OrderDate < DateTime.UtcNow) //Order date cannot be less than current date
                    return APIResponse.ConstructExceptionResponse(retVal, "Order date is not valid");

                Guid OrderId = new Guid();

                Contracts.DBModels.OrderService.Order order = new Contracts.DBModels.OrderService.Order()
                {
                    Address = AddressId,
                    CreatedAt = DateTime.UtcNow,
                    CustomerId = userId,
                    KitchenId = kitchenId,
                    OrderDate = payload.OrderDate,
                    OrderId = OrderId,
                    Status = (int)OrderStatus.cart,
                    UpdatedAt = DateTime.UtcNow,
                };

                (bool success, Double TotalAmount, List<AnalyticsOrderItemPayload>? auditItemPayload, message) = await AddItemsToOrder(payload, OrderId, kitchenId.ToString());
                if(!success)
                    return APIResponse.ConstructExceptionResponse(retVal, message);

                order.GrossAmount = TotalAmount;
                order.NetAmount = TotalAmount;
                await _OrderUnitOfWork.OrderRepository.AddAsync(order);

                //audit
                //await AddDataToAuditTable(order, username.Value, address.Value.ToString(), auditItemPayload);

                await _OrderUnitOfWork.CompleteAsync();
                data = true;
                retVal = 1;

            }
            catch(Exception ex)
            {
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(-40, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        private async Task<(bool success, Double TotalAmount, List<AnalyticsOrderItemPayload>? auditItemPayload, String errorMessage)> AddItemsToOrder(OrderPayload payload, Guid OrderId, String kitchenId)
        {
            bool success = false;
            Double TotalAmount = 0;
            String message = String.Empty;
            List<AnalyticsOrderItemPayload> auditItemPayload = new List<AnalyticsOrderItemPayload>();
            try
            {
                //verify menu items are correct
                String RedisKey = $"{_Redis.KitchenKey}:{kitchenId}";
                var menu = await _Redis.HashGetAll($"{_Redis.KitchenKey}:{kitchenId}");
                if (menu == null)
                    return (false, 0, null, "System Error: No menu items found");

                List<OrderItem> orderItems = await _OrderUnitOfWork.OrderItemRepository.GetOrdersByOrderIdAsync(OrderId);
                foreach (OrderItemPayload orderItemPayload in payload.Items)
                {
                    OrderItem? orderItem = default(OrderItem);
                    var menuItem = menu.FirstOrDefault(x => x.Name.StartsWith("menu:" + Convert.ToString(orderItemPayload.MenuItemId)));
                    if (menuItem.Name == RedisValue.Null)
                        throw new Exception("Invalid Menu Item : " + orderItemPayload.MenuItemId);

                    String val = menuItem.Value;
                    String[] values = val.Split(":");
                    TotalAmount += (Convert.ToDouble(values[1]) * orderItemPayload.Quantity);

                    if(orderItems != null && orderItems.Count > 0)
                        orderItem = orderItems.FirstOrDefault(x => x.MenuItemId == orderItemPayload.MenuItemId);
                    
                    if (orderItem != null && (orderItem.Quantity != orderItemPayload.Quantity))
                    {
                        orderItem.Quantity = orderItemPayload.Quantity;
                        _OrderUnitOfWork.OrderItemRepository.Update(orderItem);
                    }
                    else if (orderItem == null)
                    {
                        OrderItem item = new OrderItem()
                        {
                            MenuItemId = orderItemPayload.MenuItemId,
                            OrderId = OrderId,
                            OrderItemId = new Guid(),
                            Quantity = orderItemPayload.Quantity,
                        };
                        await _OrderUnitOfWork.OrderItemRepository.AddAsync(item);
                    }
                    //AnalyticsOrderItemPayload analyticsOrderItemPayload = new AnalyticsOrderItemPayload()
                    //{
                    //    MenuItemId = orderItemPayload.MenuItemId,
                    //    Quantity = orderItemPayload.Quantity,
                    //    MenuItemName = values[0],
                    //    IsDeleted = 0
                    //};
                    //auditItemPayload.Add(analyticsOrderItemPayload);
                }
                //delete items from the db if not present in the cart
                if (orderItems != null && orderItems.Count > 0)
                {
                    List<Int64> MenuItemIds = payload.Items.Select(x => x.MenuItemId).ToList();
                    List<Int64> deletedOrderItems = orderItems.Where(x => !MenuItemIds.Contains(x.MenuItemId)).Select(x => x.MenuItemId).ToList();
                    if (deletedOrderItems != null && deletedOrderItems.Count > 0)
                    {
                        foreach(Int64 itemId in deletedOrderItems)
                        {
                            await _OrderUnitOfWork.OrderItemRepository.DeleteByMenuItemId(itemId);
                            //AnalyticsOrderItemPayload analyticsOrderItemPayload = new AnalyticsOrderItemPayload()
                            //{
                            //    MenuItemId = itemId,
                            //    IsDeleted = 1
                            //};
                            //auditItemPayload.Add(analyticsOrderItemPayload );
                        }
                    }
                }
                success = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                success = false;
            }

            return (success, TotalAmount, auditItemPayload, message);
        }
        //API to confirm an order - to be called only when payment method is Cash on delivery
        public async Task<HTTPResponse> ConfirmOrder(String userId, ConfirmOrderPayload payload)
        {

            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {

                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                Guid OrderId = new Guid(payload.OrderId);
                Contracts.DBModels.OrderService.Order? order = await _OrderUnitOfWork.OrderRepository.GetByGuidAsync(OrderId);

                if (order == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Order Id");

                order.Status = (int)OrderStatus.placed;
                order.UpdatedAt = DateTime.UtcNow;
                _OrderUnitOfWork.OrderRepository.Update(order);

                //apply discount if present
                DiscountUsage? discountUsage = await _OrderUnitOfWork.IDiscountUsageRepository.GetDiscountUsageByOrderIdAsync(OrderId);
                if(discountUsage != null)
                {
                    discountUsage.IsApplied = 1;
                    _OrderUnitOfWork.IDiscountUsageRepository.Update(discountUsage);
                }

                //update Payment details
                Payment payment = new Payment()
                {
                    Amount = order.NetAmount,
                    OrderId = order.OrderId,
                    PaymentId = new Guid(),
                    PaymentMethod = (int)PaymentMethod.CashOnDelivery,
                    PaymentStatus = (int)PaymentStatus.pending,
                };
                await _OrderUnitOfWork.PaymentRepository.AddAsync(payment);

                //order.Payment = payment;
                //order.DiscountUsage = discountUsage;
                //await AddDataToAuditTable(order, "", "", null);

                await _OrderUnitOfWork.CompleteAsync();
                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(-40, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to cancel an order
        public async Task<HTTPResponse> CancelOrder(String orderId)
        {

            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (String.IsNullOrEmpty(orderId))
                    return APIResponse.ConstructExceptionResponse(retVal, "Order Id is empty");

                Guid OrderId = new Guid(orderId);
                Contracts.DBModels.OrderService.Order? Order = await _OrderUnitOfWork.OrderRepository.GetByGuidAsync(OrderId);

                if (Order == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Order Id");

                //to cancel order, it should be placed within 5min
                if (DateTime.UtcNow.Subtract(Order.OrderDate).TotalMinutes > 5)
                    return APIResponse.ConstructExceptionResponse(retVal, "Cannot cancel order: order has been placed over 5min ago");

                Order.Status = (int)OrderStatus.cancelled;
                _OrderUnitOfWork.OrderRepository.Update(Order);

                //update discount usage
                DiscountUsage? discountUsage = await _OrderUnitOfWork.IDiscountUsageRepository.GetDiscountUsageByOrderIdAsync(OrderId);
                if (discountUsage != null)
                {
                    discountUsage.IsApplied = 0; //cancelled
                    _OrderUnitOfWork.IDiscountUsageRepository.Update(discountUsage);
                }

                //Update payment details
                Payment? payment = await _OrderUnitOfWork.PaymentRepository.GetPaymentByOrderIdAsync(OrderId);
                if (payment == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "System Error: No Payment details found");

                payment.PaymentStatus = (int)PaymentStatus.canceled;
                _OrderUnitOfWork.PaymentRepository.Update(payment);


                //Order.Payment = payment;
                //Order.DiscountUsage = discountUsage;
                //await AddDataToAuditTable(Order, "", "", null);

                await _OrderUnitOfWork.CompleteAsync();
                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(-40, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to view an order
        public async Task<HTTPResponse> ViewOrder(String orderId, String kitchenId, String userId)
        {

            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (!String.IsNullOrEmpty(orderId))
                    return APIResponse.ConstructExceptionResponse(retVal, "Order Id is empty");

                Guid OrderId = new Guid(orderId);
                Contracts.DBModels.OrderService.Order? order = await _OrderUnitOfWork.OrderRepository.GetByGuidAsync(OrderId);

                if (order == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Order Id");

                OrderDTO orderDTO = new OrderDTO();
                _Mapper.Map(order, orderDTO);


                //get menu Item name
                var details = await _Redis.HashGetAll($"{_Redis.KitchenKey}:{kitchenId}");
                if (details == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                List<OrderItem> items = await _OrderUnitOfWork.OrderItemRepository.GetOrdersByOrderIdAsync(OrderId);
                orderDTO.Items = new List<OrderItemDTO>();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        foreach (var menuItem in details)
                        {
                            if (menuItem.Name != RedisValue.Null && menuItem.Name.StartsWith("menu:" + Convert.ToString(item.MenuItemId)))
                            {
                                //extract menu id
                                String val = menuItem.Name;
                                String[] values = val.Split(":");
                                OrderItemDTO itemDTO = new OrderItemDTO()
                                {
                                    ItemName = values[2],
                                    MenuItemId = item.MenuItemId,
                                    OrderId = item.OrderId,
                                    Quantity = item.Quantity,
                                };
                                orderDTO.Items.Add(itemDTO);
                            }
                        }
                    }
                }

                orderDTO.Address = await _Redis.HashGet($"{_Redis.CustomerKey}:{userId}", $"address:{order.Address}");

                data = orderDTO;
                retVal = 1;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(-40, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to view all orders
        public async Task<HTTPResponse> ViewAllOrder(String userId, int pageSize, int pageNumber)
        {

            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                OrderList orderList = new OrderList();
                Guid _UserId = new Guid(userId);
                bool isUserExist = await _Redis.Has($"{_Redis.CustomerKey}:{userId}");
                if (!isUserExist)
                    return APIResponse.ConstructExceptionResponse(retVal, $"Invalid user id found: {userId}");

                IQueryable<Contracts.DBModels.OrderService.Order> orderQuery = await _OrderUnitOfWork.OrderRepository.GetOrdersByCustomerIdAsync( _UserId );
                orderList.TotalCount = orderQuery.Count();

                List<Contracts.DBModels.OrderService.Order> Orders = orderQuery.OrderByDescending(x => x.OrderDate)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                if(Orders != null && Orders.Count > 0)
                {
                    foreach(Contracts.DBModels.OrderService.Order order in Orders )
                    {
                        Int64 orderItemCount = await _OrderUnitOfWork.OrderItemRepository.GetOrdersCountByOrderIdAsync( order.OrderId );
                        String kitchenName = await _Redis.HashGet($"{_Redis.KitchenKey}:{order.KitchenId}", "name");
                        OrderListDTO orderListDTO = new OrderListDTO()
                        {
                            ItemCount = orderItemCount,
                            KitchenName = kitchenName,
                            NetAmount = order.NetAmount,
                            OrderDate = order.OrderDate,
                            OrderStatus = Enum.GetName(typeof(OrderStatus), order.Status)
                        };
                        orderList.Orders.Add( orderListDTO );
                    }
                }

                data = orderList;
                retVal = 1;

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(-40, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        #endregion

        #region " Business Owner "
        //API to update an order - Accept/Decline
        public async Task<HTTPResponse> UpdateOrder(String orderId, String status)
        {

            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (String.IsNullOrEmpty(orderId))
                    return APIResponse.ConstructExceptionResponse(retVal, "Order Id is empty");

                status = status.ToLower();
                Guid OrderId = new Guid(orderId);
                Order? order = await _OrderUnitOfWork.OrderRepository.GetByGuidAsync(OrderId);
                if (order == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Order Id");

                if (Enum.TryParse(typeof(OrderStatus), status, out var result) && Enum.IsDefined(typeof(OrderStatus), result))
                {
                    order.Status = (int)(OrderStatus)result;
                    _OrderUnitOfWork.OrderRepository.Update(order);
                    await _OrderUnitOfWork.CompleteAsync();
                    data = true;
                    retVal = 1;
                }
                else
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Status");

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(-40, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to view an order
        public async Task<HTTPResponse> ViewKitchenOrder(String kitchenId, String orderId, String status, int pageSize, int pageNumber)
        {

            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                Guid OrderId = new Guid(orderId);
                Order? order = await _OrderUnitOfWork.OrderRepository.GetByGuidAsync(OrderId);
                if (order == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid order id");

                if (order.KitchenId.ToString() != kitchenId)
                    return APIResponse.ConstructExceptionResponse(retVal, "Not enough permissions to view this order");

                List<OrderItem> orderItems = await _OrderUnitOfWork.OrderItemRepository.GetOrdersByOrderIdAsync(order.OrderId);
                
                DiscountUsage? discountUsage = await _OrderUnitOfWork.IDiscountUsageRepository.GetDiscountUsageByOrderIdAsync(order.OrderId);
                if(discountUsage != null)
                {
                    Discount? discount = await _OrderUnitOfWork.IDicountRepository.GetByGuidAsync(discountUsage.DiscountId);
                    if (discount != null)
                    {

                    }
                }
                OrderList orderList = new OrderList();
                Guid KitchenId = new Guid(kitchenId);
                bool IsKitchenExist = await _Redis.Has($"{_Redis.KitchenKey}:{KitchenId}");
                if (!IsKitchenExist)
                    return APIResponse.ConstructExceptionResponse(retVal, $"Invalid kitchen id found: {kitchenId}");

                

                data = orderList;
                retVal = 1;

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(-40, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to view all orders
        public async Task<HTTPResponse> ViewAllKitchenOrder(String kitchenId, String status, int pageSize, int pageNumber)
        {

            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                OrderList orderList = new OrderList();
                Guid KitchenId = new Guid(kitchenId);
                bool IsKitchenExist = await _Redis.Has($"{_Redis.KitchenKey}:{KitchenId}");
                if (!IsKitchenExist)
                    return APIResponse.ConstructExceptionResponse(retVal, $"Invalid kitchen id found: {kitchenId}");

                IQueryable<Contracts.DBModels.OrderService.Order> orderQuery = await _OrderUnitOfWork.OrderRepository.GetOrdersByKitchenIdAsync(KitchenId);
                orderList.TotalCount = orderQuery.Count();

                //only send those orders that isnt in cart and isnt failed
                if(String.IsNullOrEmpty(status))
                    orderQuery = orderQuery.Where(x => x.Status != (int)OrderStatus.cart || x.Status != (int)OrderStatus.failed)
                        .OrderByDescending(x => x.OrderDate).ThenBy(x => x.Status);
                else
                {
                    if (Enum.TryParse(typeof(OrderStatus), status, out var result) && Enum.IsDefined(typeof(OrderStatus), result))
                    {
                        int orderStatus = (int)(OrderStatus)result;
                        orderQuery = orderQuery.Where(x => x.Status == orderStatus).OrderByDescending(x => x.OrderDate);
                        data = true;
                        retVal = 1;
                    }
                    else
                        return APIResponse.ConstructExceptionResponse(retVal, "Invalid Status");
                }

                List<Contracts.DBModels.OrderService.Order> Orders = orderQuery.Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize).ToList();

                if (Orders != null && Orders.Count > 0)
                {
                    foreach (Contracts.DBModels.OrderService.Order order in Orders)
                    {
                        Int64 orderItemCount = await _OrderUnitOfWork.OrderItemRepository.GetOrdersCountByOrderIdAsync(order.OrderId);
                        String kitchenName = await _Redis.HashGet($"{_Redis.KitchenKey}:{order.KitchenId}", "name");
                        OrderListDTO orderListDTO = new OrderListDTO()
                        {
                            ItemCount = orderItemCount,
                            KitchenName = kitchenName,
                            NetAmount = order.NetAmount,
                            OrderDate = order.OrderDate,
                            OrderStatus = Enum.GetName(typeof(OrderStatus), order.Status)
                        };
                        orderList.Orders.Add(orderListDTO);
                    }
                }

                data = orderList;
                retVal = 1;

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(-40, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        #endregion

        private async Task AddDataToAuditTable(Order order, String customerName, String customerAddress, List<AnalyticsOrderItemPayload>? auditItemPayload)
        {
            AnalyticsOrderPayload payload = new AnalyticsOrderPayload();
            _Mapper.Map(order, payload);

            payload.CustomerName = customerName;
            payload.Address = customerAddress;

            if(order.Payment != null)
            {
                payload.PaymentMethod = order.Payment.PaymentMethod;
                payload.PaymentStatus = order.Payment.PaymentStatus;
            }

            if(auditItemPayload != null && auditItemPayload.Count > 0)
            {
                payload.OrderItems = auditItemPayload;
            }
            else if (order.Items != null)
            {
                payload.OrderItems = new List<AnalyticsOrderItemPayload>();
                foreach (var item in order.Items)
                {
                    AnalyticsOrderItemPayload itemPayload = new AnalyticsOrderItemPayload()
                    {
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity,
                    };
                    payload.OrderItems.Add(itemPayload);
                }
            }

            if(order.DiscountUsage != null)
            {
                payload.DiscountId = order.DiscountUsage.DiscountId;
            }

            String json = await Utility.SerialiseData<AnalyticsOrderPayload>(payload);

            AuditTable orderAuditTable = new AuditTable()
            {
                CreatedAt = DateTime.UtcNow,
                EntityId = order.OrderId.ToString(),
                EntityType = (int)EntityType.Order,
                IsSent = 0,
                Payload = json
            };
            await _OrderUnitOfWork.OrderAuditRepository.AddAsync(orderAuditTable);
        }
    }
}
