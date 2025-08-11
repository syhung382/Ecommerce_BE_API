using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.WebApi.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_BE_API.WebApi.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class MstCategoryClientController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IMstCategoryService _categoryService;

        public MstCategoryClientController(ILoggerService logger, IConfiguration config, IMstCategoryService categoryService)
        {
            _logger = logger;
            _config = config;
            _categoryService = categoryService;
        }

        [HttpPost]
        [Route("list")]
        public async Task<ResponseResult<ResponseList>> List([FromBody] MstCategoryFilter filter, int limit = 25, int page = 1)
        {
            try
            {
                var res = await _categoryService.GetListCategoryAsync(filter, limit, page);
                return new ResponseResult<ResponseList>(RetCodeEnum.Ok, RetCodeEnum.Ok.ToString(), res);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<ResponseList>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpGet]
        [Route("detail/{id}")]
        public async Task<ResponseResult<MstCategory>> Get(Guid id)
        {
            try
            {
                var res = await _categoryService.GetDetailCategoryAsync(id);

                if (res == null) return ResponseError("Danh mục không tồn tại!");

                return new ResponseResult<MstCategory>(RetCodeEnum.Ok, RetCodeEnum.Ok.ToString(), res);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstCategory>(RetCodeEnum.ApiError, ex.Message, null);
            }
            ResponseResult<MstCategory> ResponseError(string message)
            {
                return new ResponseResult<MstCategory>(RetCodeEnum.ResultNotExists, message, null);
            }
        }

        [HttpGet]
        [Route("list-have-product")]
        public async Task<ResponseResult<List<MstCategory>>> getListHaveProduct(int limit)
        {
            try
            {
                var res = await _categoryService.GetListCategoryHasProductAsync(limit);

                return new ResponseResult<List<MstCategory>>(RetCodeEnum.Ok, "ok", res);
            }catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<List<MstCategory>>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
