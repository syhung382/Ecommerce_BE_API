using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.WebApi.Controllers.Base;
using Ecommerce_BE_API.WebApi.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_BE_API.WebApi.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class MstTagOfProductClientController : BaseApiController
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IMstTagOfProductService _typeOfProductService;

        public MstTagOfProductClientController(ILoggerService logger, IConfiguration config, IMstTagOfProductService typeOfProductService)
        {
            _logger = logger;
            _config = config;
            _typeOfProductService = typeOfProductService;
        }

        [HttpPost]
        [Route("list")]
        public async Task<ResponseResult<ResponseList>> list([FromBody] MstTagOfProductFilter filter, int limit = 25, int page = 1)
        {
            try
            {
                var res = await _typeOfProductService.GetListTypeOfProductAsync(filter, limit, page);

                return new ResponseResult<ResponseList>(RetCodeEnum.Ok, "Danh sách!", res);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<ResponseList>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpGet]
        [Route("list-has-product")]
        public async Task<ResponseResult<List<MstTagOfProduct>>> listHasProduct(int limit = 10)
        {
            try
            {
                var res = await _typeOfProductService.getListHasProductAsync(limit);

                return new ResponseResult<List<MstTagOfProduct>>(RetCodeEnum.Ok, RetCodeEnum.Ok.ToString(), res);
            }catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<List<MstTagOfProduct>>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
