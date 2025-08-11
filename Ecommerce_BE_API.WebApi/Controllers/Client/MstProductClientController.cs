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
    public class MstProductClientController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IMstProductService _productService;

        public MstProductClientController(ILoggerService logger, IConfiguration config, IMstProductService productService)
        {
            _logger = logger;
            _config = config;
            _productService = productService;
        }

        [HttpPost]
        [Route("list")]
        public async Task<ResponseResult<ResponseList>> list([FromBody] MstProductFilter2 filter, int limit = 25, int page = 1)
        {
            try
            {
                var res = await _productService.GetListProductAsync(filter, limit, page);


                return new ResponseResult<ResponseList>(RetCodeEnum.Ok, "Danh sách sản phẩm!", res);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<ResponseList>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpGet]
        [Route("detail/{id}")]
        public async Task<ResponseResult<MstProductRes>> detail(Guid id)
        {
            try
            {
                var res = await _productService.GetDetailProductAsync(id);

                if (res == null) return ResponseError("Sản phẩm không tồn tại!");

                return new ResponseResult<MstProductRes>(RetCodeEnum.Ok, "Chi tiết sản phẩm!", res);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstProductRes>(RetCodeEnum.ApiError, ex.Message, null);
            }
            ResponseResult<MstProductRes> ResponseError(string message)
            {
                return new ResponseResult<MstProductRes>(RetCodeEnum.ResultNotExists, message, null);
            }
        }
    }
}
