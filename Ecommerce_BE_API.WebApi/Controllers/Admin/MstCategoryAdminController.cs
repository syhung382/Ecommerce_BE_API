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
    public class MstCategoryAdminController : BaseApiController
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IMstCategoryService _categoryService;

        public MstCategoryAdminController(ILoggerService logger, IConfiguration config, IMstCategoryService categoryService)
        {
            _logger = logger;
            _config = config;
            _categoryService = categoryService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ResponseResult<string>> Create([FromBody]MstCategoryReq req)
        {
            var currentId = GetCurrentUserId();
            try
            {
                var res = await _categoryService.AddCategoryAsync(req, currentId);
                if (res == (int)ErrorCategoryCode.ParentNotFound) throw new Exception("Danh mục cha không tồn tại!");
                if (res == (int)ErrorCategoryCode.InvalidStatus) throw new Exception("Trạng thái không đúng!");
                if (res == (int)ErrorCategoryCode.TitleEmpty) throw new Exception("Tên không được để trống!");

                return new ResponseResult<string>(RetCodeEnum.Ok, "Tạo mới danh mục thành công!", res.ToString());
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<ResponseResult<string>> Update([FromBody]MstCategory req)
        {
            try
            {
                int currentId = GetCurrentUserId();
                var res = await _categoryService.UpdateCategoryAsync(req, currentId);

                if (res == (int)ErrorCategoryCode.ItemNotFound) throw new Exception("Danh mục không tồn tại!");
                if (res == (int)ErrorCategoryCode.ParentNotFound) throw new Exception("Danh mục cha không tồn tại!");
                if (res == (int)ErrorCategoryCode.InvalidStatus) throw new Exception("Trạng thái không đúng!");
                if (res == (int)ErrorCategoryCode.TitleEmpty) throw new Exception("Tên không được để trống!");

                return new ResponseResult<string>(RetCodeEnum.Ok, "Cập nhật danh mục thành công!", res.ToString());
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpPost]
        [Route("list")]
        public async Task<ResponseResult<ResponseList>> List([FromBody]MstCategoryFilter filter, int limit = 25, int page = 1)
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

        [HttpPost]
        [Route("list-no-parent")]
        public async Task<ResponseResult<List<MstCategory>>> ListNoParent([FromBody] MstCategoryFilter filter)
        {
            try
            {
                var res = await _categoryService.GetListCategoryNotParentAsync(filter);

                if (res == null) throw new Exception("Danh mục trống!");

                return new ResponseResult<List<MstCategory>>(RetCodeEnum.Ok, "Ok", res);
            }
            catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<List<MstCategory>>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpGet]
        [Route("detail/{id}")]
        public async Task<ResponseResult<MstCategory>> Get(Guid id)
        {
            try
            {
                var res = await _categoryService.GetDetailCategoryAsync(id);

                if (res == null) throw new Exception("Danh mục không tồn tại!");

                return new ResponseResult<MstCategory>(RetCodeEnum.Ok, RetCodeEnum.Ok.ToString(), res);
            }catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstCategory>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ResponseResult<MstDeletedRes>> Delete([FromBody] List<Guid> listDel)
        {
            try
            {
                var currentId = GetCurrentUserId();

                var res = await _categoryService.DeleteCategoryAsync(listDel, currentId);

                return new ResponseResult<MstDeletedRes>(RetCodeEnum.Ok, "Xóa thành công", res);
            }
            catch (Exception ex) 
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstDeletedRes>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
