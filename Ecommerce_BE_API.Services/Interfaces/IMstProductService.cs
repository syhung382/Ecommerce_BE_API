using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.Services.Utils.Response;


namespace Ecommerce_BE_API.Services.Interfaces
{
    public interface IMstProductService
    {
        Task<int> AddProductAsync(MstProductReq req, int currentUserId);
        Task<int> UpdateProductAsync(MstProductReq req, int currentUserId);
        Task<ResponseList> GetListProductAsync(MstProductFilter filter, int limit = 25, int page = 1);
        Task<ResponseList> GetListProductAsync(MstProductFilter2 filter, int limit = 25, int page = 1);
        Task<MstProductRes> GetDetailProductAsync(Guid id);
        Task<MstDeletedRes> DeleteProductAsync(List<Guid> listId, int currentUserId);
    }
}
