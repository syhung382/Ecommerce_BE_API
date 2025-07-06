using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.Services.Utils;
using Ecommerce_BE_API.WebApi.Controllers.Base;
using Ecommerce_BE_API.WebApi.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Tls;

namespace Ecommerce_BE_API.WebApi.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IMstUserService _userService;

        public UserController(ILoggerService logger, IConfiguration config, IMstUserService userService)
        {
            _logger = logger;
            _config = config;
            _userService = userService;
        }

        [HttpPost]
        [Route("UserAdd")]
        public async Task<ResponseResult<MstUser>> CreateUserAsync([FromBody] MstUserRegisterReq userReq)
        {
            try
            {
                if (string.IsNullOrEmpty(userReq.UserName)) throw new Exception("UserName cannot be empty!");
                if (string.IsNullOrEmpty(userReq.Password)) throw new Exception("Password cannot be empty!");
                if (string.IsNullOrEmpty(userReq.Email)) throw new Exception("Email cannot be empty!");

                var res = await _userService.AddUserInfoAsync(userReq);

                return new ResponseResult<MstUser>(RetCodeEnum.Ok, "Add user successfully!", res);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstUser>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpPost]
        [Route("UserLoginByUsernamePassword")]
        public async Task<ResponseResult<UserLoginRes>> LoginUserByUsernamePasswordAsync([FromBody] LoginReq req)
        {
            try
            {
                var response = await _userService.SyncUserInfoByUsernamePasswordAsync(req);
                if (response == null) throw new Exception("Username or Password incorrect!");
                if (response.IsBanned == (int)BannedEnum.Yes) throw new Exception("Account has been banned!");
                if (response.DeleteFlag == true) throw new Exception("Account has been deleted!");

                if (string.IsNullOrEmpty(response.CurrentSession))
                {
                    var randomNumberGenerator = new UtilsRandomNumberGenerator();

                    response.CurrentSession = randomNumberGenerator.RandomString(8, false);

                    response = await _userService.UpdateUserInfoAsync(response);
                }
                

                var tokenGenerator = CreateToken(response);

                var result = new UserLoginRes()
                {
                    Id = response.Id,
                    FullName = response.FullName,
                    UserName = response.UserName,
                    Email = response.Email,
                    Avatar = response.Avatar,
                    Gender = response.Gender,
                    Role = response.Role,
                    RoleAdmin = response.RoleAdmin,
                    InviteUserId = response.InviteUserId,
                    InviteUserCount = response.InviteUserCount,
                    CodeInvite = response.CodeInvite,
                    IsFirstLogin = response.IsFirstLogin,
                    LastLoginDate = response.LastLoginDate,
                    CurrentSession = response.CurrentSession,
                    Token = tokenGenerator,
                    CreatedAt = response.CreatedAt,
                    CreatedBy = response.CreatedBy,
                    UpdatedAt = response.UpdatedAt,
                    UpdatedBy = response.UpdatedBy,
                };
                return new ResponseResult<UserLoginRes>(RetCodeEnum.Ok, "Login successfully!", result);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<UserLoginRes>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpGet]
        [Route("get-user")]
        public async Task<ResponseResult<MstUser>> GetUser([FromQuery] int userId)
        {
            try
            {
                var res = await _userService.getUserFromId(userId);

                return new ResponseResult<MstUser>(RetCodeEnum.Ok, "user: " + res.FullName, res);
            }
            catch (Exception ex) {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstUser>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        #region "Private Methods"
        private string CreateToken(MstUser user)
        {
            if (user == null) return string.Empty;

            if(user.Role == (int)UserRoleEnum.Staff && user.RoleAdmin != null)
            {
                var tokenString = CreateTokenString(
                string.IsNullOrEmpty(user.Email) ? user.Email : user.Email,
                user.FullName ?? user.Email,
                user.Id.ToString(),
                user.UserName,
                user.CurrentSession,
                (int)user.RoleAdmin,
                _config["Tokens:Key"],
                _config["Tokens:Issuer"]);
                return tokenString;
            }
            else
            {
                var tokenString = CreateTokenString(
                string.IsNullOrEmpty(user.Email) ? user.Email : user.Email,
                user.FullName ?? user.Email,
                user.Id.ToString(),
                user.UserName,
                user.CurrentSession,
                user.Role,
                _config["Tokens:Key"],
                _config["Tokens:Issuer"]);
                return tokenString;
            }
        }
        #endregion "Private Methods"
    }
}
