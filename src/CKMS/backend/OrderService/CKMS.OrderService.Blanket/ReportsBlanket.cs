using AutoMapper;
using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Contracts.DBModels.OrderService;
using CKMS.Contracts.DTOs;
using CKMS.Contracts.DTOs.Order.Request;
using CKMS.Contracts.DTOs.Order.Response;
using CKMS.Interfaces.Repository;
using CKMS.Library.Generic;
using CKMS.OrderService.DataAccess.Repository;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.OrderService.Blanket
{
    public class ReportsBlanket
    {
        private readonly IOrderUnitOfWork _OrderUnitOfWork;
        private readonly Interfaces.Storage.IRedis _Redis;
        public ReportsBlanket(IOrderUnitOfWork orderUnitOfWork, IMapper mapper, Interfaces.Storage.IRedis redis) 
        {
            _OrderUnitOfWork = orderUnitOfWork;
            _Redis = redis;
        }

        public async Task<HTTPResponse> GetSummary(String _kitchenId, String _startDate, String _endDate)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                Guid kitchenId = new Guid(_kitchenId);

                DateTime startDate = Utility.ConvertDateToString(_startDate);
                DateTime endDate = Utility.ConvertDateToString(_endDate);

                if (startDate > endDate)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Date");

                IQueryable<Contracts.DBModels.OrderService.Order> orderQuery = _OrderUnitOfWork.OrderRepository.GetOrdersByKitchenIdAsync(kitchenId, false, true);
                List<Contracts.DBModels.OrderService.Order> orderList = orderQuery.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate).ToList();

                if(orderList != null && orderList.Count > 0)
                {
                    OrderReportSummary reportSummary = new OrderReportSummary();
                    reportSummary.TotalOrders = orderList.Count;
                    reportSummary.TotalDiscountedOrders = orderList.Count(x => x.DiscountUsage != null);
                    reportSummary.NetRevenue = Math.Round(orderList.Sum(x => x.NetAmount), 2);
                    reportSummary.GrossRevenue = Math.Round(orderList.Sum(x => x.GrossAmount),2);
                    reportSummary.AvgOrderValue = reportSummary.TotalOrders > 0 ? Math.Round(reportSummary.NetRevenue / reportSummary.TotalOrders, 2) : 0;

                    //ordering pattern
                    var orders = orderList.Select(o => new
                    {
                        TimePeriod = GetTimePeriod(o.OrderDate.Hour),
                    }).ToList();

                    // Aggregate the orders by time period
                    reportSummary.OrderingPatterns = orders
                        .GroupBy(o => o.TimePeriod)
                        .Select(g => new CustomerOrderingPattern()
                        {
                            TimePeriod = g.Key,
                            OrdersCount = g.Count()
                        })
                        .OrderBy(tp => tp.TimePeriod) // Optional: Order periods logically (e.g., Morning -> Night)
                        .ToList();

                    data = reportSummary;
                }
                
                retVal = 1;
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        public async Task<HTTPResponse> GetBestSellingDish(String _kitchenId, String _startDate, String _endDate, int top, bool desc)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                Guid kitchenId = new Guid(_kitchenId);

                DateTime startDate = Utility.ConvertDateToString(_startDate);
                DateTime endDate = Utility.ConvertDateToString(_endDate);

                if (startDate > endDate)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Date");

                //get menu Item name
                var details = await _Redis.HashGetAll($"{_Redis.KitchenKey}:{_kitchenId}");
                if (details == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                IQueryable<Contracts.DBModels.OrderService.Order> orderQuery = _OrderUnitOfWork.OrderRepository.GetOrdersByKitchenIdAsync(kitchenId, true);
                orderQuery = orderQuery.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate);

                IQueryable<OrderItem> orderItemList = orderQuery.SelectMany(x => x.Items);

                var bestSellingDishesQuery = orderItemList.GroupBy(x => x.MenuItemId).Select(g => new BestSellingDish()
                {
                    MenuItemId = g.Key,
                    MenuItemName = Utility.GetMenuName(details, g.Key),
                    OrderCount = g.Count(),
                    TotalQuantity = g.Sum(x => x.Quantity)
                });

                if (desc)
                    bestSellingDishesQuery = bestSellingDishesQuery.OrderByDescending(g => g.OrderCount);
                else
                    bestSellingDishesQuery = bestSellingDishesQuery.OrderBy(g => g.OrderCount);
                
                List<BestSellingDish> bestSellingDishes = bestSellingDishesQuery.Take(top).ToList();

                data = bestSellingDishes;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        public async Task<HTTPResponse> GetTopCustomers(String _kitchenId, String? _startDate, String? _endDate, int pageSize, int pageNumber, bool desc = true)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                //if startDate and endDate are null then return all the customers based on pagination
                Guid kitchenId = new Guid(_kitchenId);
                IQueryable<Contracts.DBModels.OrderService.Order> orderQuery = _OrderUnitOfWork.OrderRepository.GetOrdersByKitchenIdAsync(kitchenId, true);

                if (!String.IsNullOrEmpty(_startDate) && !String.IsNullOrEmpty(_endDate))
                {

                    DateTime startDate = Utility.ConvertDateToString(_startDate);
                    DateTime endDate = Utility.ConvertDateToString(_endDate);
                    if (startDate > endDate)
                        return APIResponse.ConstructExceptionResponse(retVal, "Invalid Date");

                    orderQuery = orderQuery.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate);
                }

                IQueryable<TopCustomers> topCustomersQuery = orderQuery.GroupBy(x => x.CustomerId)
                    .Select(g => new TopCustomers()
                    {
                        CustomerId = g.Key.ToString(),
                        TotalOrders = g.Count()
                    });
                if (desc)
                    topCustomersQuery = topCustomersQuery.OrderByDescending(t => t.TotalOrders);
                else
                    topCustomersQuery = topCustomersQuery.OrderBy(t => t.TotalOrders);

                if (pageNumber != 0 && String.IsNullOrEmpty(_startDate) && String.IsNullOrEmpty(_endDate))
                    topCustomersQuery = topCustomersQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                List<TopCustomers> topCustomers = topCustomersQuery.Take(pageSize).ToList();

                if(topCustomers != null && topCustomers.Count > 0)
                {
                    var tasks = topCustomers.Select(async t => t.CustomerName = await GetCustomerName(t.CustomerId));

                    await Task.WhenAll(tasks);
                    data = topCustomers;
                    retVal = 1;
                }

            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        
        public async Task<HTTPResponse> GetCustomerSummary(String _customerId, String kitchenId)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {

                //get menu Item name
                var details = await _Redis.HashGetAll($"{_Redis.KitchenKey}:{kitchenId}");
                if (details == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                CustomerSummary customerSummary = new CustomerSummary();

                Guid KitchenId = new Guid(kitchenId);
                Guid CustomerId = new Guid(_customerId);
                customerSummary.CustomerId = _customerId;
                customerSummary.CustomerName = await GetCustomerName(_customerId);

                IQueryable<Contracts.DBModels.OrderService.Order> ordersQuery = _OrderUnitOfWork.OrderRepository.GetOrdersByCustomerIdAsync(CustomerId, true, true);
                ordersQuery = ordersQuery.Where(x => x.KitchenId == KitchenId);

                customerSummary.TotalOrders = ordersQuery.Count();
                customerSummary.TotalDiscountedOrders = ordersQuery.Count(x => x.DiscountUsage!= null);
                customerSummary.NetRevenue = Math.Round(ordersQuery.Sum(x => x.NetAmount), 2);
                customerSummary.GrossRevenue = Math.Round(ordersQuery.Sum(x => x.GrossAmount), 2);
                customerSummary.AvgOrderValue = customerSummary.TotalOrders > 0 ? Math.Round(customerSummary.NetRevenue / customerSummary.TotalOrders, 2) : 0;

                var latestOrders = ordersQuery.OrderByDescending(x => x.OrderDate).Take(5).ToList();
                if (latestOrders.Any())
                {
                    foreach(var order in latestOrders)
                    {
                        LatestOrder latestOrder = new LatestOrder()
                        {
                            ItemCount = order.Items.Count(),
                            OrderDate = order.OrderDate,
                            NetAmount = order.NetAmount,
                            OrderId = order.OrderId.ToString(),
                            Status = Utility.GetEnumStringValue<OrderStatus>(order.Status),
                        };

                        foreach(var item in order.Items)
                        {
                            OrderItemSummary summary = new OrderItemSummary()
                            {
                                MenuItemName = Utility.GetMenuName(details, item.MenuItemId),
                                TotalQuantity = item.Quantity
                            };
                            latestOrder.Items.Add(summary);
                        }
                        customerSummary.LatestOrders.Add(latestOrder);
                    }
                }

                var customerOrdersQuery = ordersQuery.SelectMany(x => x.Items.Select(i => new
                {
                    i.MenuItemId,
                    i.Quantity,
                    TimePeriod = GetTimePeriod(x.OrderDate.Hour)
                })).ToList();

                // Group and aggregate the data to find the preferred dish and time period
                customerSummary.PreferredDish = customerOrdersQuery
                    .GroupBy(oi => new { oi.MenuItemId, oi.TimePeriod })
                    .Select(g => new PreferredDish()
                    {
                        MenuItemId = g.Key.MenuItemId,
                        MenuItemName = Utility.GetMenuName(details, g.Key.MenuItemId),
                        TimePeriod = g.Key.TimePeriod,
                        TotalQuantity = g.Sum(x => x.Quantity)
                    })
                    .OrderByDescending(x => x.TotalQuantity)
                    .FirstOrDefault();

                //ordering pattern
                var orders = ordersQuery.Select(o => new
                {
                    TimePeriod = GetTimePeriod(o.OrderDate.Hour),
                }).ToList();

                // Aggregate the orders by time period
                customerSummary.OrderingPatterns = orders
                    .GroupBy(o => o.TimePeriod)
                    .Select(g => new CustomerOrderingPattern()
                    {
                        TimePeriod = g.Key,
                        OrdersCount = g.Count()
                    })
                    .OrderBy(tp => tp.TimePeriod) // Optional: Order periods logically (e.g., Morning -> Night)
                    .ToList();

                data = customerSummary;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        private async Task<string> GetCustomerName(String CustomerId)
        {
            string RedisCustomerKey = $"{_Redis.CustomerKey}:{CustomerId}";
            //get user data
            var userData = await _Redis.HashGetAll(RedisCustomerKey);
            if (userData == null)
                throw new Exception("Invalid customer id");
            var val = userData.FirstOrDefault(x => x.Name.StartsWith("name")).ToString();
            var values = val.Split(':');
            return values[1].Trim();
        }

        private static String GetTimePeriod(int hour)
        {
            return hour switch
            {
                >= 5 and < 12 => "Morning",
                >= 12 and < 17 => "Afternoon",
                >= 17 and < 21 => "Evening",
                _ => "Night"
            };
        }
    }
}
