using AutoMapper;
using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Contracts.DTOs;
using CKMS.Contracts.DTOs.Inventory.Request;
using CKMS.Contracts.DTOs.Inventory.Response;
using CKMS.Interfaces.Repository;
using CKMS.Interfaces.Storage;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.InventoryService.Blanket
{
    public class InventoryBlanket
    {
        private readonly IRedis _Redis;
        private readonly IMapper _Mapper;
        private readonly IInventoryUnitOfWork _InventoryUnitOfWork;
        public InventoryBlanket(IInventoryUnitOfWork inventoryUnitOfWork, IMapper mapper, IRedis redis) 
        {
            _Redis = redis;
            _Mapper = mapper;
            _InventoryUnitOfWork = inventoryUnitOfWork;
        }
        #region " INVENTORY "
        #region " ADD "
        public async Task<HTTPResponse> AddInventory(InventoryPayload payload)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                //verify kitchen id is valid
                bool isKitchenIdExists = await _Redis.Has($"{_Redis.KitchenKey}:{payload.KitchenId}");

                if (!isKitchenIdExists)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                Inventory inventory = new Inventory() 
                {
                    CreatedAt = DateTime.UtcNow,
                    InventoryName = payload.InventoryName,
                    KitchenId = payload.KitchenId,
                    LastUpdatedAt = DateTime.UtcNow,
                    MaxStockLevel = payload.MaxStockLevel,
                    RestockThreshold = payload.RestockThreshold,
                    Quantity = payload.Quantity,
                    Unit = payload.Unit
                };

                await _InventoryUnitOfWork.InventoryRepository.AddAsync(inventory);
                await _InventoryUnitOfWork.CompleteAsync();

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

        #region " GET "
        public async Task<HTTPResponse> GetInventoryById(int Id)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            InventoryDTO inventoryDTO = new InventoryDTO();
            try
            {
                Inventory? inventory = await _InventoryUnitOfWork.InventoryRepository.GetByIdAsync(Id);

                if (inventory == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "No INventory found");

                _Mapper.Map(inventory, inventoryDTO);
                data = inventoryDTO;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        public async Task<HTTPResponse> GetAllInventory(String KitchenId, int pageNumber, int pageSize)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (KitchenId == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "KitchenId is null");

                //check if kitchen id is valid or not
                bool isKitchenIdExists = await _Redis.Has($"{_Redis.KitchenKey}:{KitchenId}");

                if (!isKitchenIdExists)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                InventoryListDTO inventoryListDTO = new InventoryListDTO();

                Guid kitchenId = new Guid(KitchenId);
                IEnumerable<Inventory> inventories = await _InventoryUnitOfWork.InventoryRepository.GetAllByKitchenId(kitchenId);
                if(inventories != null)
                {
                    var inventoryList = inventories.ToList();
                    inventoryListDTO.TotalCount = inventoryList.Count;
                    inventoryList = inventoryList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    _Mapper.Map(inventoryList, inventoryListDTO.Inventories);
                    data = inventoryListDTO;
                    retVal = 1;
                }

            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        #endregion

        #region " UPDATE "
        public async Task<HTTPResponse> UpdateInventory(InventoryUpdatePayload payload)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is null");

                //check if inventory id is valid or not
                Inventory? inventory = await _InventoryUnitOfWork.InventoryRepository.GetByIdAsync(payload.InventoryId);

                if (inventory == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid inventory id");

                if(!String.IsNullOrEmpty(payload.InventoryName))
                    inventory.InventoryName = payload.InventoryName;

                inventory.Unit = payload.Unit;
                inventory.Quantity = payload.Quantity;
                inventory.MaxStockLevel = payload.MaxStockLevel;
                inventory.RestockThreshold = payload.RestockThreshold;

                inventory.LastUpdatedAt = DateTime.UtcNow;

                _InventoryUnitOfWork.InventoryRepository.Update(inventory);
                await _InventoryUnitOfWork.CompleteAsync();

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

        #region " DELETE "
        public async Task<HTTPResponse> DeleteInventory(Int64 InventoryId)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                //check if inventory id is valid or not
                Inventory? inventory = await _InventoryUnitOfWork.InventoryRepository.GetByIdAsync(InventoryId);

                if (inventory == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid inventory id");

                _InventoryUnitOfWork.InventoryRepository.Delete(inventory);
                await _InventoryUnitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        #endregion
        #endregion

        #region " INVENTORY MOVEMENT "
        #region " ADD "
        public async Task<HTTPResponse> AddInventoryMovement(InventoryMovementPayload payload)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                //verify kitchen id is valid
                bool isKitchenIdExists = await _Redis.Has($"{_Redis.KitchenKey}:{payload.KitchenId}");

                if (!isKitchenIdExists)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                //check inventory is valid or not
                Inventory? inventory = await _InventoryUnitOfWork.InventoryRepository.GetByIdAsync(payload.InventoryId);
                if (inventory == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid inventory id");

                InventoryMovement inventoryMovement = new InventoryMovement()
                {
                    CreatedAt = DateTime.UtcNow,
                    InventoryId = payload.InventoryId,
                    KitchenId = payload.KitchenId,
                    MovementDate = payload.MovementDate,
                    Quantity = payload.Quantity
                };

                await _InventoryUnitOfWork.InventoryMovementRepository.AddAsync(inventoryMovement);
                await _InventoryUnitOfWork.CompleteAsync();

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

        #region " GET "
        public async Task<HTTPResponse> GetInventoryMovementById(int Id)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            InventoryMovementDTO inventoryDTO = new InventoryMovementDTO();
            try
            {
                InventoryMovement? inventory = await _InventoryUnitOfWork.InventoryMovementRepository.GetByIdAsync(Id);

                if (inventory == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "No Inventory movement found");

                _Mapper.Map(inventory, inventoryDTO);
                data = inventoryDTO;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        #endregion

        #region " UPDATE "
        public async Task<HTTPResponse> UpdateInventoryMovement(InventoryMovementUpdatePayload payload)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is null");

                InventoryMovement? inventoryMovement = await _InventoryUnitOfWork.InventoryMovementRepository.GetByIdAsync(payload.InventoryMovementId);

                if (inventoryMovement == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid inventory movement id");

                inventoryMovement.MovementDate = payload.MovementDate;
                inventoryMovement.Quantity = payload.Quantity;
                inventoryMovement.InventoryId = payload.InventoryId;
                inventoryMovement.UpdatedAt = DateTime.UtcNow;

                _InventoryUnitOfWork.InventoryMovementRepository.Update(inventoryMovement);
                await _InventoryUnitOfWork.CompleteAsync();

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

        #region " DELETE "
        public async Task<HTTPResponse> DeleteInventoryMovement(Int64 id)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                InventoryMovement? inventoryMovement = await _InventoryUnitOfWork.InventoryMovementRepository.GetByIdAsync(id);

                if (inventoryMovement == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid inventory movement id");

                _InventoryUnitOfWork.InventoryMovementRepository.Delete(inventoryMovement);
                await _InventoryUnitOfWork.CompleteAsync();

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
        #endregion
    }
}
