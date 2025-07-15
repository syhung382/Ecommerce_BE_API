using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.Services.Utils.Response;
using Ecommerce_BE_API.WebApi.Controllers.Base;
using Ecommerce_BE_API.WebApi.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_BE_API.WebApi.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class MstCustomerAdminController : BaseApiController
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        private readonly IMstUserService _userService;

        public MstCustomerAdminController(ILoggerService logger, IConfiguration config, IMstUserService userService)
        {
            _logger = logger;
            _config = config;
            _userService = userService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ResponseResult<string>> create([FromBody] MstUserReq req)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var res = await _userService.AddUserInfoAsync(req, (int)UserRoleEnum.User, currentUserId);

                if (res == (int)ErrorUserCode.InvalidEmail) return ResponseError("Email không hợp lệ!");
                if (res == (int)ErrorUserCode.InvalidPassword) return ResponseError("Mật khẩu phải ít nhất 6 ký tự!");
                if (res == (int)ErrorUserCode.InvalidUsername) return ResponseError("Tên đăng nhập không hợp lệ!");
                if (res == (int)ErrorUserCode.InExistUsername) return ResponseError("Tên đăng nhập đã tồn tại!");
                if (res == (int)ErrorUserCode.InExistEmail) return ResponseError("Email đã tồn tại!");
                if (res == (int)ErrorUserCode.InvalidGender) return ResponseError("Giới tính không đúng!");

                return new ResponseResult<string>(RetCodeEnum.Ok, RetCodeEnum.Ok.ToString(), "Thêm mới thành công!");
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return ResponseError(ex.Message);
            }

            ResponseResult<string> ResponseError(string message)
            {
                return new ResponseResult<string>(RetCodeEnum.ApiError, message, null);
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<ResponseResult<string>> update([FromBody] MstUser req)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var res = await _userService.UpdateUserInfoAsync(req, (int)UserRoleEnum.User, currentUserId);

                if (res == (int)ErrorUserCode.InvalidEmail) return ResponseError("Email không hợp lệ!");
                if (res == (int)ErrorUserCode.InvalidPassword) return ResponseError("Mật khẩu phải ít nhất 6 ký tự!");
                if (res == (int)ErrorUserCode.InvalidUsername) return ResponseError("Tên đăng nhập không hợp lệ!");
                if (res == (int)ErrorUserCode.InExistUsername) return ResponseError("Tên đăng nhập đã tồn tại!");
                if (res == (int)ErrorUserCode.InExistEmail) return ResponseError("Email đã tồn tại!");
                if (res == (int)ErrorUserCode.InvalidGender) return ResponseError("Giới tính không đúng!");
                if (res == (int)ErrorUserCode.ItemNotFound) return ResponseError("User không tồn tại!");

                return new ResponseResult<string>(RetCodeEnum.Ok, RetCodeEnum.Ok.ToString(), "Cập nhật thành công!");
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return ResponseError(ex.Message);
            }

            ResponseResult<string> ResponseError(string message)
            {
                return new ResponseResult<string>(RetCodeEnum.ApiError, message, null);
            }
        }

        [HttpPost]
        [Route("list")]
        public async Task<ResponseResult<ResponseList>> list([FromBody] MstUserFilter filter, int limit = 25, int page = 1)
        {
            try
            {
                var res = await _userService.getListAsync(filter, (int)UserRoleEnum.User, limit, page);

                return new ResponseResult<ResponseList>(RetCodeEnum.Ok, "Danh sách!", res);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return ResponseError(ex.Message);
            }

            ResponseResult<ResponseList> ResponseError(string message)
            {
                return new ResponseResult<ResponseList>(RetCodeEnum.ApiError, message, null);
            }
        }

        [HttpGet]
        [Route("detail")]
        public async Task<ResponseResult<MstUser>> get(int id)
        {
            try
            {
                var res = await _userService.getUserFromId(id);
                if (res.DeleteFlag == true) return ResponseError("Tài khoản bị xóa!");
                if (res.IsBanned == (int)BannedEnum.Yes) return ResponseError("Tài khoản bị xóa!");

                return new ResponseResult<MstUser>(RetCodeEnum.Ok, "infomation", res);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return ResponseError(ex.Message);
            }

            ResponseResult<MstUser> ResponseError(string message)
            {
                return new ResponseResult<MstUser>(RetCodeEnum.ApiError, message, null);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ResponseResult<MstDeletedIntRes>> delete([FromBody] List<int> listDel)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var res = await _userService.DeleteAsync(listDel, currentUserId);

                return new ResponseResult<MstDeletedIntRes>(RetCodeEnum.Ok, "Xóa thành công!", res);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<MstDeletedIntRes>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }
    }
}
