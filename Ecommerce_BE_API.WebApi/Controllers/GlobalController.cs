using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.WebApi.Controllers.Base;
using Ecommerce_BE_API.WebApi.Models.Response;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IMstUserService _userService;
        private readonly IInfoImageService _infoImageService;

        public GlobalController(ILoggerService logger,
                                IConfiguration config,
                                IMstUserService userService,
                                IGlobalService globalService,
                                IMstUserService mstUserService,
                                IInfoImageService infoImageService)
        {
            _logger = logger;
            _config = config;
            _globalService = globalService;
            _userService = userService;
            _infoImageService = infoImageService;
        }

        [HttpGet]
        [Route("ping")]
        public async Task<ResponseResult<GlobalRes>> get()
        {
            try
            {
                var response = new GlobalRes();
                var service = new MstService();

                response.ServerTime = DateTime.Now;
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

        [HttpPost]
        [Route("upload-image")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<ResponseResult<InfoImage>> UploadImage([FromForm] InfoUploadImageReq req)
        {
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
            const long MaxFileSize = 10 * 1024 * 1024;
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentSession = GetCurrentUserSession();

                var file = req.File;

                if (file == null) throw new Exception("Hình ảnh không hợp lệ!");
                if (file.Length > MaxFileSize) throw new Exception("Hình ảnh giới hạn 10MB!");

                var originalName = Path.GetFileNameWithoutExtension(file.FileName).Trim().Replace(" ", "-");
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || Array.IndexOf(permittedExtensions, ext) < 0)
                    throw new Exception("Định dạng hình ảnh không được hỗ trợ!");

                var check = await _userService.getUserFromId(currentUserId);
                if (check == null) throw new Exception("authorize required!");
                if (check.CurrentSession != currentSession) throw new Exception("token is expired!");

                var folderUser = Path.Combine("uploads", currentUserId.ToString());
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderUser);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var newName = $"{originalName}{ext}";
                var filePath = Path.Combine(uploadPath, newName);

                var count = 1;
                while (System.IO.File.Exists(filePath))
                {
                    newName = $"{originalName}-{count}{ext}";
                    filePath = Path.Combine(uploadPath, newName);
                    count++;
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }


                var imageUrl = $"{currentUserId.ToString()}/{newName}";

                var request = new InfoImageReq()
                {
                    ImageUrl = imageUrl,

                };

                var res = await _infoImageService.AddAsync(request, currentUserId);

                return new ResponseResult<InfoImage>(RetCodeEnum.Ok, "Tải lên thành công!", res.response);
            }
            catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<InfoImage>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpDelete]
        [Route("delete-image")]
        [Authorize]
        public async Task<ResponseResult<string>> DeleteImage([FromBody] ImageReq req)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var res = new InfoImage();
                if(req.Id == null)
                {
                    res = await _infoImageService.GetDetailAsync(req.ImageUrl);
                }
                else
                {
                    res = await _infoImageService.GetDetailAsync(req.Id.Value);
                }
                if (res == null)
                {
                    if (!string.IsNullOrEmpty(req.ImageUrl))
                    {
                        var fileName = Path.GetFileName(req.ImageUrl);
                        var trashPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", currentUserId.ToString(), "trash");

                        if (!Directory.Exists(trashPath)) Directory.CreateDirectory(trashPath);

                        var destPath = Path.Combine(trashPath, fileName);

                        System.IO.File.Move(req.ImageUrl, destPath, overwrite: true);
                    }
                }
                else
                {
                    List<Guid> listId = new List<Guid>();
                    listId.Add(res.Id);
                    var deleteRes = await _infoImageService.DeleteAsync(listId, currentUserId);
                }

                return new ResponseResult<string>(RetCodeEnum.Ok, "deleted!", "ok");
            }catch(Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return new ResponseResult<string>(RetCodeEnum.ApiError, ex.Message, null);
            }
        }

        [HttpGet]
        [Route("get-image")]
        public async Task<IActionResult> GetImage([FromQuery] string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl)) throw new Exception("File path is null!");
                imageUrl = imageUrl.TrimStart('/');

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", imageUrl);

                if (!System.IO.File.Exists(filePath)) throw new Exception("Không tìm thấy hình ảnh!");

                var fileInfo = new FileInfo(filePath);

                return Ok(new
                {
                    fileName = Path.GetFileName(filePath),
                    sizeInBytes = fileInfo.Length,
                    sizeInKb = Math.Round(fileInfo.Length / 1024.0, 2),
                    contentType = GetContentType(filePath),
                    relativePath = imageUrl
                });
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return StatusCode(500, "Lỗi hệ thống khi tải hình ảnh!");
            }
        }

        [HttpGet]
        [Route("view-image")]
        public async Task<IActionResult> ViewImage([FromQuery] string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl)) throw new Exception("File path is null!");
                imageUrl = imageUrl.TrimStart('/');

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads/" + imageUrl);

                if (!System.IO.File.Exists(filePath)) throw new Exception("Không tìm thấy hình ảnh!");

                var contentType = GetContentType(filePath);
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                return File(stream, contentType);
            }
            catch (Exception ex)
            {
                await _logger.WriteErrorLogAsync(ex, Request);
                return StatusCode(500, "Lỗi hệ thống khi tải hình ảnh!");
            }
        }

        #region "private method"
        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream",
            };
        }
        #endregion "private method"
    }
}
