using AutoMapper;
using CKMS.AdminUserService.Blanket;
using CKMS.Contracts.Configuration;
using CKMS.Contracts.DTOs.AdminUser.Request;
using CKMS.Interfaces.Repository;
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
        public AdminController(IAdminUserUnitOfWork unitOfWork, IMapper mapper, IOptions<Application> appSettings)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _userAccounts = new UserAccounts(unitOfWork, mapper);
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
        [Route("/api/admin/add-user")]
        public async Task<IActionResult> AddUser([FromBody] AdminUserPayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _userAccounts.AddUser(payload);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/admin/get-user/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _userAccounts.GetUserAccount(userId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/admin/get-users/{roleId}")]
        public async Task<IActionResult> GetUsersByRole(int roleId, int pageNumber, int pageSize)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _userAccounts.GetUserAccountsByRoleId(roleId, pageSize, pageNumber);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/admin/get-kitchen-users/{kitchenId}")]
        public async Task<IActionResult> GetUserByKitchen(String kitchenId, int pageNumber, int pageSize)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

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
        [Route("/api/admin/update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] AdminUserUpdatePayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

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
        [Route("/api/admin/delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] String userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

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
