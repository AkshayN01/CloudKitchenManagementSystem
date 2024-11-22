using AutoMapper;
using CKMS.Contracts.DBModels.OrderService;
using CKMS.Contracts.DTOs.Order.Response;
using CKMS.Contracts.DTOs;
using CKMS.Interfaces.Repository;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.OrderService.Blanket
{
    public class PaymentBlanket
    {
        private readonly Interfaces.Storage.IRedis _Redis;
        private readonly IMapper _Mapper;
        private readonly IOrderUnitOfWork _OrderUnitOfWork;
        public PaymentBlanket(IOrderUnitOfWork orderUnitOfWork, IMapper mapper, Interfaces.Storage.IRedis redis)
        {
            _Redis = redis;
            _Mapper = mapper;
            _OrderUnitOfWork = orderUnitOfWork;
        }

        public async Task<HTTPResponse> UpdatePayment(String orderId, String paymentStatus)
        {

            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                paymentStatus = paymentStatus.ToLower();
                if (String.IsNullOrEmpty(orderId))
                    return APIResponse.ConstructExceptionResponse(retVal, "Order Id is empty");

                Guid OrderId = new Guid(orderId);
                Payment? payment = await _OrderUnitOfWork.PaymentRepository.GetPaymentByOrderIdAsync(OrderId);

                if (payment == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Payment Id");

                if (Enum.TryParse(typeof(PaymentStatus), paymentStatus, out var result) && Enum.IsDefined(typeof(PaymentStatus), result))
                {
                    payment.PaymentStatus = (int)(PaymentStatus)result;
                    _OrderUnitOfWork.PaymentRepository.Update(payment);
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
    }
}
