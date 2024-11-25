using AutoMapper;
using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Contracts.DTOs.Customer.Request;
using CKMS.Contracts.DTOs.Customer.Response;
using CKMS.Contracts.DTOs;
using CKMS.Interfaces.Repository;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CKMS.Interfaces.Storage;

namespace CKMS.CustomerService.Blanket
{
    public class AddressBlanket
    {
        private readonly IRedis _Redis;
        private readonly IMapper _Mapper;
        private readonly ICustomerUnitOfWork _CustomerUnitOfWork;
        public AddressBlanket(ICustomerUnitOfWork customerUnitOfWork, IMapper mapper, IRedis redis)
        {
            _Redis = redis;
            _Mapper = mapper;
            _CustomerUnitOfWork = customerUnitOfWork;
        }
        //API to Register
        public async Task<HTTPResponse> AddAddress(AddressPayload payload, String customerId)
        {
            int retVal = -40;
            Object? data = default(Object);
            String message = String.Empty;

            try
            {
                //verify payload
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");


                if (String.IsNullOrEmpty(customerId))
                    return APIResponse.ConstructExceptionResponse(retVal, "Customer id is empty");

                Guid CustomerId = new Guid(customerId);
                Customer? customer = await _CustomerUnitOfWork.CustomerRepository.GetByGuidAsync(CustomerId);

                if (customer == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid customer id");

                Address address = new Address()
                {
                    AddressDetail = payload.AddressDetail,
                    AddressId = new Guid(),
                    City = payload.City,
                    Country = payload.Country,
                    CustomerId = CustomerId,
                    PostalCode = payload.PostalCode,
                    Region = payload.Region,
                };

                await _CustomerUnitOfWork.AddressRepository.AddAsync(address);
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

        //API for login
        public async Task<HTTPResponse> GetAddress(String customerId, String addressId)
        {
            int retVal = -40;
            Object? data = default(Object);
            String message = String.Empty;

            try
            {
                if (String.IsNullOrEmpty(addressId))
                    return APIResponse.ConstructExceptionResponse(retVal, "Address id is empty");

                Guid AddressId = new Guid(addressId);
                Address? address = await _CustomerUnitOfWork.AddressRepository.GetByGuidAsync(AddressId);

                if (address == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Address Id");

                AddressDTO addressDTO = new AddressDTO();
                _Mapper.Map(address, addressDTO);

                data = addressDTO;
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
        public async Task<HTTPResponse> GetAllAddress(String customerId)
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

                List<Address> addresses = await _CustomerUnitOfWork.AddressRepository.GetAddressByCustomerId(CustomerId);
                
                List<AddressDTO> addressesDTO = new List<AddressDTO>();
                _Mapper.Map(addresses, addressesDTO);

                data = addressesDTO;
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
        public async Task<HTTPResponse> UpdateAddress(AddressUpdatePayload payload)
        {
            int retVal = -40;
            Object? data = default(Object);
            String message = String.Empty;

            try
            {
                //verify payload
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                Guid AddressId = new Guid(payload.AddressId);
                Address? address = await _CustomerUnitOfWork.AddressRepository.GetByGuidAsync(AddressId);

                if (address == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid address id");

                if (!String.IsNullOrEmpty(payload.AddressDetail))
                    address.AddressDetail = payload.AddressDetail;

                if (!String.IsNullOrEmpty(payload.Country))
                {
                    address.Country = payload.Country;
                }
                if (!String.IsNullOrEmpty(payload.City))
                    address.City = payload.City;

                if (!String.IsNullOrEmpty(payload.PostalCode))
                    address.PostalCode = payload.PostalCode;

                if (!String.IsNullOrEmpty(payload.Region))
                    address.Region = payload.Region;

                _CustomerUnitOfWork.AddressRepository.Update(address);
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
        public async Task<HTTPResponse> DeleteAddress(String addressId)
        {
            int retVal = -40;
            Object? data = default(Object);
            String message = String.Empty;

            try
            {
                if (String.IsNullOrEmpty(addressId))
                    return APIResponse.ConstructExceptionResponse(retVal, "Address id is empty");

                Guid AddressId = new Guid(addressId);
                Address? address = await _CustomerUnitOfWork.AddressRepository.GetByGuidAsync(AddressId);

                if (address == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid address id");

                _CustomerUnitOfWork.AddressRepository.Delete(address);
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
