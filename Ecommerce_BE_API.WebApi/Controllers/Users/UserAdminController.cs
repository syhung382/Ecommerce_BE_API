using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.WebApi.Controllers.Base;
using Ecommerce_BE_API.WebApi.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_BE_API.WebApi.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAdminController : BaseApiController
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IMstUserService _userService;

        public UserAdminController(ILoggerService logger, IConfiguration config, IMstUserService userService)
        {
            _logger = logger;
            _config = config;
            _userService = userService;
        }

        [HttpGet]
        [Route("TestToken")]
        public async Task<ResponseResult<string>> testToken()
        {
            try
            {
                var currentSession = GetCurrentUserSession();

                if (string.IsNullOrEmpty(currentSession)) throw new Exception("Token is broken!");

                return new ResponseResult<string>(RetCodeEnum.Ok, "userSession", currentSession);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpGet]
        [Route("check_user_by_token")]
        public async Task<ResponseResult<UserLoginRes>> checkUserByToken()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserSession = GetCurrentUserSession();

                var res = await _userService.getUserFromId(currentUserId);
                if (res == null) throw new Exception("Phiên đăng nhập hết hạn, vui lòng đăng nhập lại!");
                if(res.CurrentSession != currentUserSession) throw new Exception("Phiên đăng nhập hết hạn, vui lòng đăng nhập lại!");
                if (res.IsBanned == (int)BannedEnum.Yes) throw new Exception("Tài khoản bị khóa!");
                if (res.Status == (int)UserStatusEnum.TemporarilyDeleted) throw new Exception("Tài khoản đang tạm xóa!");

                var result = new UserLoginRes()
                {
                    Id = res.Id,
                    Avatar = res.Avatar,
                    CodeInvite = res.CodeInvite,
                    CreatedAt = res.CreatedAt,
                    CreatedBy = res.CreatedBy,
                    CurrentSession = currentUserSession,
                    Email = res.Email,
                    FullName = res.FullName,
                    Gender = res.Gender,
                    InviteUserCount = res.InviteUserCount,
                    InviteUserId = res.InviteUserId,
                    IsFirstLogin = res.IsFirstLogin,
                    LastLoginDate = res.LastLoginDate,
                    Role = res.Role,
                    RoleAdmin = res.RoleAdmin,
                    Token = "",
                    UpdatedAt = res.UpdatedAt,
                    UpdatedBy = res.UpdatedBy,
                    UserName = res.UserName,
                };

                return new ResponseResult<UserLoginRes>(RetCodeEnum.Ok, "Đăng nhập thành công", result);

            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<UserLoginRes>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
