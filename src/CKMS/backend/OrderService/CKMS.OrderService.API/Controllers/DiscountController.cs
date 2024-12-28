using AutoMapper;
using CKMS.Contracts.DTOs.Order.Request;
using CKMS.Interfaces.Repository;
using CKMS.Interfaces.Storage;
using CKMS.OrderService.Blanket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CKMS.OrderService.API.Controllers
{
    public class DiscountController: ControllerBase
    {
        private readonly DiscountBlanket _discountBlanket;
        public DiscountController(IOrderUnitOfWork orderUnitOfWork, IMapper mapper, IRedis redis) 
        {
            _discountBlanket = new DiscountBlanket(orderUnitOfWork, mapper, redis);
        }

        #region " Business Owner "

        [HttpPost]
        [Authorize]
        [Route("/api/discount/add")]
        public async Task<IActionResult> AddDiscount([FromBody] DiscountPayload payload)
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
                var httpResponse = await _discountBlanket.AddDiscount(payload, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/api/discount/deactivate/{discountId}")]
        public async Task<IActionResult> DeactivateDiscount([FromRoute] String discountId)
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
                var httpResponse = await _discountBlanket.DeactivateDiscount(discountId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion

        #region " Customer "

        [HttpPost]
        [Authorize]
        [Route("/api/discount/apply")]
        public async Task<IActionResult> ApplyDiscount([FromBody] DiscountUsagePayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _discountBlanket.ApplyDiscount(payload, userguid);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/api/discount/cancel")]
        public async Task<IActionResult> CancelDiscount([FromBody] DiscountUsagePayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _discountBlanket.ApplyDiscount(payload, userguid);
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
