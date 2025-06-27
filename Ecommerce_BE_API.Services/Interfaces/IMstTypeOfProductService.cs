using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.Services.Utils.Response;

namespace Ecommerce_BE_API.Services.Interfaces
{
    public interface IMstTypeOfProductService
    {
        Task<int> AddTypeOfProductAsync(MstTypeOfProductReq req, int currentUserId);
        Task<int> UpdateTypeOfProductAsync(MstTypeOfProduct req, int currentUserId);
        Task<ResponseList> GetListTypeOfProductAsync(MstTypeOfProductFilter filter, int limit = 25, int page = 1);
        Task<MstTypeOfProduct> GetDetailTypeOfProductAsync(Guid id);
        Task<MstDeletedRes> DeleteTypeOfProductAsync(List<Guid> listId, int currentUserId);
    }
}
