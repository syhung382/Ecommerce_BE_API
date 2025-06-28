using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.Services.Utils.Response;

namespace Ecommerce_BE_API.Services.Interfaces
{
    public interface IMstTagOfProductService
    {
        Task<int> AddTypeOfProductAsync(MstTagOfProductReq req, int currentUserId);
        Task<int> UpdateTypeOfProductAsync(MstTagOfProduct req, int currentUserId);
        Task<ResponseList> GetListTypeOfProductAsync(MstTagOfProductFilter filter, int limit = 25, int page = 1);
        Task<MstTagOfProduct> GetDetailTypeOfProductAsync(Guid id);
        Task<MstDeletedRes> DeleteTypeOfProductAsync(List<Guid> listId, int currentUserId);
    }
}
