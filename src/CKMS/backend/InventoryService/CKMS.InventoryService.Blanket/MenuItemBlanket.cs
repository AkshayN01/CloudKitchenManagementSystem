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

namespace CKMS.InventoryService.Blanket
{
    public class MenuItemBlanket
    {
        private readonly IRedis _Redis;
        private readonly IMapper _Mapper;
        private readonly IInventoryUnitOfWork _InventoryUnitOfWork;
        public MenuItemBlanket(IInventoryUnitOfWork inventoryUnitOfWork, IMapper mapper, IRedis redis)
        {
            _Redis = redis;
            _Mapper = mapper;
            _InventoryUnitOfWork = inventoryUnitOfWork;
        }
        #region " ADD "
        public async Task<HTTPResponse> AddMenuItem(MenuItemPayload payload)
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
                    KitchenId = payload.KitchenId,
                    Name = payload.Name,
                    Price = payload.Price,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _InventoryUnitOfWork.MenuItemRepository.AddAsync(menuItem);
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
        public async Task<HTTPResponse> GetMenuItemById(int Id)
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
                IQueryable<MenuItem> menuItems = await _InventoryUnitOfWork.MenuItemRepository.GetAllByKitchenId(kitchenId);
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
        public async Task<HTTPResponse> UpdateMenuItem(MenuItemUpdatePayload payload)
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
        public async Task<HTTPResponse> DeleteInventory(Int64 menuItemId)
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

                _InventoryUnitOfWork.MenuItemRepository.Delete(menuItem);
                await _InventoryUnitOfWork.CompleteAsync();
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
