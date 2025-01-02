using AutoMapper;
using CKMS.Contracts.DTOs.Order.Request;
using CKMS.Interfaces.Repository;
using CKMS.OrderService.Blanket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CKMS.OrderService.API.Controllers
{
    public class ReportsController : ControllerBase
    {
        private readonly ReportsBlanket _reportBlanket;
        public ReportsController(IOrderUnitOfWork orderUnitOfWork, IMapper mapper, Interfaces.Storage.IRedis redis) 
        {
            _reportBlanket = new ReportsBlanket(orderUnitOfWork, mapper, redis);
        }

        [HttpGet]
        [Route("/api/report/get-summary")]
        public async Task<IActionResult> GetSummary([FromQuery] String startDate, [FromQuery] String endDate)
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
                var httpResponse = await _reportBlanket.GetSummary(kitchenId, startDate, endDate);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/report/best-selling-dish")]
        public async Task<IActionResult> GetBestSellingDish([FromQuery] String startDate, [FromQuery] String endDate, [FromQuery] int top, [FromQuery] bool desc)
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
                var httpResponse = await _reportBlanket.GetBestSellingDish(kitchenId, startDate, endDate, top, desc);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/report/top-customers")]
        public async Task<IActionResult> GetTopCustomers([FromQuery] String? startDate, [FromQuery] String? endDate, [FromQuery] int pageSize, [FromQuery] int pageNumber, [FromQuery] bool dec)
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
                var httpResponse = await _reportBlanket.GetTopCustomers(kitchenId, startDate, endDate, pageSize, pageNumber, dec);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/report/customer-summary")]
        public async Task<IActionResult> GetCustomerSummary([FromQuery] String customerId)
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
                var httpResponse = await _reportBlanket.GetCustomerSummary(customerId, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
