using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Contracts.DTOs.Inventory.Request;
using CKMS.Contracts.DTOs.Inventory.Response;
using CKMS.Contracts.DTOs;
using CKMS.Interfaces.Repository;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CKMS.Interfaces.Storage;
using StackExchange.Redis;

namespace CKMS.InventoryService.Blanket
{
    public class MenuItemBlanket
    {
        private readonly Interfaces.Storage.IRedis _Redis;
        private readonly IMapper _Mapper;
        private readonly IInventoryUnitOfWork _InventoryUnitOfWork;
        public MenuItemBlanket(IInventoryUnitOfWork inventoryUnitOfWork, IMapper mapper, Interfaces.Storage.IRedis redis)
        {
            _Redis = redis;
            _Mapper = mapper;
            _InventoryUnitOfWork = inventoryUnitOfWork;
        }
        #region " ADD "
        public async Task<HTTPResponse> AddMenuItem(MenuItemPayload payload, String kitchenId)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is empty");

                //verify kitchen id is valid
                Guid KitchenId = new Guid(kitchenId);
                bool isKitchenIdExists = await _Redis.Has($"{_Redis.KitchenKey}:{kitchenId}");

                if (!isKitchenIdExists)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Kitchen Id");

                //validate category id
                Category? category = await _InventoryUnitOfWork.CategoryRepository.GetByIdAsync(payload.CategoryId);
                if (category == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid Category Id");

                MenuItem menuItem = new MenuItem()
                {
                    CreatedAt = DateTime.UtcNow,
                    CategoryId = payload.CategoryId,
                    Description = payload.Description,
                    IsAvalilable = payload.IsAvalilable,
                    KitchenId = KitchenId,
                    Name = payload.Name,
                    Price = payload.Price,
                    UpdatedAt = DateTime.UtcNow,
                };

                MenuItem menu = await _InventoryUnitOfWork.MenuItemRepository.AddAndReturnEntity(menuItem);
                await _InventoryUnitOfWork.CompleteAsync();

                //add to redis
                string keyName = $"{_Redis.KitchenKey}:{kitchenId}";
                HashEntry[] hashEntries = new HashEntry[] { new HashEntry($"menu:{menu.MenuItemId}", $"{menu.Name}:{menu.Price}" )};
                await _Redis.HashSet(keyName, hashEntries);

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
        public async Task<HTTPResponse> GetMenuItemById(Int64 Id)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            MenuItemDTO menuItemDTO = new MenuItemDTO();
            try
            {
                MenuItem? menuItem = await _InventoryUnitOfWork.MenuItemRepository.GetByIdAsync(Id);

                if (menuItem == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "No Menu item found");

                Category? category = await _InventoryUnitOfWork.CategoryRepository.GetByIdAsync(menuItem.CategoryId);

                _Mapper.Map(menuItem, menuItemDTO);
                menuItemDTO.CategoryName = category.Name;
                data = menuItemDTO;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        public async Task<HTTPResponse> GetAllMenuItem(String KitchenId, int categoryId, int pageNumber, int pageSize)
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

                MenuItemListDTO menuItemListDTO = new MenuItemListDTO();
                List<MenuItem> items = new List<MenuItem>();

                Guid kitchenId = new Guid(KitchenId);
                IQueryable<MenuItem> menuItems = _InventoryUnitOfWork.MenuItemRepository.GetAllByKitchenId(kitchenId);
                if (menuItems != null)
                {
                    menuItemListDTO.TotalCount = menuItems.Count();
                    if(categoryId != 0)
                        menuItems = menuItems.Where(x => x.CategoryId == categoryId);
                    items = menuItems.Skip((pageNumber - 1) * pageSize).Take(pageSize).OrderBy(x => x.CategoryId).ToList();

                    _Mapper.Map(items, menuItemListDTO.MenuItems);
                    data = menuItemListDTO;
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
        public async Task<HTTPResponse> UpdateMenuItem(MenuItemUpdatePayload payload, String kitchenId)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Payload is null");

                //check if inventory id is valid or not
                MenuItem? menuItem = await _InventoryUnitOfWork.MenuItemRepository.GetByIdAsync(payload.MenuItemId);

                if (menuItem == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid menu item id");

                if (menuItem.KitchenId.ToString() != kitchenId)
                    return APIResponse.ConstructExceptionResponse(retVal, "Not enough permissions");

                if (!String.IsNullOrEmpty(payload.Name))
                    menuItem.Name = payload.Name;
                if (!String.IsNullOrEmpty(payload.Description))
                    menuItem.Description = payload.Description;

                if(payload.CategoryId != 0)
                {
                    //validate category id
                    Category? category = await _InventoryUnitOfWork.CategoryRepository.GetByIdAsync(payload.CategoryId);
                    if (category == null)
                        return APIResponse.ConstructExceptionResponse(retVal, "Invalid Category Id");

                    menuItem.CategoryId = category.CategoryId;
                }

                menuItem.Price = payload.Price;
                menuItem.IsAvalilable = payload.IsAvalilable;

                menuItem.UpdatedAt = DateTime.UtcNow;

                _InventoryUnitOfWork.MenuItemRepository.Update(menuItem);
                await _InventoryUnitOfWork.CompleteAsync();

                //update in redis
                string keyName = $"{_Redis.KitchenKey}:{menuItem.KitchenId}";
                HashEntry[] hashEntries = new HashEntry[] { new HashEntry($"menu:{menuItem.MenuItemId}", $"{menuItem.Name}:{menuItem.Price}") };
                await _Redis.HashSet(keyName, hashEntries);

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
        public async Task<HTTPResponse> DeleteMenuItem(Int64 menuItemId, String kitchenId)
        {
            int retVal = -40;
            Object? data = default(Object?);
            String message = String.Empty;
            try
            {
                //check if inventory id is valid or not
                MenuItem? menuItem = await _InventoryUnitOfWork.MenuItemRepository.GetByIdAsync(menuItemId);

                if (menuItem == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "Invalid menu item id");

                if (menuItem.KitchenId.ToString() != kitchenId)
                    return APIResponse.ConstructExceptionResponse(retVal, "Not enough permissions");

                _InventoryUnitOfWork.MenuItemRepository.Delete(menuItem);
                await _InventoryUnitOfWork.CompleteAsync();

                //delete in redis
                string keyName = $"{_Redis.KitchenKey}:{menuItem.KitchenId}";
                string fieldName = $"menu:{menuItem.MenuItemId}";
                await _Redis.HashDelete(keyName, fieldName);

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
