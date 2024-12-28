using AutoMapper;
using CKMS.Contracts.DTOs.Customer.Request;
using CKMS.CustomerService.Blanket;
using CKMS.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CKMS.CustomerService.API.Controllers
{
    public class AddressController : ControllerBase
    {
        private readonly AddressBlanket _addressBlanket;
        public AddressController(ICustomerUnitOfWork customerUnitOfWork, IMapper mapper, Interfaces.Storage.IRedis redis) 
        {
            _addressBlanket = new AddressBlanket(customerUnitOfWork, mapper, redis);
        }

        [HttpPost]
        [Authorize]
        [Route("/api/customer/add-address")]
        public async Task<IActionResult> AddAddress([FromBody] AddressPayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _addressBlanket.AddAddress(payload, userguid);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("/api/customer/update-address")]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressUpdatePayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _addressBlanket.UpdateAddress(payload);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/api/customer/delete-address/{addressId}")]
        public async Task<IActionResult> DeleteAddress([FromRoute] String addressId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _addressBlanket.DeleteAddress(addressId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/customer/get-address/{addressId}")]
        public async Task<IActionResult> GetAddress([FromRoute] String addressId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _addressBlanket.GetAddress(userguid, addressId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/customer/get-all-address")]
        public async Task<IActionResult> GetAllAddress()
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _addressBlanket.GetAllAddress(userguid);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
