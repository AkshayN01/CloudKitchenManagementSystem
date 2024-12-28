using AutoMapper;
using CKMS.Contracts.Configuration;
using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Contracts.DBModels.NotificationService;
using CKMS.Contracts.DTOs;
using CKMS.Contracts.DTOs.Customer.Request;
using CKMS.Contracts.DTOs.Customer.Response;
using CKMS.Contracts.DTOs.Notification.Request;
using CKMS.Interfaces.HttpClientServices;
using CKMS.Interfaces.Repository;
using CKMS.Interfaces.Storage;
using CKMS.Library.Authentication;
using CKMS.Library.Generic;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.CustomerService.Blanket
{
    public class CustomerBlanket
    {
        private readonly Interfaces.Storage.IRedis _Redis;
        private readonly IMapper _Mapper;
        private readonly Application _appSettings;
        private readonly ICustomerUnitOfWork _CustomerUnitOfWork;
        private readonly INotificationHttpService _notificationHttpService;
        public CustomerBlanket(ICustomerUnitOfWork customerUnitOfWork, IMapper mapper, Interfaces.Storage.IRedis redis, IOptions<Application> appSettings, INotificationHttpService notificationHttpService)
        {
            _Redis = redis;
            _Mapper = mapper;
            _appSettings = appSettings.Value;
            _CustomerUnitOfWork = customerUnitOfWork;
            _notificationHttpService = notificationHttpService;
        }
        //API to Register
        public async Task<HTTPResponse> RegisterUser(RegisterPayload payload)
        {
            int retVal = -40;
            Object? data = default(Object);
            String message = String.Empty;

            try
            {
                //verify payload
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                //create user account
                Customer customer = new Customer()
                {
                    CreatedAt = DateTime.UtcNow,
                    CustomerId = new Guid(),
                    EmailId = payload.EmailId,
                    Name = payload.Name,
                    PhoneNumber = payload.PhoneNumber,
                    UserName = payload.EmailId.ToString(),
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = 1,
                    IsVerified = 0,
                };
                customer.VerificationToken = JWTAuth.
                    GenerateVerificationToken(customer.CustomerId.ToString(), _appSettings.JWTAuthentication.secretKey,
                    _appSettings.JWTAuthentication.issuer, _appSettings.JWTAuthentication.audience, 60);

                customer.PasswordHash = PasswordHasher.HashPassword(payload.Password);

                await _CustomerUnitOfWork.CustomerRepository.AddAsync(customer);
                await _CustomerUnitOfWork.CompleteAsync();

                //add data to redis
                string keyName = $"customer:{customer.CustomerId}";
                HashEntry[] hashEntries = new HashEntry[]
                {
                    new HashEntry("name", customer.Name),
                    new HashEntry("emailId", customer.EmailId),
                    new HashEntry("phoneNumber", customer.PhoneNumber),
                };
                await _Redis.HashSet(keyName, hashEntries);


                //send verification code
                String verificationUrl = _appSettings.VerficationUrl + customer.VerificationToken;
                (String subject, String emailBody) = Utility.GetAccountVerificationEmailContent(verificationUrl, customer.Name);
                //send email for verification
                NotificationPayload notificationPayload = new NotificationPayload()
                {
                    UserId = customer.CustomerId.ToString(),
                    Recipient = customer.EmailId,
                    NotificationType = (int)NotificationType.Email,
                    Scenario = (int)NotificationScenario.AdminVerification,
                    UserType = (int)NotificationUserType.Admin,
                    Message = emailBody,
                    Title = subject,
                };
                List<NotificationPayload> payloads = new List<NotificationPayload>() { notificationPayload };
                data = await _notificationHttpService.SendNotification(payloads);

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = -50;
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(retVal, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to Verify Account
        public async Task<HTTPResponse> VerifyUserAccount(String token)
        {
            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);

            try
            {
                (String userId, String tokenVal) = 
                    JWTAuth.ValidateToken(token, _appSettings.JWTAuthentication.secretKey, _appSettings.JWTAuthentication.issuer, _appSettings.JWTAuthentication.audience);
                //check if User is present or not
                if (String.IsNullOrEmpty(userId))
                    return APIResponse.ConstructExceptionResponse(-40, "UserId is missing");

                Guid UserId = new Guid(userId);
                Customer? customer = await _CustomerUnitOfWork.CustomerRepository.GetByGuidAsync(UserId);

                if (customer == null)
                    return APIResponse.ConstructExceptionResponse(-40, "User not found");

                if (token != customer.VerificationToken)
                    return APIResponse.ConstructExceptionResponse(-40, "Invalid token");

                customer.IsVerified = 1;

                _CustomerUnitOfWork.CustomerRepository.Update(customer);
                await _CustomerUnitOfWork.CompleteAsync();

                //Add user to redis
                string keyName = $"{_Redis.CustomerKey}:{customer.CustomerId}";
                HashEntry[] hashEntries = new HashEntry[]
                {
                    new HashEntry("emailId", customer.EmailId),
                    new HashEntry("phoneNumber", customer.PhoneNumber),
                };
                await _Redis.HashSet(keyName, hashEntries);

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API for login
        public async Task<HTTPResponse> Login(CustomerLoginPayload payload)
        {
            int retVal = -40;
            Object? data = default(Object);
            String message = String.Empty;

            try
            {
                Customer? customer = await _CustomerUnitOfWork.CustomerRepository.GetCustomerByUsername(payload.UserName);
                if (customer == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid username");

                if(!PasswordHasher.VerifyPassword(payload.Password, customer.PasswordHash))
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid password");
                
                CustomerLoginDTO loginDTO = new CustomerLoginDTO() { Name = customer.Name };
                
                String token = JWTAuth.GenerateCustomerJWTToken(customer.CustomerId.ToString(), customer.Name, _appSettings.JWTAuthentication.secretKey);
                loginDTO.Token = token;

                customer.LastLogin = DateTime.UtcNow;
                _CustomerUnitOfWork.CustomerRepository.Update(customer);
                await _CustomerUnitOfWork.CompleteAsync();

                data = loginDTO;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = -50;
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(retVal, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to get details
        public async Task<HTTPResponse> GetCustomerDetails(String customerId)
        {
            int retVal = -40;
            Object? data = default(Object);
            String message = String.Empty;

            try
            {
                if (String.IsNullOrEmpty(customerId))
                    return APIResponse.ConstructExceptionResponse(retVal, "Customer id is empty");

                Guid CustomerId = new Guid(customerId);
                Customer? customer = await _CustomerUnitOfWork.CustomerRepository.GetByGuidAsync(CustomerId);

                if (customer == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid customer id");

                CustomerDTO customerDTO = new CustomerDTO();
                _Mapper.Map(customer, customerDTO);

                data = customerDTO;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = -50;
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(retVal, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to update details
        public async Task<HTTPResponse> UpdateDetails(CustomerUpdatePayload payload, String customerId)
        {
            int retVal = -40;
            Object? data = default(Object);
            String message = String.Empty;

            try
            {
                //verify payload
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                Guid CustomerId = new Guid(customerId);
                Customer? customer = await _CustomerUnitOfWork.CustomerRepository.GetByGuidAsync(CustomerId);

                if (customer == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid customer id");

                if(!String.IsNullOrEmpty(payload.Name))
                    customer.Name = payload.Name;

                if (!String.IsNullOrEmpty(payload.EmailId))
                {
                    customer.EmailId = payload.EmailId;
                    customer.UserName = payload.EmailId.ToString();
                }
                if (!String.IsNullOrEmpty(payload.PhoneNumber))
                    customer.PhoneNumber = payload.PhoneNumber;

                if (!String.IsNullOrEmpty(payload.Password))
                    customer.PasswordHash = PasswordHasher.HashPassword(payload.Password);

                _CustomerUnitOfWork.CustomerRepository.Update(customer);
                await _CustomerUnitOfWork.CompleteAsync();

                //Update user in redis
                string keyName = $"{_Redis.CustomerKey}:{customer.CustomerId}";
                HashEntry[] hashEntries = new HashEntry[]
                {
                    new HashEntry("emailId", customer.EmailId),
                    new HashEntry("phoneNumber", customer.PhoneNumber),
                };
                await _Redis.HashSet(keyName, hashEntries);

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = -50;
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(retVal, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        //API to delete details
        public async Task<HTTPResponse> DeleteDetails(String customerId)
        {
            int retVal = -40;
            Object? data = default(Object);
            String message = String.Empty;

            try
            {
                if (String.IsNullOrEmpty(customerId))
                    return APIResponse.ConstructExceptionResponse(retVal, "Customer id is empty");

                Guid CustomerId = new Guid(customerId);
                Customer? customer = await _CustomerUnitOfWork.CustomerRepository.GetByGuidAsync(CustomerId);

                if (customer == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid customer id");

                _CustomerUnitOfWork.CustomerRepository.Delete(customer);
                await _CustomerUnitOfWork.CompleteAsync();

                //de;ete user in redis
                string keyName = $"{_Redis.CustomerKey}:{customer.CustomerId}";
                await _Redis.DeleteKey(keyName);

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = -50;
                message = ex.Message;
                return APIResponse.ConstructExceptionResponse(retVal, message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
    }
}
