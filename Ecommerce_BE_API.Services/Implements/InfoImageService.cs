using Ecommerce_BE_API.DbContext.Common;
using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.Services.Utils;
using Ecommerce_BE_API.Services.Utils.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace Ecommerce_BE_API.Services.Implements
{
    public class InfoImageService : IInfoImageService
    {
        private readonly ILoggerService _logger;
        private readonly IGenericDbContext<Ecommerce_BE_APIContext> _unitOfWork;

        public InfoImageService(ILoggerService logger, IGenericDbContext<Ecommerce_BE_APIContext> unitOfWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ResponseService<InfoImage>> AddAsync(InfoImageReq req, int currentUserId)
        {
            var result = new ResponseService<InfoImage>();
            result.status = (int)ErrorCategoryCode.Success;

            if (currentUserId <= 0)
            {
                result.status = (int)ErrorImageCode.UserNotFound;
                return result;
            }
            if (string.IsNullOrEmpty(req.ImageUrl))
            {
                result.status = (int)ErrorImageCode.ItemIsEmpty;
                return result;
            }

            var request = new InfoImage()
            {
                Id = Guid.NewGuid(),
                ImageUrl = req.ImageUrl,
                UserId = currentUserId,
                DeleteFlag = false,
                CreatedAt = DateTime.Now,
                CreatedBy = currentUserId
            };

            var res = await _unitOfWork.Repository<InfoImage>().AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            result.response = res.Entity;

            return result;
        }

        public async Task<ResponseService<InfoImage>> UpdateAsync(InfoImage req, int currentUserId)
        {
            var result = new ResponseService<InfoImage>();
            result.status = (int)ErrorCategoryCode.Success;

            if (req.UserId <= 0)
            {
                result.status = (int)ErrorImageCode.UserNotFound;
                return result;
            }
            if (string.IsNullOrEmpty(req.ImageUrl))
            {
                result.status = (int)ErrorImageCode.ItemIsEmpty;
                return result;
            }

            var request = await _unitOfWork.Repository<InfoImage>().Where(x => x.Id == req.Id && x.DeleteFlag != true)
                                                                    .FirstOrDefaultAsync();

            if(request == null)
            {
                request = new InfoImage()
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = req.ImageUrl,
                    UserId = currentUserId,
                    DeleteFlag = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = currentUserId
                };

                await _unitOfWork.Repository<InfoImage>().AddAsync(request);
            }
            else
            {
                request.ImageUrl = req.ImageUrl;
                request.UpdatedAt = DateTime.Now;
                request.UpdatedBy = currentUserId;

                _unitOfWork.Repository<InfoImage>().Update(request);
            }

            await _unitOfWork.SaveChangesAsync();

            result.response = request;

            return result;
        }

        public async Task<MstDeletedRes> DeleteAsync(List<Guid> listId, int currentUserId)
        {
            var result = new MstDeletedRes();

            if (listId == null || !listId.Any()) return result;

            var response = await _unitOfWork.Repository<InfoImage>().Where(x => x.DeleteFlag != true && listId.Contains(x.Id))
                                                                    .AsNoTracking().ToListAsync();
            if (!response.Any())
            {
                result.NotFoundIds = listId;
                return result;
            }

            var existingIds = response.Select(p => p.Id).ToList();

            result.DeletedIds = existingIds;
            result.NotFoundIds = listId.Except(existingIds).ToList();

            if (response.Any())
            {
                var utcNow = DateTime.Now;

                foreach (var item in response)
                {
                    item.DeleteFlag = true;
                    item.UpdatedAt = utcNow;
                    item.UpdatedBy = currentUserId;

                    var imageUrl = item.ImageUrl.TrimStart('/');
                    var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads/", imageUrl);

                    var test = System.IO.File.Exists(sourcePath);
                    if (System.IO.File.Exists(sourcePath))
                    {
                        var fileName = Path.GetFileName(sourcePath);
                        var trashPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", item.CreatedBy.ToString(), "trash");

                        if (!Directory.Exists(trashPath)) Directory.CreateDirectory(trashPath);

                        var destPath = Path.Combine(trashPath, fileName);

                        System.IO.File.Move(sourcePath, destPath, overwrite: true);

                        item.ImageUrl = $"{item.CreatedBy.ToString()}/trash/{fileName}";
                    }
                }
                _unitOfWork.Repository<InfoImage>().UpdateRange(response);
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }

        public async Task<InfoImage> GetDetailAsync(Guid id)
        {
            var res = await _unitOfWork.Repository<InfoImage>().Where(x => x.Id == id && x.DeleteFlag != true)
                                                                .AsNoTracking().FirstOrDefaultAsync();

            return res;
        }

        public async Task<ResponseList> GetListAsync(InfoImageFilter filter, int limit = 25, int page = 1)
        {
            var result = new ResponseList();

            var query = _unitOfWork.Repository<InfoImage>().Where(x => x.DeleteFlag != true);
            if (filter.UserId > 0) query = query.Where(x => x.UserId == filter.UserId);

            if (!string.IsNullOrEmpty(filter.TypeSort))
            {
                bool isDesc = filter.IsDesc ?? false;
                query = FunctionUtils.OrderByDynamic(query, filter.TypeSort, !isDesc);
            }
            else
            {
                query = query.OrderByDescending(o => o.CreatedAt);
            }

            query = query.AsNoTracking();

            var totalRow = await query.CountAsync();
            result.Paging = new Paging(totalRow, page, limit);
            int start = result.Paging.start;
            var responseList = await query.Skip(start).Take(limit).ToListAsync();
            result.ListData = responseList;

            return result;
        }

        public async Task<MstDeletedRes> HardDeleteAsync(List<Guid> listId, int currentUserId)
        {
            var result = new MstDeletedRes();

            if (listId == null || !listId.Any()) return result;

            var respose = await _unitOfWork.Repository<InfoImage>().Where(x => x.DeleteFlag != true && listId.Contains(x.Id))
                                                                    .AsNoTracking().ToListAsync();
            if (!respose.Any())
            {
                result.NotFoundIds = listId;
                return result;
            }

            var existingIds = respose.Select(p => p.Id).ToList();

            result.DeletedIds = existingIds;
            result.NotFoundIds = listId.Except(existingIds).ToList();

            if (respose.Any())
            {
                foreach(var item in respose)
                {
                    var imageUrl = item.ImageUrl.TrimStart('/');
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads/", imageUrl);

                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }

                _unitOfWork.Repository<InfoImage>().RemoveRange(respose);
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }

        public async Task<InfoImage> GetDetailAsync(string url)
        {
            var trimUrl = url.TrimStart('/');
            var res = await _unitOfWork.Repository<InfoImage>().Where(x => x.DeleteFlag != true)
                                                               .Where(x => x.ImageUrl == url || x.ImageUrl == trimUrl)
                                                                .AsNoTracking().FirstOrDefaultAsync();

            return res;
        }
    }
}
