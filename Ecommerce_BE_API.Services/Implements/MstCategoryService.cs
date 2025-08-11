using Azure;
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
                DeleteFlag = req.DeleteFlag ?? false,
                CreatedAt = DateTime.Now,
                CreatedBy = currentUserId
            };

            if (req.ParentId != null)
            {
                var checkParrent = await ValidateParentId(req.ParentId);
                if (checkParrent == null) return (int)ErrorCategoryCode.ParentNotFound;
                else request.ParentId = checkParrent;
            }
            if (!FunctionUtils.IsStatusEnum(req.Status)) return (int)ErrorCategoryCode.InvalidStatus;
            if (string.IsNullOrEmpty(req.Title)) return (int)ErrorCategoryCode.TitleEmpty;

            var res = await _unitOfWork.Repository<MstCategory>().AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return (int)ErrorCategoryCode.Success;
        }

        public async Task<int> UpdateCategoryAsync(MstCategory req, int currentUserId)
        {
            var request = await _unitOfWork.Repository<MstCategory>().Where(x => x.Id == req.Id && x.DeleteFlag != true).FirstOrDefaultAsync();

            if(request == null)
            {
                return (int)ErrorCategoryCode.ItemNotFound;
            }

            if (req.ParentId != null)
            {
                var checkParrent = await ValidateParentId(req.ParentId);
                if (checkParrent == null) return (int)ErrorCategoryCode.ParentNotFound;
                else request.ParentId = checkParrent;
            }
            if (!FunctionUtils.IsStatusEnum(req.Status)) return (int)ErrorCategoryCode.InvalidStatus;
            if (string.IsNullOrEmpty(req.Title)) return (int)ErrorCategoryCode.TitleEmpty;

            request.Title = req.Title;
            request.Description = req.Description;
            request.Image = req.Image;
            request.Status = req.Status;
            request.UpdatedAt = DateTime.Now;
            request.UpdatedBy = currentUserId;

            var res = _unitOfWork.Repository<MstCategory>().Update(request);
            await _unitOfWork.SaveChangesAsync();

            return (int)ErrorCategoryCode.Success;
        }

        public async Task<ResponseList> GetListCategoryAsync(MstCategoryFilter filter, int limit = 25, int page = 1)
        {
            var result = new ResponseList();

            var query = _unitOfWork.Repository<MstCategory>().Where(x => x.DeleteFlag != true);

            if (!string.IsNullOrEmpty(filter.Title))
            {   
                var keyword = FunctionUtils.RemoveVietnameseTones(filter.Title);
                query = query.Where(x => x.Title.ToLower().Contains(filter.Title.ToLower()) || x.Title.ToLower().Contains(keyword));
            }
            if (!string.IsNullOrEmpty(filter.TypeSort))
            {
                bool isDesc = filter.IsDesc ?? false;
                query = FunctionUtils.OrderByDynamic(query, filter.TypeSort, !isDesc);
            }
            else
            {
                query = query.OrderByDescending(o => o.CreatedAt);
            }

            var totalRow = await query.CountAsync();
            result.Paging = new Paging(totalRow, page, limit);
            int start = result.Paging.start;
            var responseList = await query.Skip(start).Take(limit).AsNoTracking().ToListAsync();

            var listParentId = responseList.Where(x => x.ParentId != null).Select(x => x.ParentId).Distinct().ToList();

            var listCategory = await _unitOfWork.Repository<MstCategory>().Where(x => 
                                                                                    x.DeleteFlag != true && 
                                                                                    listParentId.Contains(x.Id))
                                                                            .AsNoTracking().ToListAsync();
            var resList = responseList.Select(item => new MstCategryRes()
            {
                Id = item.Id,
                Title = item.Title,
                ParentCategory = item.ParentId != null ? listCategory.FirstOrDefault(x => x.Id == item.ParentId) : null,
                Image = item.Image,
                Description = item.Description,
                Status = item.Status,
                DeleteFlag = item.DeleteFlag,
                CreatedAt = item.CreatedAt,
                CreatedBy = item.CreatedBy,
                UpdatedAt = item.UpdatedAt,
                UpdatedBy = item.UpdatedBy,
            }).ToList();

            result.ListData = resList;
            return result;
        }

        public async Task<MstCategory> GetDetailCategoryAsync(Guid id)
        {
            var response = await _unitOfWork.Repository<MstCategory>()
                                            .Where(x => x.Id == id && x.DeleteFlag != true)
                                            .AsNoTracking().FirstOrDefaultAsync();
            return response;
        }

        public async Task<MstDeletedRes> DeleteCategoryAsync(List<Guid> listId, int currentUserId)
        {
            var result = new MstDeletedRes();

            var listResponse = await _unitOfWork.Repository<MstCategory>()
                                             .Where(x => listId.Any(p => p == x.Id)
                                                        && x.DeleteFlag != true)
                                             .ToListAsync();
            if (!listResponse.Any())
            {
                result.NotFoundIds = listId;
                return result;
            }

            var existingIds = listResponse.Select(p => p.Id).ToList();

            result.DeletedIds = existingIds;
            result.NotFoundIds = listId.Except(existingIds).ToList();

            var allChildIds = await _unitOfWork.Repository<MstCategory>()
                                .Where(x => x.DeleteFlag != true && x.ParentId != null && listId.Contains(x.ParentId.Value))
                                .Select(x => x.ParentId.Value)
                                .ToListAsync();

            var allProductCategoryIds = await _unitOfWork.Repository<MstProduct>()
                                                         .Where(x => x.DeleteFlag != true && listId.Contains(x.CategoryId))
                                                         .Select(x => x.CategoryId)
                                                         .ToListAsync();

            var restrictedIds = allChildIds.Union(allProductCategoryIds).Distinct().ToList();

            var canDelete = listResponse.Where(x => !restrictedIds.Contains(x.Id)).ToList();

            var cannotDeleteIds = listResponse.Select(x => x.Id)
                                     .Where(id => restrictedIds.Contains(id))
                                     .ToList();

            result.NotFoundIds.AddRange(cannotDeleteIds);

            if (canDelete.Any())
            {
                foreach (var item in canDelete)
                {
                    item.DeleteFlag = true;
                    item.UpdatedAt = DateTime.Now;
                    item.UpdatedBy = currentUserId;
                }
                _unitOfWork.Repository<MstCategory>().UpdateRange(canDelete);
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }

        public async Task<List<MstCategory>> GetListCategoryDropdownAsync(MstCategoryFilter filter)
        {

            var query = _unitOfWork.Repository<MstCategory>().Where(x => x.DeleteFlag != true);

            if (filter.NoParent == true) query = query.Where(x => x.ParentId == null);
            if (!string.IsNullOrEmpty(filter.Title)) query = query.Where(x => x.Title.ToLower().Contains(filter.Title.ToLower()));
            if (!string.IsNullOrEmpty(filter.TypeSort))
            {
                bool isDesc = filter.IsDesc ?? false;
                query = FunctionUtils.OrderByDynamic(query, filter.TypeSort, !isDesc);
            }
            else
            {
                query = query.OrderByDescending(o => o.CreatedAt);
            }
            var responseList = await query.AsNoTracking().ToListAsync();

            return responseList;
        }

        public async Task<List<MstCategory>> GetListCategoryHasProductAsync(int limit = 25)
        {
            var categoriesWithProducts = await _unitOfWork.Repository<MstProduct>()
                .Where(p => !p.DeleteFlag)
                .Select(p => p.CategoryId)
                .Distinct()
                .Join(
                    _unitOfWork.Repository<MstCategory>(),
                    productCategoryId => productCategoryId,
                    category => category.Id,
                    (productCategoryId, category) => category
                )
                .Where(c => !c.DeleteFlag) 
                .OrderBy(c => c.Title)     
                .Take(limit)
                .ToListAsync();

            return categoriesWithProducts;
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
