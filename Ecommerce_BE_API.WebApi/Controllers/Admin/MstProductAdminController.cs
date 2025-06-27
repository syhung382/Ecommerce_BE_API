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
    public class MstProductAdminController : BaseApiController
    {

        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IMstProductService _productService;

        public MstProductAdminController(ILoggerService logger, IConfiguration config, IMstProductService productService)
        {
            _logger = logger;
            _config = config;
            _productService = productService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ResponseResult<string>> create([FromBody] MstProductReq req)
        {
            try
            {
                var currentId = GetCurrentUserId();

                if (req.PriceSale != null && req.PriceSale > req.Price) throw new Exception("Giá tiền khuyễn mãi phải lớn hơn giá tiền gốc!");

                var res = await _productService.AddProductAsync(req, currentId);

                if (res == (int)ErrorProductCode.TitleEmpty) throw new Exception("Tiêu đề không được để trống!");
                if (res == (int)ErrorProductCode.CategoryNotFound) throw new Exception("Danh mục không tồn tại!");
                if (res == (int)ErrorProductCode.InvalidStatus) throw new Exception("Trạng thái không đúng!");
                if (res == (int)ErrorProductCode.PriceInvalid) throw new Exception("Giá tiền không hợp lệ!");
                if (res == (int)ErrorProductCode.PriceSaleInvalid) throw new Exception("Giá tiền khuyễn mãi không hợp lệ!");

                return new ResponseResult<string>(RetCodeEnum.Ok, "Thêm sản phẩm thành công!", res.ToString());
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<ResponseResult<string>> update([FromBody] MstProduct req)
        {
            try
            {
                var curentId = GetCurrentUserId();

                if (req.PriceSale != null && req.PriceSale > req.Price) throw new Exception("Giá tiền khuyễn mãi phải lớn hơn giá tiền gốc!");

                var res = await _productService.UpdateProductAsync(req, curentId);
                if (res == (int)ErrorProductCode.TitleEmpty) throw new Exception("Tiêu đề không được để trống!");
                if (res == (int)ErrorProductCode.CategoryNotFound) throw new Exception("Danh mục không tồn tại!");
                if (res == (int)ErrorProductCode.InvalidStatus) throw new Exception("Trạng thái không đúng!");
                if (res == (int)ErrorProductCode.PriceInvalid) throw new Exception("Giá tiền không hợp lệ!");
                if (res == (int)ErrorProductCode.PriceSaleInvalid) throw new Exception("Giá tiền khuyễn mãi không hợp lệ!");
                if (res == (int)ErrorProductCode.ItemNotFound) throw new Exception("Sản phẩm không tồn tại!");

                return new ResponseResult<string>(RetCodeEnum.Ok, "Cập nhật sản phẩm thành công!", res.ToString());
            }
            catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpPost]
        [Route("list")]
        public async Task<ResponseResult<ResponseList>> list([FromBody] MstProductFilter filter, int limit = 25, int page = 1)
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
        public async Task<ResponseResult<MstProduct>> detail(Guid id)
        {
            try
            {
                var res = await _productService.GetDetailProductAsync(id);

                if (res == null) throw new Exception("Sản phẩm không tồn tại!");

                return new ResponseResult<MstProduct>(RetCodeEnum.Ok, "Chi tiết sản phẩm!", res);
            }catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstProduct>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ResponseResult<MstDeletedRes>> delete([FromBody] List<Guid> listId)
        {
            try
            {
                var currentId = GetCurrentUserId();

                var res = await _productService.DeleteProductAsync(listId, currentId);

                return new ResponseResult<MstDeletedRes>(RetCodeEnum.Ok, "Xóa sản phẩm thành công!", res);
            }catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex,Request);
                return new ResponseResult<MstDeletedRes>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
