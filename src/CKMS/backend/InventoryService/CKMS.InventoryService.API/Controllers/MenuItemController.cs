using AutoMapper;
using CKMS.Contracts.DTOs.Inventory.Request;
using CKMS.Interfaces.Repository;
using CKMS.InventoryService.Blanket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CKMS.InventoryService.API.Controllers
{
    public class MenuItemController : ControllerBase
    {
        private readonly MenuItemBlanket _menuItemBlanket;
        public MenuItemController(IInventoryUnitOfWork inventoryUnitOfWork, IMapper mapper, Interfaces.Storage.IRedis redis) 
        {
            _menuItemBlanket = new MenuItemBlanket(inventoryUnitOfWork, mapper, redis);
        }

        [HttpPost]
        [Authorize]
        [Route("/api/menu/add-menu-item")]
        public async Task<IActionResult> AddMenuItem([FromBody] MenuItemPayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            var kitchenId = claims.FirstOrDefault(c => c.Type == "kitchenId")?.Value;
            if (kitchenId == null)
                return Unauthorized();

            try
            {
                var httpResponse = await _menuItemBlanket.AddMenuItem(payload, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("/api/menu/update-menu-item")]
        public async Task<IActionResult> UpdateMenuItem([FromBody] MenuItemUpdatePayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            var kitchenId = claims.FirstOrDefault(c => c.Type == "kitchenId")?.Value;
            if (kitchenId == null)
                return Unauthorized();

            try
            {
                var httpResponse = await _menuItemBlanket.UpdateMenuItem(payload, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/api/menu/delete-menu-item/{menuItemId}")]
        public async Task<IActionResult> DeleteMenuItem([FromRoute] Int64 menuItemId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            var kitchenId = claims.FirstOrDefault(c => c.Type == "kitchenId")?.Value;
            if (kitchenId == null)
                return Unauthorized();

            try
            {
                var httpResponse = await _menuItemBlanket.DeleteMenuItem(menuItemId, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/menu/get-menu-item/{menuItemId}")]
        public async Task<IActionResult> GetMenuItemDetails([FromRoute] Int64 menuItemId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _menuItemBlanket.GetMenuItemById(menuItemId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/menu/{kitchenId}/get-all-menu")]
        public async Task<IActionResult> GetAllMenuItems([FromRoute] String kitchenId, [FromQuery] int categoryId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _menuItemBlanket.GetAllMenuItem(kitchenId, categoryId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
