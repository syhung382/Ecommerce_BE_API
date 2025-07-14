using Ecommerce_BE_API.DbContext.Common;
using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.Services.Utils;
using Ecommerce_BE_API.Services.Utils.Response;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_BE_API.Services.Implements
{
    public class MstTagOfProductService : IMstTagOfProductService
    {
        private readonly ILoggerService _logger;
        private readonly IGenericDbContext<Ecommerce_BE_APIContext> _unitOfWork;

        public MstTagOfProductService(ILoggerService logger, IGenericDbContext<Ecommerce_BE_APIContext> unitOfWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<int> AddTypeOfProductAsync(MstTagOfProductReq req, int currentUserId)
        {
            if (string.IsNullOrEmpty(req.Title)) return (int)ErrorTypeOfProductCode.TitleEmpty;
            if (!FunctionUtils.IsStatusEnum(req.Status)) return (int)ErrorTypeOfProductCode.InvalidStatus;

            var request = new MstTagOfProduct()
            {
                Id = Guid.NewGuid(),
                Title = req.Title,
                Status = req.Status,
                DeleteFlag = req.DeleteFlag ?? false,
                CreatedAt = DateTime.Now,
                CreatedBy = currentUserId,
            };

            await _unitOfWork.Repository<MstTagOfProduct>().AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return (int)ErrorTypeOfProductCode.Success;
        }

        public async Task<MstDeletedRes> DeleteTypeOfProductAsync(List<Guid> listId, int currentUserId)
        {
            var result = new MstDeletedRes();

            if(listId == null || !listId.Any()) return result;

            var listResponse = await _unitOfWork.Repository<MstTagOfProduct>().Where(x => x.DeleteFlag != true && listId.Contains(x.Id))
                                                                           .ToListAsync();

            if(!listResponse.Any())
            {
                result.NotFoundIds = listId;
                return result;
            }

            var existingIds = listResponse.Select(p => p.Id).ToList();

            result.DeletedIds = existingIds;
            result.NotFoundIds = listId.Except(existingIds).ToList();


            if (listResponse.Any())
            {
                var utcNow = DateTime.Now;

                foreach (var item in listResponse) 
                {
                    item.DeleteFlag = true;
                    item.UpdatedAt = utcNow;
                    item.UpdatedBy = currentUserId;
                }
                _unitOfWork.Repository<MstTagOfProduct>().UpdateRange(listResponse);
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }

        public async Task<MstTagOfProduct> GetDetailTypeOfProductAsync(Guid id)
        {
            var result = await _unitOfWork.Repository<MstTagOfProduct>().Where(x => x.Id == id && x.DeleteFlag != true)
                                                                         .AsNoTracking().FirstOrDefaultAsync();
            return result;
        }

        public async Task<ResponseList> GetListTypeOfProductAsync(MstTagOfProductFilter filter, int limit = 25, int page = 1)
        {
            var result = new ResponseList();

            var query = _unitOfWork.Repository<MstTagOfProduct>().Where(x => x.DeleteFlag != true);

            if(!string.IsNullOrEmpty(filter.Title))
            {
                var keyword = FunctionUtils.RemoveVietnameseTones(filter.Title);
                query = query.Where(x => x.Title.Contains(filter.Title) || x.Title.ToLower().Contains(keyword));
            }
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

        public async Task<List<MstTagOfProduct>> getListDropdown(MstTagOfProductFilter filter)
        {
            var query = _unitOfWork.Repository<MstTagOfProduct>().Where(x => x.DeleteFlag != true);

            if (!string.IsNullOrEmpty(filter.Title))
            {
                var keyword = FunctionUtils.RemoveVietnameseTones(filter.Title);
                query = query.Where(x => x.Title.Contains(filter.Title) || x.Title.ToLower().Contains(keyword));
            }
            if (filter.Status != null) query = query.Where(x => x.Status == filter.Status);

            if (!string.IsNullOrEmpty(filter.TypeSort))
            {
                bool isDesc = filter.IsDesc ?? false;
                query = FunctionUtils.OrderByDynamic(query, filter.TypeSort, !isDesc);
            }
            else
            {
                query = query.OrderByDescending(o => o.CreatedAt);
            }

            var result = await query.AsNoTracking().ToListAsync();

            return result;
        }

        public async Task<int> UpdateTypeOfProductAsync(MstTagOfProduct req, int currentUserId)
        {
            var request = await _unitOfWork.Repository<MstTagOfProduct>().Where(x => x.Id == req.Id && x.DeleteFlag != true)
                                                                          .FirstOrDefaultAsync();

            if (request == null) return (int)ErrorTypeOfProductCode.ItemNotFound;
            if (string.IsNullOrEmpty(req.Title)) return (int)ErrorTypeOfProductCode.TitleEmpty;
            if (!FunctionUtils.IsStatusEnum(req.Status)) return (int)ErrorTypeOfProductCode.InvalidStatus;

            request.Title = req.Title;
            request.Status = req.Status;
            request.DeleteFlag = req.DeleteFlag;
            request.UpdatedAt = DateTime.Now;
            request.UpdatedBy = currentUserId;

            _unitOfWork.Repository<MstTagOfProduct>().Update(request);
            await _unitOfWork.SaveChangesAsync();

            return (int)ErrorTypeOfProductCode.Success;
        }
    }
}
