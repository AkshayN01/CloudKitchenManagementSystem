using AutoMapper;
using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Contracts.DTOs;
using CKMS.Contracts.DTOs.Customer.Request;
using CKMS.Contracts.DTOs.Customer.Response;
using CKMS.Interfaces.Repository;
using CKMS.Interfaces.Storage;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.CustomerService.Blanket
{
    public class CustomerBlanket
    {
        private readonly IRedis _Redis;
        private readonly IMapper _Mapper;
        private readonly ICustomerUnitOfWork _CustomerUnitOfWork;
        public CustomerBlanket(ICustomerUnitOfWork customerUnitOfWork, IMapper mapper, IRedis redis)
        {
            _Redis = redis;
            _Mapper = mapper;
            _CustomerUnitOfWork = customerUnitOfWork;
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
                };

                customer.PasswordHash = PasswordHasher.HashPassword(payload.Password);

                await _CustomerUnitOfWork.CustomerRepository.AddAsync(customer);
                await _CustomerUnitOfWork.CompleteAsync();


                //send verification code

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

        //API for login
        public async Task<HTTPResponse> Login(String username, string password)
        {
            int retVal = -40;
            Object? data = default(Object);
            String message = String.Empty;

            try
            {
                Customer? customer = await _CustomerUnitOfWork.CustomerRepository.GetCustomerByUsername(username);
                if (customer == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid username");

                if(!PasswordHasher.VerifyPassword(password, customer.PasswordHash))
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid password");

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
