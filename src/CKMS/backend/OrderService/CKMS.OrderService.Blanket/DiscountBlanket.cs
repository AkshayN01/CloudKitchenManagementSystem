using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Contracts.DTOs.Inventory.Request;
using CKMS.Contracts.DTOs;
using CKMS.Interfaces.Repository;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CKMS.Contracts.DTOs.Order.Request;
using CKMS.Contracts.DBModels.OrderService;
using AutoMapper;
using CKMS.Interfaces.Storage;

namespace CKMS.OrderService.Blanket
{
    public class DiscountBlanket
    {
        private readonly IRedis _Redis;
        private readonly IMapper _Mapper;
        private readonly IOrderUnitOfWork _OrderUnitOfWork;
        public DiscountBlanket(IOrderUnitOfWork orderUnitOfWork, IMapper mapper, IRedis redis)
        {
            _Redis = redis;
            _Mapper = mapper;
            _OrderUnitOfWork = orderUnitOfWork;
        }

        #region " Business Owner "

        //API to add discount
        public async Task<HTTPResponse> AddDiscount(DiscountPayload payload)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                //if discount id is already present and its a persnolised discount then it means owner wants to assign
                //users to existing discount
                //check if discount already exisits
                if (!String.IsNullOrEmpty(payload.DiscountId))
                {
                    if (payload.IsPersonalised == 0)
                        return APIResponse.ConstructExceptionResponse(retVal, "Discount already present");

                    Guid DiscountId = new Guid(payload.DiscountId);
                    Discount? discount = await _OrderUnitOfWork.IDicountRepository.GetByGuidAsync(DiscountId);

                    if (discount == null)
                        return APIResponse.ConstructExceptionResponse(retVal, "Invalid Dicount Id");

                    //check if there is data in the PersonalDiscount payload
                    if (payload.PersonalDiscounts == null || payload.PersonalDiscounts.Count == 0)
                        return APIResponse.ConstructExceptionResponse(retVal, "No payload found for Personal Discounts");

                    foreach (var personalDiscount in payload.PersonalDiscounts)
                    {
                        Guid userId = new Guid(personalDiscount.UserId);
                        Guid dicousntId = new Guid(personalDiscount.DiscountId);

                        //check user exists or not
                        bool isUserExist = await _Redis.Has($"{_Redis.CustomerKey}:{userId}");
                        if (!isUserExist)
                            return APIResponse.ConstructExceptionResponse(retVal, $"Invalid user id found: {userId}");

                        //check if the user has been already assigned
                        PersonalDiscounts? personalDiscounts = await _OrderUnitOfWork.PersonalDiscountRespository
                            .GetDiscountByUserIdAndDicountId(userId, dicousntId);

                        if (personalDiscounts == null)
                        {
                            PersonalDiscounts personal = new PersonalDiscounts()
                            {
                                PersonalDiscountId = new Guid(),
                                DiscountId = dicousntId,
                                UserId = userId,
                                UpdatedAt = DateTime.UtcNow,
                            };

                            await _OrderUnitOfWork.PersonalDiscountRespository.AddAsync(personal);
                        }
                    }
                    await _OrderUnitOfWork.CompleteAsync();
                }
                //create new discount
                else
                {
                    //verify kitchen id is valid
                    bool isKitchenIdExists = await _Redis.Has($"{_Redis.KitchenKey}:{payload.KitchenId}");

                    if (!isKitchenIdExists)
                        return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                    Discount discount = new Discount()
                    {
                        DiscountId = new Guid(),
                        DiscountType = payload.DiscountType,
                        DiscountValue = payload.DiscountValue,
                        EndDate = payload.EndDate,
                        IsActive = 1,
                        IsPersonalised = payload.IsPersonalised,
                        KitchenId = new Guid(payload.KitchenId),
                        StartDate = payload.StartDate,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt= DateTime.UtcNow,
                    };
                    await _OrderUnitOfWork.IDicountRepository.AddAsync(discount);

                    if (payload.IsPersonalised == 1 && payload.PersonalDiscounts != null && payload.PersonalDiscounts.Count > 0)
                    {
                        foreach (var personalDiscount in payload.PersonalDiscounts)
                        {
                            Guid userId = new Guid(personalDiscount.UserId);

                            //check user exists or not
                            bool isUserExist = await _Redis.Has($"{_Redis.CustomerKey}:{userId}");
                            if (!isUserExist)
                                return APIResponse.ConstructExceptionResponse(retVal, $"Invalid user id found: {userId}");

                            PersonalDiscounts personal = new PersonalDiscounts()
                            {
                                PersonalDiscountId = new Guid(),
                                DiscountId = discount.DiscountId,
                                UserId = userId,
                                UpdatedAt = DateTime.UtcNow,
                            };

                            await _OrderUnitOfWork.PersonalDiscountRespository.AddAsync(personal);
                        }
                        await _OrderUnitOfWork.CompleteAsync();
                    }
                }

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to deactivate discount
        public async Task<HTTPResponse> DeactivateDiscount(String DiscountId)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (String.IsNullOrEmpty(DiscountId))
                    return APIResponse.ConstructExceptionResponse(retVal, "DiscountId is empty");

                //check discount exists or not
                Guid dicountId = new Guid(DiscountId);
                Discount? discount = await _OrderUnitOfWork.IDicountRepository.GetByGuidAsync(dicountId);

