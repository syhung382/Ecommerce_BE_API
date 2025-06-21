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
                int currentUserId = GetCurrentUserId();

                if (currentUserId == 0) throw new Exception("Token is broken!");

                return new ResponseResult<string>(RetCodeEnum.Ok, "UserId", currentUserId.ToString());
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
