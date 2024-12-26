using AutoMapper;
using CKMS.Contracts.DTOs.Inventory.Request;
using CKMS.Contracts.DTOs.Order.Request;
using CKMS.Interfaces.Repository;
using CKMS.Interfaces.Storage;
using CKMS.InventoryService.Blanket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CKMS.InventoryService.API.Controllers
{
    public class InventoryController : ControllerBase
    {
        private readonly InventoryBlanket _inventoryBlanket;
        public InventoryController(IInventoryUnitOfWork inventoryUnitOfWork, IMapper mapper, IRedis redis) 
        {
            _inventoryBlanket = new InventoryBlanket(inventoryUnitOfWork, mapper, redis);
        }

        #region " Inventory "

        [HttpPost]
        [Authorize]
        [Route("/api/inventory/add-inventory")]
        public async Task<IActionResult> AddInventory([FromBody] InventoryPayload payload)
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
                var httpResponse = await _inventoryBlanket.AddInventory(payload, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("/api/inventory/update-inventory")]
        public async Task<IActionResult> UpdateInventory([FromBody] InventoryUpdatePayload payload)
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
                var httpResponse = await _inventoryBlanket.UpdateInventory(payload, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/api/inventory/delete-inventory/{inventoryId}")]
        public async Task<IActionResult> DeleteInventory([FromRoute] Int64 inventoryId)
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
                var httpResponse = await _inventoryBlanket.DeleteInventory(inventoryId, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/inventory/get-inventory/{inventoryId}")]
        public async Task<IActionResult> GetInventoryDetails([FromRoute] Int64 inventoryId)
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
                var httpResponse = await _inventoryBlanket.GetInventoryById(inventoryId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/inventory/get-all-inventory")]
        public async Task<IActionResult> GetAllInventory([FromQuery] int pageNumber, [FromQuery] int pageSize)
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
                var httpResponse = await _inventoryBlanket.GetAllInventory(kitchenId, pageNumber, pageSize);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion

        #region " Inventory Movement "

        [HttpPost]
        [Authorize]
        [Route("/api/inventory/add-inventory-movement")]
        public async Task<IActionResult> AddInventoryMovement([FromBody] InventoryMovementPayload payload)
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
                var httpResponse = await _inventoryBlanket.AddInventoryMovement(payload, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("/api/inventory/update-inventory-movement")]
        public async Task<IActionResult> UpdateInventoryMovement([FromBody] InventoryMovementUpdatePayload payload)
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
                var httpResponse = await _inventoryBlanket.UpdateInventoryMovement(payload, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/api/inventory/delete-inventory-movement/{inventoryMovementId}")]
        public async Task<IActionResult> DeleteInventoryMovement([FromRoute] Int64 inventoryMovementId)
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
                var httpResponse = await _inventoryBlanket.DeleteInventoryMovement(inventoryMovementId, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/inventory/get-inventory-movement/{inventoryMovementId}")]
        public async Task<IActionResult> GetInventoryMovementDetails([FromRoute] Int64 inventoryMovementId)
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
                var httpResponse = await _inventoryBlanket.GetInventoryMovementById(inventoryMovementId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/inventory/get-all-inventory-movement/{inventoryId}")]
        public async Task<IActionResult> GetAllInventoryMovements([FromRoute] Int64 inventoryId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
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
                var httpResponse = await _inventoryBlanket.GetInventoryMovements(inventoryId, pageNumber, pageSize);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion
    }
}
