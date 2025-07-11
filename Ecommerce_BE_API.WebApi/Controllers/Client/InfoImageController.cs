using DocumentFormat.OpenXml.Math;
using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.Services.Utils.Response;
using Ecommerce_BE_API.WebApi.Controllers.Base;
using Ecommerce_BE_API.WebApi.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_BE_API.WebApi.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InfoImageController : BaseApiController
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IInfoImageService _infoImageService;

        public InfoImageController(ILoggerService logger, IConfiguration config, IInfoImageService infoImageService)
        {
            _logger = logger;
            _config = config;
            _infoImageService = infoImageService;
        }

        [HttpPost]
        [Route("list")]
        [AuthorizeRole(AdminRoleEnum.Admin, AdminRoleEnum.SuperAdmin)]
        public async Task<ResponseResult<ResponseList>> list([FromBody] InfoImageFilter filter, int limit = 25, int page = 1)
        {
            try
            {
                var res = await _infoImageService.GetListAsync(filter, limit, page);
                return new ResponseResult<ResponseList>(RetCodeEnum.Ok, "list", res);
            }catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<ResponseList>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpPost]
        [Route("list-user")]
        public async Task<ResponseResult<ResponseList>> ListUser([FromBody] InfoImageUserFilter filter, int limit = 25, int page = 1)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var res = await _infoImageService.GetListByUserIdAsync(filter, currentUserId, limit, page);

                return new ResponseResult<ResponseList>(RetCodeEnum.Ok, "successfull!", res);
            }
            catch (Exception ex) 
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<ResponseList>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpGet]
        [Route("detail")]
        public async Task<ResponseResult<InfoImage>> detail([FromQuery] Guid id)
        {
            try
            {
                var res = await _infoImageService.GetDetailAsync(id);

                return new ResponseResult<InfoImage>(RetCodeEnum.Ok, "detail", res);
            }catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<InfoImage>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ResponseResult<MstDeletedRes>> delete([FromBody] List<Guid> listDel)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var res = await _infoImageService.DeleteAsync(listDel, currentUserId);

                return new ResponseResult<MstDeletedRes>(RetCodeEnum.Ok, "success!", res);
            }catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstDeletedRes>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
        [HttpDelete]
        [Route("hard-delete")]
        [AuthorizeRole(AdminRoleEnum.SuperAdmin, AdminRoleEnum.Admin)]
        public async Task<ResponseResult<MstDeletedRes>> HardDelete([FromBody] List<Guid> listDel)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var res = await _infoImageService.DeleteAsync(listDel, currentUserId);

                return new ResponseResult<MstDeletedRes>(RetCodeEnum.Ok, "success!", res);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstDeletedRes>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
