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
            if(req.ListImageUrl != null && req.ListImageUrl.Any())
            {
                var listImage = new List<InfoProductImage>();
                foreach(var item in req.ListImageUrl)
                {
                    var ProudctImage = new InfoProductImage()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = request.Id,
                        ImageUrl = item,
                        CreatedAt = DateTime.Now,
                        CreatedBy = currentUserId,
                        DeleteFlag = false
                    };
                    listImage.Add(ProudctImage);
                }

                await _unitOfWork.Repository<InfoProductImage>().AddRangeAsync(listImage);
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

        public async Task<MstProductRes> GetDetailProductAsync(Guid id)
        {
            var response = await _unitOfWork.Repository<MstProduct>()
                                            .Where(x => x.Id == id && x.DeleteFlag != true)
                                            .AsNoTracking().FirstOrDefaultAsync();
            if (response == null) return null;

            var result = new MstProductRes()
            {
                Id = response.Id,
                CategoryId = response.CategoryId,
                Title = response.Title,
                Description = response.Description,
                Detail = response.Detail,
                DiscountId = response.DiscountId,
                Image = response.Image,
                Price = response.Price,
                PriceSale = response.PriceSale,
                Status = response.Status,
                CreatedAt = response.CreatedAt,
                CreatedBy = response.CreatedBy,
                UpdatedAt = response.UpdatedAt,
                UpdatedBy = response.UpdatedBy,
                DeleteFlag = response.DeleteFlag,
            };

            var listTag = await _unitOfWork.Repository<InfoProductTag>().Where(x => x.ProductId == response.Id && x.DeleteFlag != true)
                                                                        .AsNoTracking().ToListAsync();
            if (listTag.Any())
            {
                var listTagId = listTag.Select(x => x.Id).ToList();
                var listTagById = await _unitOfWork.Repository<MstTagOfProduct>().Where(x => x.DeleteFlag != true && listTagId.Contains(x.Id))
                                                                                 .AsNoTracking().ToListAsync();
                if(listTagById.Any())
                {
                    var listTagRes = new List<InfoProductTagRes>();
                    foreach (var item in listTagRes)
                    {
                        var itemDetail = listTagById.Where(x => x.Id == item.Id).FirstOrDefault();
                        if(itemDetail != null)
                        {
                            var tagRes = new InfoProductTagRes()
                            {
                                Id = item.Id,
                                ProductId = item.ProductId,
                                TagOfProductId = itemDetail.Id,
                                TagTitle = itemDetail.Title,
                            };

                            listTagRes.Add(tagRes);
                        }
                       
                    }
                    result.ListTagRes = listTagRes;
                }
            }

            var listImage = await _unitOfWork.Repository<InfoProductImage>().Where(x => x.DeleteFlag != true && x.ProductId == result.Id)
                                                                            .AsNoTracking().ToListAsync();
            if (listImage.Any())
            {
                var listImageRes = new List<InfoProductUpdateImageReq>();
                foreach(var item in listImage)
                {
                    var imgRes = new InfoProductUpdateImageReq()
                    {
                        Id = item.Id,
                        ImageUrl = item.ImageUrl,

                    };

                    listImageRes.Add(imgRes);
                }
                result.listProductImage = listImageRes;
            }

            return result;
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

        public async Task<int> UpdateProductAsync(MstProductUpdateReq req, int currentUserId)
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

                var existingTags = await _unitOfWork.Repository<InfoProductTag>().Where(x => x.ProductId == request.Id && x.DeleteFlag != true)
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

            if(req.listProductImage != null && req.listProductImage.Any())
            {
                var listProductImageNew = req.listProductImage.Where(x => x.Id == null && x.ImageUrl != null).ToList();
                var listProductImageAdd = new List<InfoProductImage>();
                foreach(var item in listProductImageNew)
                {
                    var image = new InfoProductImage()
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = item.ImageUrl,
                        ProductId = request.Id,
                        CreatedAt = DateTime.Now,
                        CreatedBy = currentUserId,
                        DeleteFlag = false
                    };
                    listProductImageAdd.Add(image);
                }

                await _unitOfWork.Repository<InfoProductImage>().AddRangeAsync(listProductImageAdd);

                var listProductImageId = req.listProductImage.Where(x => x.Id != null).Select(x => x.Id).ToList();

                var listProductImageDelete = await _unitOfWork.Repository<InfoProductImage>().Where(x => 
                                                                                                    x.DeleteFlag != true && 
                                                                                                    !listProductImageId.Contains(x.Id) && 
                                                                                                    x.ProductId == request.Id)
                                                                                            .ToListAsync();
                if(listProductImageDelete.Any())
                {
                    _unitOfWork.Repository<InfoProductImage>().RemoveRange(listProductImageDelete);
                }
                
            }

            _unitOfWork.Repository<MstProduct>().Update(request);
            await _unitOfWork.SaveChangesAsync();

            return (int)ErrorProductCode.Success;
        }

    }
}
