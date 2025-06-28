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

namespace Ecommerce_BE_API.WebApi.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "3,4,5,6")]
    public class MstTagOfProductAdminController : BaseApiController
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IMstTagOfProductService _typeOfProductService;

        public MstTagOfProductAdminController(ILoggerService logger, IConfiguration config, IMstTagOfProductService typeOfProductService)
        {
            _logger = logger;
            _config = config;
            _typeOfProductService = typeOfProductService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ResponseResult<string>> create([FromBody] MstTagOfProductReq req)
        {
            try
            {
                var currentId = GetCurrentUserId();

                var res = await _typeOfProductService.AddTypeOfProductAsync(req, currentId);

                if (res == (int)ErrorTypeOfProductCode.TitleEmpty) throw new Exception("Tiêu đề không được để trống!");
                if (res == (int)ErrorTypeOfProductCode.InvalidStatus) throw new Exception("Trạng thái không đúng!");

                return new ResponseResult<string>(RetCodeEnum.Ok, "Thêm mới thành công!", res.ToString());
            }
            catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<ResponseResult<string>> update([FromBody] MstTagOfProduct req)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var res = await _typeOfProductService.UpdateTypeOfProductAsync(req, currentUserId);

                if (res == (int)ErrorTypeOfProductCode.TitleEmpty) throw new Exception("Tiêu đề không được để trống!");
                if (res == (int)ErrorTypeOfProductCode.InvalidStatus) throw new Exception("Trạng thái không đúng!");
                if (res == (int)ErrorTypeOfProductCode.ItemNotFound) throw new Exception("Tag không tồn tại!");

                return new ResponseResult<string>(RetCodeEnum.Ok, "Thêm mới thành công!", res.ToString());
            }
            catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }
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
            catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<ResponseList>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpGet]
        [Route("detail/{id}")]
        public async Task<ResponseResult<MstTagOfProduct>> detail(Guid id)
        {
            try
            {
                var res = await _typeOfProductService.GetDetailTypeOfProductAsync(id);

                return new ResponseResult<MstTagOfProduct>(RetCodeEnum.Ok, "Chi tiết!", res);
            }
            catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstTagOfProduct>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ResponseResult<MstDeletedRes>> delete([FromBody] List<Guid> listDel)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var res = await _typeOfProductService.DeleteTypeOfProductAsync(listDel, currentUserId);

                return new ResponseResult<MstDeletedRes>(RetCodeEnum.Ok, "Xóa thành công!", res);
            }
            catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstDeletedRes>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