                if (discount == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid discountId");

                discount.IsActive = 0;
                discount.UpdatedAt = DateTime.UtcNow;
                _OrderUnitOfWork.IDicountRepository.Update(discount);
                await _OrderUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        #endregion
        #region " Customer "
        //API to apply discount
        public async Task<HTTPResponse> ApplyDiscount(DiscountUsagePayload payload)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                //check if it's a valid DiscountId
                if (String.IsNullOrEmpty(payload.DiscountId))
                    return APIResponse.ConstructExceptionResponse(retVal, "DiscountId is empty");
                
                Guid DiscountId = new Guid(payload.DiscountId);
                Discount? discount = await _OrderUnitOfWork.IDicountRepository.GetByGuidAsync(DiscountId);

                if (discount == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Dicount Id");

                //check if it's a valid user
                if (String.IsNullOrEmpty(payload.UserId))
                    return APIResponse.ConstructExceptionResponse(retVal, "UserId is empty");

                Guid userId = new Guid(payload.UserId);
                bool isUserExist = await _Redis.Has($"{_Redis.CustomerKey}:{userId}");
                if (!isUserExist)
                    return APIResponse.ConstructExceptionResponse(retVal, $"Invalid user id found: {userId}");

                //check if it's valid order
                if (String.IsNullOrEmpty(payload.OrderId))
                    return APIResponse.ConstructExceptionResponse(retVal, "OrderId is empty");

                Guid OrderId = new Guid(payload.OrderId);
                Order? order = await _OrderUnitOfWork.OrderRepository.GetByGuidAsync(OrderId);

                IEnumerable<DiscountUsage> discounts = await _OrderUnitOfWork.IDiscountUsageRepository
                    .GetUsageByUserIdAndDiscountId(userId, DiscountId);

                bool createNewRecord = true;
                if(discounts != null)
                {
                    List<DiscountUsage> discountUsages = discounts.ToList();
                    if (discountUsages.Count != 0 || discount.UsageCount <= discountUsages.Count)
                        return APIResponse.ConstructExceptionResponse(retVal, "Discount coupon has already been used");

                    DiscountUsage? orderSpecificUsage = discountUsages.FirstOrDefault(x => x.OrderId == OrderId);
                    if (orderSpecificUsage != null && (orderSpecificUsage.IsApplied == 1 || orderSpecificUsage.IsApplied == payload.IsApplied))
                        return APIResponse.ConstructExceptionResponse(retVal, "Discount has already been applied for the order");
                    else if(orderSpecificUsage != null)
                    {
                        orderSpecificUsage.IsApplied = payload.IsApplied;
                        orderSpecificUsage.UpdatedAt = DateTime.UtcNow;
                        _OrderUnitOfWork.IDiscountUsageRepository.Update(orderSpecificUsage);
                        createNewRecord = false;
                    }
                }
                if (createNewRecord)
                {
                    DiscountUsage newUsage = new DiscountUsage()
                    {
                        CreatedAt = DateTime.UtcNow,
                        DiscountId = DiscountId,
                        IsApplied = payload.IsApplied,
                        OrderId = OrderId,
                        UpdatedAt = DateTime.UtcNow,
                        UsageId = new Guid(),
                        UserId = userId,
                    };
                    await _OrderUnitOfWork.IDiscountUsageRepository.AddAsync(newUsage);
                }

                await _OrderUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to cancel discount
        public async Task<HTTPResponse> CancelDiscount(DiscountUsagePayload payload)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                //check if it's a valid DiscountId
                if (String.IsNullOrEmpty(payload.DiscountId))
                    return APIResponse.ConstructExceptionResponse(retVal, "DiscountId is empty");

                Guid DiscountId = new Guid(payload.DiscountId);
                Discount? discount = await _OrderUnitOfWork.IDicountRepository.GetByGuidAsync(DiscountId);

                if (discount == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Dicount Id");

                //check if it's a valid user
                if (String.IsNullOrEmpty(payload.UserId))
                    return APIResponse.ConstructExceptionResponse(retVal, "UserId is empty");

                Guid userId = new Guid(payload.UserId);
                bool isUserExist = await _Redis.Has($"{_Redis.CustomerKey}:{userId}");
                if (!isUserExist)
                    return APIResponse.ConstructExceptionResponse(retVal, $"Invalid user id found: {userId}");

                //check if it's valid order
                if (String.IsNullOrEmpty(payload.OrderId))
                    return APIResponse.ConstructExceptionResponse(retVal, "OrderId is empty");

                Guid OrderId = new Guid(payload.OrderId);
                Order? order = await _OrderUnitOfWork.OrderRepository.GetByGuidAsync(OrderId);

                IEnumerable<DiscountUsage> discounts = await _OrderUnitOfWork.IDiscountUsageRepository
                    .GetUsageByUserIdAndDiscountId(userId, DiscountId);

                if (discounts == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Discount has not been applied yet");

                DiscountUsage? orderSpecificUsage = discounts.FirstOrDefault(x => x.OrderId == OrderId);

                if (orderSpecificUsage == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Discount has not been applied yet for this order");

                _OrderUnitOfWork.IDiscountUsageRepository.Delete(orderSpecificUsage);
                await _OrderUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        #endregion
    }
}
