using AutoMapper;
using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DTOs.AdminUser.Request;
using CKMS.Contracts.DTOs.AdminUser.Response;
using CKMS.Contracts.DTOs;
using CKMS.Interfaces.Repository;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.AdminUserService.Blanket
{
    public class KitchenBlanket
    {
        private readonly IMapper _mapper;
        private readonly IAdminUserUnitOfWork _AdminUserUnitOfWork;
        public KitchenBlanket(IAdminUserUnitOfWork adminUserUnitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _AdminUserUnitOfWork = adminUserUnitOfWork;
        }

        public async Task<HTTPResponse> AddKitchen(KitchenPayload payload)
        {
            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);

            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(-40, "Payload is missing");

                Kitchen kitchen = new Kitchen()
                {
                    Address = payload.Address,
                    City = payload.City,
                    Country = payload.Country,
                    CreatedAt = DateTime.UtcNow,
                    KitchenId = new Guid(),
                    KitchenName = payload.KitchenName,
                    LastUpdatedAt = DateTime.UtcNow,
                    PostalCode = payload.PostalCode,
                    Region = payload.Region,
                };

                await _AdminUserUnitOfWork.KitchenRepository.AddAsync(kitchen);
                await _AdminUserUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        public async Task<HTTPResponse> GetKitchen(String kitchenId)
        {
            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);

            try
            {
                //check if Kitchen Id is valid or not
                if (String.IsNullOrEmpty(kitchenId))
                    return APIResponse.ConstructExceptionResponse(-40, "KitchenId is missing");

                Guid KitchenId = new Guid(kitchenId);
                Kitchen? kitchen = await _AdminUserUnitOfWork.KitchenRepository.GetByGuidAsync(KitchenId);
                if (kitchen == null)
                    return APIResponse.ConstructExceptionResponse(-40, "Invalid Kitchen Id");

                KitchenDTO kitchenDTO = new KitchenDTO();
                _mapper.Map(kitchen, kitchenDTO);

                data = kitchenDTO;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        public async Task<HTTPResponse> GetAllKitchen(int pageSize, int pageNumber)
        {
            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);

            try
            {
                KitchenListDTO kitchenListDTO = new KitchenListDTO();
                IQueryable<Kitchen> kitchens = _AdminUserUnitOfWork.KitchenRepository.GetAllKitchen();

                kitchenListDTO.TotalCount = kitchens.Count();
                
                List<Kitchen> kitchenList = kitchens.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                _mapper.Map(kitchenList, kitchenListDTO.KitchenList);

                data = kitchenListDTO;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        public async Task<HTTPResponse> UpdateKitchen(KitchenUpdatePayload payload)
        {
            if (payload == null)
                return APIResponse.ConstructExceptionResponse(-40, "Payload is missing");

            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);
            AdminUserDTO userDTO = new AdminUserDTO();

            try
            {
                //check if Kitchen Id is valid or not
                if (String.IsNullOrEmpty(payload.KitchenId))
                    return APIResponse.ConstructExceptionResponse(-40, "KitchenId is missing");

                Guid KitchenId = new Guid(payload.KitchenId);
                Kitchen? kitchen = await _AdminUserUnitOfWork.KitchenRepository.GetByGuidAsync(KitchenId);
                if (kitchen == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                if(!String.IsNullOrEmpty(payload.KitchenName))
                    kitchen.KitchenName = payload.KitchenName;

                if(!String.IsNullOrEmpty(payload.Country))
                    kitchen.Country = payload.Country;

                if(!String.IsNullOrEmpty(payload.Address))
                    kitchen.Address = payload.Address;

                if(!String.IsNullOrEmpty(payload.City))
                    kitchen.City = payload.City;

                if(!String.IsNullOrEmpty(payload.PostalCode))
                    kitchen.PostalCode = payload.PostalCode;

                if(!String.IsNullOrEmpty(payload.Region))
                    kitchen.Region = payload.Region;

                _AdminUserUnitOfWork.KitchenRepository.Update(kitchen);
                await _AdminUserUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        public async Task<HTTPResponse> DeleteKitchen(String kitchenId)
        {
            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);
            AdminUserDTO userDTO = new AdminUserDTO();

            try
            {
                //check if Kitchen Id is valid or not
                if (String.IsNullOrEmpty(kitchenId))
                    return APIResponse.ConstructExceptionResponse(-40, "KitchenId is missing");

                Guid KitchenId = new Guid(kitchenId);
                Kitchen? kitchen = await _AdminUserUnitOfWork.KitchenRepository.GetByGuidAsync(KitchenId);
                if (kitchen == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                _AdminUserUnitOfWork.KitchenRepository.Delete(kitchen);
                await _AdminUserUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
    }
}
