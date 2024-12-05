using AutoMapper;
using CKMS.AdminUserService.Blanket;
using CKMS.Contracts.Configuration;
using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DTOs.AdminUser.Request;
using CKMS.Interfaces.HttpClientServices;
using CKMS.Interfaces.Repository;
using CKMS.Interfaces.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CKMS.AdminUserService.API.Controllers
{
    public partial class AdminController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application _appSettings;
        private readonly UserAccounts _userAccounts;
        private readonly KitchenBlanket _kitchenBlanket;
        public AdminController(IAdminUserUnitOfWork unitOfWork, IMapper mapper, IOptions<Application> appSettings, IRedis redis, INotificationHttpService notificationHttpService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _userAccounts = new UserAccounts(unitOfWork, mapper, appSettings);
            _kitchenBlanket = new KitchenBlanket(unitOfWork, mapper, redis, notificationHttpService, appSettings);
        }

        [HttpPost]
        [Route("/api/admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };

            try
            {
                var httpResponse = await _userAccounts.Login(loginRequest);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/api/admin/add-user")]
        public async Task<IActionResult> AddUser([FromBody] AdminUserPayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            var kitchenId = claims.FirstOrDefault(c => c.Type == "kitchenId")?.Value;

            try
            {
                var httpResponse = await _userAccounts.AddUser(payload, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/admin/get-user/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var kitchenId = claims.FirstOrDefault(c => c.Type == "kitchenId")?.Value;
            if (userguid == null || kitchenId == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _userAccounts.GetUserAccount(userId, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/admin/get-users/{roleId}")]
        public async Task<IActionResult> GetUsersByRole(int roleId, int pageNumber, int pageSize)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var kitchenId = claims.FirstOrDefault(c => c.Type == "kitchenId")?.Value;
            if (userguid == null || kitchenId == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _userAccounts.GetUserAccountsByRoleId(roleId, kitchenId, pageSize, pageNumber);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/admin/get-kitchen-users")]
        public async Task<IActionResult> GetUserByKitchen(int pageNumber, int pageSize)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var kitchenId = claims.FirstOrDefault(c => c.Type == "kitchenId")?.Value;
            if (userguid == null || kitchenId == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _userAccounts.GetUserAccountsByKitchenId(kitchenId, pageSize, pageNumber);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("/api/admin/update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] AdminUserUpdatePayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleId = claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
            if (userguid == null || roleId == null) { return Unauthorized(); }

            if(payload.UserId != userguid && Convert.ToInt32(roleId) != (int)Role.SuperAdmin) { return Unauthorized(); }

            try
            {
                var httpResponse = await _userAccounts.UpdateUser(payload);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/api/admin/delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] String userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleId = claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
            if (userguid == null || roleId == null) { return Unauthorized(); }

            if (userId != userguid && Convert.ToInt32(roleId) != (int)Role.SuperAdmin) { return Unauthorized(); }

            try
            {
                var httpResponse = await _userAccounts.DeleteUser(userId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
