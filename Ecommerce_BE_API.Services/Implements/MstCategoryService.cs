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
    public class MstCategoryService : IMstCategoryService
    {
        private readonly ILoggerService _logger;
        private readonly IGenericDbContext<Ecommerce_BE_APIContext> _unitOfWork;

        public MstCategoryService(ILoggerService logger, IGenericDbContext<Ecommerce_BE_APIContext> unitOfWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }


        public async Task<int> AddCategoryAsync(MstCategoryReq req, int currentUserId)
        {

            var request = new MstCategory
            {
                Id = Guid.NewGuid(),
                ParentId = req.ParentId,
                Title = req.Title,
                Description = req.Description,
                Image = req.Image,
                Status = req.Status,
                CreatedAt = DateTime.Now,
                CreatedBy = currentUserId
            };

            if (req.ParentId != null)
            {
                var checkParrent = await ValidateParentId(req.ParentId);
                if (checkParrent == null) return (int)CategoryErrorCode.ParentNotFound;
                else request.ParentId = checkParrent;
            }
            if (!FunctionUtils.IsStatusEnum(req.Status)) return (int)CategoryErrorCode.InvalidStatus;
            if (string.IsNullOrEmpty(req.Title)) return (int)CategoryErrorCode.TitleEmpty;

            var res = await _unitOfWork.Repository<MstCategory>().AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return (int)CategoryErrorCode.Success;
        }

        public async Task<int> UpdateCategoryAsync(MstCategory req, int currentUserId)
        {
            var request = await _unitOfWork.Repository<MstCategory>().Where(x => x.Id == req.Id && x.DeleteFlag != true).FirstOrDefaultAsync();

            if(request == null)
            {
                return (int)CategoryErrorCode.ItemNotFound;
            }

            if (req.ParentId != null)
            {
                var checkParrent = await ValidateParentId(req.ParentId);
                if (checkParrent == null) return (int)CategoryErrorCode.ParentNotFound;
                else request.ParentId = checkParrent;
            }
            if (!FunctionUtils.IsStatusEnum(req.Status)) return (int)CategoryErrorCode.InvalidStatus;
            if (string.IsNullOrEmpty(req.Title)) return (int)CategoryErrorCode.TitleEmpty;

            request.Title = req.Title;
            request.Description = req.Description;
            request.Image = req.Image;
            request.Status = req.Status;
            request.UpdatedAt = DateTime.Now;
            request.UpdatedBy = currentUserId;

            var res = _unitOfWork.Repository<MstCategory>().Update(request);
            await _unitOfWork.SaveChangesAsync();

            return (int)CategoryErrorCode.Success;
        }

        public async Task<ResponseList> GetListCategoryAsync(MstCategoryFilter filter, int limit = 25, int page = 1)
        {
            var result = new ResponseList();

            var query = _unitOfWork.Repository<MstCategory>().Where(x => x.DeleteFlag != true);

            if (!string.IsNullOrEmpty(filter.Title)) query = query.Where(x => x.Title.ToLower().Contains(filter.Title.ToLower()));
            if (!string.IsNullOrEmpty(filter.TypeSort))
            {
                query = FunctionUtils.OrderByDynamic(query, filter.TypeSort, !filter.IsDesc.Value);
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

        public async Task<MstCategory> GetDetailCategoryAsync(Guid id)
        {
            var response = await _unitOfWork.Repository<MstCategory>()
                                            .Where(x => x.Id == id && x.DeleteFlag != true)
                                            .AsNoTracking().FirstOrDefaultAsync();
            return response;
        }

        public async Task<CategoryDelRes> DeleteCategoryAsync(List<Guid> listId, int currentUserId)
        {
            var result = new CategoryDelRes();
            result.Status = 0;

            var listResponse = await _unitOfWork.Repository<MstCategory>()
                                             .Where(x => listId.Any(p => p == x.Id)
                                                        && x.DeleteFlag != true)
                                             .ToListAsync();
            if (listResponse.Any())
            {
                var allChildIds = await _unitOfWork.Repository<MstCategory>()
                                    .Where(x => x.DeleteFlag != true && x.ParentId != null && listId.Contains(x.ParentId.Value))
                                    .Select(x => x.ParentId.Value)
                                    .ToListAsync();

                var allProductCategoryIds = await _unitOfWork.Repository<MstProduct>()
                                                             .Where(x => x.DeleteFlag != true && listId.Contains(x.CategoryId))
                                                             .Select(x => x.CategoryId)
                                                             .ToListAsync();

                foreach (var item in listResponse)
                {
                    if (allChildIds.Contains(item.Id))
                    {
                        result.Id = item.Id;
                        result.Title = item.Title;
                        result.Status = (int)CategoryErrorCode.HasChildCategory;
                        return result;
                    }

                    if (allProductCategoryIds.Contains(item.Id))
                    {
                        result.Id = item.Id;
                        result.Title = item.Title;
                        result.Status = (int)CategoryErrorCode.HasRelatedProduct;
                        return result;
                    }

                    item.DeleteFlag = true;
                    item.UpdatedAt = DateTime.Now;
                    item.UpdatedBy = currentUserId;
                }

                _unitOfWork.Repository<MstCategory>().UpdateRange(listResponse);
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }


        #region "Private Methods"
        private async Task<Guid?> ValidateParentId(Guid? parentId)
        {
            if (parentId == null) return null;
            var parent = await _unitOfWork.Repository<MstCategory>()
                                          .Where(x => x.Id == parentId && x.DeleteFlag != true)
                                          .FirstOrDefaultAsync();
            return parent?.Id;
        }
        #endregion "Private Methods"
    }
}
