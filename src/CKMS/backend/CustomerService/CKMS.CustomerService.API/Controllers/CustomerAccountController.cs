using AutoMapper;
using CKMS.Contracts.Configuration;
using CKMS.Contracts.DTOs.AdminUser.Request;
using CKMS.Contracts.DTOs.Customer.Request;
using CKMS.CustomerService.Blanket;
using CKMS.Interfaces.HttpClientServices;
using CKMS.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CKMS.CustomerService.API.Controllers
{
    public class CustomerAccountController : ControllerBase
    {
        private readonly CustomerBlanket _customerBlanket;
        public CustomerAccountController(ICustomerUnitOfWork customerUnitOfWork, IMapper mapper, Interfaces.Storage.IRedis redis, IOptions<Application> appSettings, INotificationHttpService notificationHttpService) 
        {
            _customerBlanket = new CustomerBlanket(customerUnitOfWork, mapper, redis, appSettings, notificationHttpService);
        }

        [HttpPost]
        [Route("/api/customer/login")]
        public async Task<IActionResult> CustomerLogin([FromBody] CustomerLoginPayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };

            try
            {
                var httpResponse = await _customerBlanket.Login(payload);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/api/customer/register")]
        public async Task<IActionResult> Register([FromBody] RegisterPayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };

            try
            {
                var httpResponse = await _customerBlanket.RegisterUser(payload);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/customer/get-user")]
        public async Task<IActionResult> GetUser()
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _customerBlanket.GetCustomerDetails(userguid);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("/api/customer/update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] CustomerUpdatePayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _customerBlanket.UpdateDetails(payload, userguid);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/api/customer/delete-user")]
        public async Task<IActionResult> DeleteUser()
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _customerBlanket.DeleteDetails(userguid);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        [Route("/api/customer/verify-user")]
        public async Task<IActionResult> VerifyUser([FromQuery] String token)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };

            try
            {
                var httpResponse = await _customerBlanket.VerifyUserAccount(token);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
