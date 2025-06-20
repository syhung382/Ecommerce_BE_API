using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.WebApi.Controllers.Base;
using Ecommerce_BE_API.WebApi.Extensions;
using Ecommerce_BE_API.WebApi.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_BE_API.WebApi.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    //[MiddlewareFilter(typeof(LocalizationPipeline))]
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
        [Route("AddUser")]
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

        [HttpGet]
        [Route("Get")]
        public async Task<ResponseResult<string>> get()
        {
            try
            {
                return new ResponseResult<string>(RetCodeEnum.Ok, "ok", "ok");
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
