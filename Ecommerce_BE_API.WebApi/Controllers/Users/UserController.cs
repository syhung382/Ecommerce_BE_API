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
        public async Task<ResponseResult<string>> CreateUserAsync([FromBody] MstUserRegisterReq userReq)
        {
            try
            {
                var res = await _userService.AddUserByRegisterAsync(userReq);

                if (res == (int)ErrorUserCode.InvalidEmail) return ResError("Email không hợp lệ!");
                if (res == (int)ErrorUserCode.InvalidPassword) return ResError("Mật khẩu phải ít nhất 6 ký tự!");
                if (res == (int)ErrorUserCode.InvalidUsername) return ResError("Tên đăng nhập không hợp lệ!");
                if (res == (int)ErrorUserCode.InExistUsername) return ResError("Tên đăng nhập đã tồn tại!");
                if (res == (int)ErrorUserCode.InExistEmail) return ResError("Email đã tồn tại!");
                if (res == (int)ErrorUserCode.InvalidGender) return ResError("Giới tính không đúng!");

                return new ResponseResult<string>(RetCodeEnum.Ok, "Add user successfully!", null);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }

            ResponseResult<string> ResError(string message)
            {
                return new ResponseResult<string>(RetCodeEnum.ApiError, message, null);
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

                    await _userService.UpdateUserInfoAsync(response, (int)UserRoleEnum.User, response.Id);
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
        public async Task<ResponseResult<MstUserRes>> GetUser([FromQuery] int userId)
        {
            try
            {
                var res = await _userService.getUserFromId(userId);

                if (res == null) throw new Exception("Tài khoản không tồn tại!");

                var response = new MstUserRes()
                {
                    Id = res.Id,
                    Avatar = res.Avatar,
                    Email = res.Email,
                    FullName = res.FullName,
                    Gender = res.Gender,
                    UserName = res.UserName,
                };

                return new ResponseResult<MstUserRes>(RetCodeEnum.Ok, "user: " + res.FullName, response);
            }
            catch (Exception ex) {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstUserRes>(RetCodeEnum.ApiError, ex.Message, null);
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
