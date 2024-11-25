using AutoMapper;
using CKMS.AdminUserService.Blanket;
using CKMS.Contracts.Configuration;
using CKMS.Contracts.DTOs.AdminUser.Request;
using CKMS.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CKMS.AdminUserService.API.Controllers
{
    public partial class AdminController
    {
        [HttpPost]
        [Route("/api/admin/add-kitchen")]
        public async Task<IActionResult> AddKitchen([FromBody] KitchenPayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };

            try
            {
                var httpResponse = await _kitchenBlanket.AddKitchen(payload);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/admin/get-kitchen/{kitchenId}")]
        public async Task<IActionResult> GetKitchen(string kitchenId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _kitchenBlanket.GetKitchen(kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/admin/get-all-kitchen")]
        public async Task<IActionResult> GetAllKitchen(int pageNumber, int pageSize)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _kitchenBlanket.GetAllKitchen(pageSize, pageNumber);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("/api/admin/update-kitchen")]
        public async Task<IActionResult> UpdateKitchen([FromBody] KitchenUpdatePayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _kitchenBlanket.UpdateKitchen(payload);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("/api/admin/delete-kitchen")]
        public async Task<IActionResult> DeleteKitchen([FromBody] String kitchenId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _kitchenBlanket.DeleteKitchen(kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
