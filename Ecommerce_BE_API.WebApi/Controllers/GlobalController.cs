using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.WebApi.Controllers.Base;
using Ecommerce_BE_API.WebApi.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_BE_API.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlobalController : BaseApiController
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IGlobalService _globalService;

        public GlobalController(ILoggerService logger, IConfiguration config, IMstUserService userService, IGlobalService globalService)
        {
            _logger = logger;
            _config = config;
            _globalService = globalService;
        }

        [HttpGet]
        [Route("ping")]
        public async Task<ResponseResult<GlobalRes>> get()
        {
            try
            {
                var response = new GlobalRes();
                var service = new MstService();

                response.ServerTime = DateTime.UtcNow;
                response.Uptime = Environment.TickCount64 / 1000;

                var res = await _globalService.CheckConnection();

                if (res == true)
                {
                    service.Database = "OK";
                    response.Status = "OK";
                    response.Service = service;
                }
                else
                {
                    service.Database = "Error";
                    response.Status = "Error";
                    response.Service = service;
                }

                return new ResponseResult<GlobalRes> (RetCodeEnum.Ok, "Success", response);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<GlobalRes>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
