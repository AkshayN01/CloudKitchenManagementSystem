using AutoMapper;
using CKMS.Contracts.Configuration;
using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DTOs;
using CKMS.Contracts.DTOs.AdminUser.Request;
using CKMS.Contracts.DTOs.AdminUser.Response;
using CKMS.Interfaces.Repository;
using CKMS.Library.Authentication;
using CKMS.Library.Generic;
using Microsoft.Extensions.Options;

namespace CKMS.AdminUserService.Blanket
{
    public class UserAccounts
    {
        private readonly IMapper _mapper;
        private readonly Application _appSettings;
        private readonly IAdminUserUnitOfWork _AdminUserUnitOfWork;
        public UserAccounts(IAdminUserUnitOfWork adminUserUnitOfWork, IMapper mapper, IOptions<Application> appSettings) 
        {
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _AdminUserUnitOfWork = adminUserUnitOfWork;
        }
        public async Task<HTTPResponse> Login(LoginRequest loginRequest)
        {
            if (loginRequest == null)
                return APIResponse.ConstructExceptionResponse(-40, "Payload is missing");

            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);
            AdminUserLoginDTO userDTO = new AdminUserLoginDTO();

            try
            {
                AdminUser? adminUser = await _AdminUserUnitOfWork.AdminUserRepository.GetUserByUsername(loginRequest.UserName);
                if (adminUser == null)
                    throw new Exception("Invalid user name");

                if (adminUser.IsEmailVerified == 0)
                    throw new Exception("Email Id is not verified");

                //check password
                bool IsValid = PasswordHasher.VerifyPassword(loginRequest.Password, adminUser.PasswordHash);

                //if login successfull update last login time
                if (!IsValid)
                    throw new Exception("Invalid password");

                adminUser.LastLogin = DateTime.UtcNow;
                _AdminUserUnitOfWork.AdminUserRepository.Update(adminUser);
                await _AdminUserUnitOfWork.CompleteAsync();

                userDTO.Name = adminUser.FullName;
                userDTO.token = JWTAuth.
                    GenerateAdminJWTToken(adminUser.UserId.ToString(), adminUser.KitchenId.ToString(), adminUser.RoleId.ToString(), _appSettings.JWTAuthentication.secretKey);

                data = userDTO;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        #region " ADD "
        public async Task<HTTPResponse> AddUser(AdminUserPayload payload, String? KitchenId)
        {
            if (payload == null)
                return APIResponse.ConstructExceptionResponse(-40, "Payload is missing");

            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);
            AdminUserDTO userDTO = new AdminUserDTO();

            try
            {
                //check if Kitchen Id is valid or not
                if (String.IsNullOrEmpty(KitchenId))
                    return APIResponse.ConstructExceptionResponse(-40, "KitchenId is missing");

                Guid kitchenId = new Guid(KitchenId);
                Kitchen? kitchen = await _AdminUserUnitOfWork.KitchenRepository.GetByGuidAsync(kitchenId);
                if (kitchen == null)
                    return APIResponse.ConstructExceptionResponse(-40, "Invalid Kitchen Id");

                String verificationToken = Utility.GenerateVerificationToken();
                AdminUser adminUser = new AdminUser()
                {
                    CreatedAt = DateTime.UtcNow,
                    KitchenId = kitchenId,
                    EmailId = payload.EmailId,
                    FullName = payload.FullName,
                    IsEmailVerified = 0,
                    LastUpdatedAt = DateTime.UtcNow,
                    RoleId = payload.RoleId,
                    UserName = payload.UserName,
                    VerificationToken = verificationToken,
                    UserId = new Guid(),
                };

                await _AdminUserUnitOfWork.AdminUserRepository.AddAsync(adminUser);
                await _AdminUserUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        #endregion

        #region " GET "
        public async Task<HTTPResponse> VerifyUserAccount(String token, String userId)
        {
            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);

            try
            {
                //check if User is present or not
                if (String.IsNullOrEmpty(userId))
                    return APIResponse.ConstructExceptionResponse(-40, "UserId is missing");

                Guid UserId = new Guid(userId);
                AdminUser? adminUser = await _AdminUserUnitOfWork.AdminUserRepository.GetByGuidAsync(UserId);

                if (adminUser == null)
                    return APIResponse.ConstructExceptionResponse(-40, "User not found");

                if (token != adminUser.VerificationToken)
                    return APIResponse.ConstructExceptionResponse(-40, "Invalid token");

                adminUser.IsEmailVerified = 1;
                _AdminUserUnitOfWork.AdminUserRepository.Update(adminUser);

                //Update Kitchen
                Kitchen? kitchen = await _AdminUserUnitOfWork.KitchenRepository.GetByGuidAsync(adminUser.KitchenId);
                kitchen.IsActive = 1;
                _AdminUserUnitOfWork.KitchenRepository.Update(kitchen);

                await _AdminUserUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        public async Task<HTTPResponse> GetUserAccount(string _userId, String? kitchenId)
        {
            if (String.IsNullOrEmpty(_userId))
                return APIResponse.ConstructExceptionResponse(-40, "UserId missing");

            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);
            AdminUserDTO userDTO = new AdminUserDTO();

            try
            {
                Guid userId = new Guid(_userId);
                AdminUser? adminUser = await _AdminUserUnitOfWork.AdminUserRepository.GetByGuidAsync(userId);
                if (adminUser == null)
                    throw new Exception("Invalid user id");

                if (adminUser.KitchenId.ToString() != kitchenId)
                    return APIResponse.ConstructExceptionResponse(401, "Do not have enough permissions");

                _mapper.Map(adminUser, userDTO);

                data = userDTO;
                retVal = 1;
            }
            catch(Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        public async Task<HTTPResponse> GetUserAccountsByRoleId(int roleId, String KitchenId, int pageSize, int pageNumber)
        {
            if (roleId == 0)
                return APIResponse.ConstructExceptionResponse(-40, "RoleId is missing");

            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);
            AdminUserListDTO userListDTO = new AdminUserListDTO();

            try
            {
                Guid kitchenId = new Guid(KitchenId);
                IQueryable<AdminUser> adminUsers = _AdminUserUnitOfWork.AdminUserRepository.GetUsersByRole(roleId, kitchenId);
                if (adminUsers != null)
                {
                    userListDTO.TotalCount = adminUsers.Count();

                    List<AdminUser> adminUsersList = adminUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    _mapper.Map(adminUsersList, userListDTO.Users);

                    data = userListDTO;
                }
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        public async Task<HTTPResponse> GetUserAccountsByKitchenId(string _kitchenId, int pageSize, int pageNumber)
        {
            if (String.IsNullOrEmpty(_kitchenId))
                return APIResponse.ConstructExceptionResponse(-40, "KitchenId is missing");

            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);
            AdminUserListDTO userListDTO = new AdminUserListDTO();

            try
            {
                Guid kitchenId = new Guid(_kitchenId);
                IQueryable<AdminUser> adminUsers = _AdminUserUnitOfWork.AdminUserRepository.GetUsersByKitchen(kitchenId);
                if (adminUsers != null)
                {
                    userListDTO.TotalCount = adminUsers.Count();

                    List<AdminUser> adminUsersList = adminUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    _mapper.Map(adminUsersList, userListDTO.Users);

                    data = userListDTO;
                }
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
        #endregion

        #region " UPDATE "

        public async Task<HTTPResponse> UpdateUser(AdminUserUpdatePayload payload)
        {
            if (payload == null)
                return APIResponse.ConstructExceptionResponse(-40, "Payload is missing");

            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);

            try
            {
                //check if User is present or not
                if (String.IsNullOrEmpty(payload.UserId))
                    return APIResponse.ConstructExceptionResponse(-40, "UserId is missing");

                Guid userId = new Guid(payload.UserId);
                AdminUser? adminUser = await _AdminUserUnitOfWork.AdminUserRepository.GetByGuidAsync(userId);

                if (adminUser == null)
                    return APIResponse.ConstructExceptionResponse(-40, "User not found");

                if (!String.IsNullOrEmpty(payload.EmailId))
                {
                    if (Utility.IsValidEmail(payload.EmailId))
                        adminUser.EmailId = payload.EmailId;
                    else
                        return APIResponse.ConstructExceptionResponse(-40, "Invalid Email Id Format");
                }

                if (!String.IsNullOrEmpty(payload.Password))
                {
                    String PasswordHash = PasswordHasher.HashPassword(payload.Password);
                    adminUser.PasswordHash = PasswordHash;
                }

                if (!String.IsNullOrEmpty(payload.FullName))
                {
                    adminUser.FullName = payload.FullName;
                }

                if (payload.RoleId != adminUser.RoleId)
                {
                    if (Utility.IsValueInEnum<Role>(payload.RoleId))
                        adminUser.RoleId = payload.RoleId;
                    else
                        return APIResponse.ConstructExceptionResponse(-40, "Invalid RoleId");
                }

                _AdminUserUnitOfWork.AdminUserRepository.Update(adminUser);
                await _AdminUserUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        #endregion

        #region " DELETE "
        public async Task<HTTPResponse> DeleteUser(String userId)
        {
            if (String.IsNullOrEmpty(userId))
                return APIResponse.ConstructExceptionResponse(-40, "UserId is missing");

            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);
            AdminUserDTO userDTO = new AdminUserDTO();

            try
            {
                //check if User is present or not
                if (String.IsNullOrEmpty(userId))
                    return APIResponse.ConstructExceptionResponse(-40, "UserId is missing");

                Guid _userId = new Guid(userId);
                AdminUser? adminUser = await _AdminUserUnitOfWork.AdminUserRepository.GetByGuidAsync(_userId);

                if (adminUser == null)
                    return APIResponse.ConstructExceptionResponse(retVal, "No user found");

                _AdminUserUnitOfWork.AdminUserRepository.Delete(adminUser);

                await _AdminUserUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }

        #endregion
    }
}
