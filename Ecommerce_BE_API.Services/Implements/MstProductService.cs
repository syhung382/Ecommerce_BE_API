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
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace Ecommerce_BE_API.Services.Implements
{
    public class MstProductService : IMstProductService
    {
        private readonly ILoggerService _logger;
        private readonly IGenericDbContext<Ecommerce_BE_APIContext> _unitOfWork;

        public MstProductService(ILoggerService logger, IGenericDbContext<Ecommerce_BE_APIContext> unitOfWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<int> AddProductAsync(MstProductReq req, int currentUserId)
        {
            if (string.IsNullOrEmpty(req.Title)) return (int)ErrorProductCode.TitleEmpty;
            if (req.Price < 0) return (int)ErrorProductCode.PriceInvalid;
            if (req.PriceSale != null && req.PriceSale >= req.Price) return (int)ErrorProductCode.PriceSaleInvalid;
            if (!FunctionUtils.IsStatusEnum(req.Status)) return (int)ErrorProductCode.InvalidStatus;
            var category = await _unitOfWork.Repository<MstCategory>()
                                            .Where(x => x.Id == req.CategoryId
                                                        && x.DeleteFlag != true
                                                        && x.Status == (int)StatusEnum.Active)
                                            .AsNoTracking().FirstOrDefaultAsync();
            if (category == null) return (int)ErrorProductCode.CategoryNotFound;

            var request = new MstProduct()
            {
                Id = Guid.NewGuid(),
                CategoryId = req.CategoryId,
                Title = req.Title,
                Price = req.Price,
                Description = req.Description,
                Detail = req.Detail,
                Image = req.Image,
                PriceSale = req.PriceSale,
                DeleteFlag = req.DeleteFlag ?? false,
                Status = req.Status,
                CreatedAt = DateTime.Now,
                CreatedBy = currentUserId,
            };

            if (req.DiscountId != null)
            {
                request.DiscountId = req.DiscountId;
            }

            if (req.ListTagId != null && req.ListTagId.Any())
            {
                var timeNow = DateTime.Now;

                var listProductTags = req.ListTagId.Select(tagId => new InfoProductTag
                {
                    Id = Guid.NewGuid(),
                    ProductId = request.Id,
                    TagOfProductId = tagId,
                    CreatedAt = timeNow,
                    CreatedBy = currentUserId,
                }).ToList();

                await _unitOfWork.Repository<InfoProductTag>().AddRangeAsync(listProductTags);
            }

            await _unitOfWork.Repository<MstProduct>().AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return (int)ErrorProductCode.Success;
        }

        public async Task<MstDeletedRes> DeleteProductAsync(List<Guid> listId, int currentUserId)
        {
            var result = new MstDeletedRes();

            if (listId == null || !listId.Any()) return result;

            var listResponse = await _unitOfWork.Repository<MstProduct>()
                                                .Where(x => listId.Contains(x.Id) && x.DeleteFlag != true)
                                                .ToListAsync();
            if (!listResponse.Any())
            {
                result.NotFoundIds = listId;
                return result;
            }

            var productIds = listResponse.Select(p => p.Id).ToList();

            result.DeletedIds = productIds;
            result.NotFoundIds = listId.Except(productIds).ToList();

            var allProductTag = await _unitOfWork.Repository<InfoProductTag>()
                                                  .Where(x => productIds.Contains(x.ProductId) && x.DeleteFlag != true)
                                                  .ToListAsync();

            var utcNow = DateTime.Now;

            foreach (var item in allProductTag)
            {
                item.DeleteFlag = true;
                item.UpdatedAt = utcNow;
                item.UpdatedBy = currentUserId;
            }

            foreach (var item in listResponse)
            {
                item.DeleteFlag = true;
                item.UpdatedAt = utcNow;
                item.UpdatedBy = currentUserId;
            }

            _unitOfWork.Repository<InfoProductTag>().UpdateRange(allProductTag);
            _unitOfWork.Repository<MstProduct>().UpdateRange(listResponse);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<MstProduct> GetDetailProductAsync(Guid id)
        {
            var response = await _unitOfWork.Repository<MstProduct>()
                                            .Where(x => x.Id == id && x.DeleteFlag != true)
                                            .AsNoTracking().FirstOrDefaultAsync();
            return response;
        }

        public async Task<ResponseList> GetListProductAsync(MstProductFilter filter, int limit = 25, int page = 1)
        {
            var result = new ResponseList();

            var query = _unitOfWork.Repository<MstProduct>().Where(x => x.DeleteFlag != true);

            if (!string.IsNullOrEmpty(filter.Title))
            {
                var keyword = FunctionUtils.RemoveVietnameseTones(filter.Title);
                query = query.Where(x => x.Title.ToLower().Contains(filter.Title) || x.Title.ToLower().Contains(keyword));
            }

            if(filter.CategoryId != null) query = query.Where(x => x.CategoryId == filter.CategoryId);

            if (filter.StartPrice != null) query = query.Where(x => (x.PriceSale != null && x.PriceSale >= filter.StartPrice)
                                                                    || (x.PriceSale == null && x.Price >= filter.StartPrice));

            if (filter.EndPrice != null) query = query.Where(x => (x.PriceSale != null && x.PriceSale <= filter.EndPrice)
                                                                    || (x.PriceSale == null && x.Price <= filter.EndPrice));

            if(filter.Status != null) query = query.Where(x => x.Status == filter.Status);

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

        public async Task<int> UpdateProductAsync(MstProductRes req, int currentUserId)
        {
            var request = await _unitOfWork.Repository<MstProduct>().Where(x => x.Id == req.Id && x.DeleteFlag != true)
                                            .FirstOrDefaultAsync();
            if(request == null) return (int)ErrorProductCode.ItemNotFound;

            if (string.IsNullOrEmpty(req.Title)) return (int)ErrorProductCode.TitleEmpty;

            if (req.Price <= 0) return (int)ErrorProductCode.PriceInvalid;

            if (req.PriceSale != null && req.PriceSale >= req.Price) return (int)ErrorProductCode.PriceSaleInvalid;

            if (!FunctionUtils.IsStatusEnum(req.Status)) return (int)ErrorProductCode.InvalidStatus;

            var category = await _unitOfWork.Repository<MstCategory>()
                                            .Where(x => x.Id == req.CategoryId
                                                        && x.DeleteFlag != true
                                                        && x.Status == (int)StatusEnum.Active)
                                            .AsNoTracking().FirstOrDefaultAsync();
            if (category == null) return (int)ErrorProductCode.CategoryNotFound;

            request.CategoryId = req.CategoryId;
            request.Title = req.Title;
            request.Price = req.Price;
            request.Description = req.Description;
            request.Detail = req.Detail;
            request.Image = req.Image;
            request.PriceSale = req.PriceSale;
            request.DeleteFlag = req.DeleteFlag;
            request.Status = req.Status;
            request.UpdatedAt = DateTime.Now;
            request.UpdatedBy = currentUserId;
            if (req.DiscountId != null)
            {
                request.DiscountId = req.DiscountId;
            }

            if (req.ListTagRes != null && req.ListTagRes.Any())
            {
                var tagIds = req.ListTagRes.Select(t => t.TagOfProductId).Distinct().ToList();

                var existingTags = await _unitOfWork.Repository<InfoProductTag>().Where(x => x.ProductId == request.Id && !x.DeleteFlag)
                                                    .ToListAsync();

                var tagIdsExist = existingTags.Select(x => x.TagOfProductId).ToHashSet();

                var tagIdsToAdd = tagIds.Except(tagIdsExist).ToList();

                var validTagsToAdd = await _unitOfWork.Repository<MstTagOfProduct>().Where(x => tagIdsToAdd.Contains(x.Id) && !x.DeleteFlag)
                                                      .AsNoTracking().Select(x => x.Id).ToListAsync();

                var tagsToRemove = existingTags.Where(x => !tagIds.Contains(x.TagOfProductId)).ToList();

                var now = DateTime.Now;

                var newTags = validTagsToAdd.Select(tagId => new InfoProductTag
                {
                    Id = Guid.NewGuid(),
                    ProductId = request.Id,
                    TagOfProductId = tagId,
                    CreatedAt = now,
                    CreatedBy = currentUserId
                }).ToList();

                if (newTags.Any())
                    await _unitOfWork.Repository<InfoProductTag>().AddRangeAsync(newTags);
                if (tagsToRemove.Any())
                    _unitOfWork.Repository<InfoProductTag>().RemoveRange(tagsToRemove);
            }

            _unitOfWork.Repository<MstProduct>().Update(request);
            await _unitOfWork.SaveChangesAsync();

            return (int)ErrorProductCode.Success;
        }
    }
}
