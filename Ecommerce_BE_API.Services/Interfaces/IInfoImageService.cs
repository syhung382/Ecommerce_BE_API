using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.Services.Utils.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.Services.Interfaces
{
    public interface IInfoImageService
    {
        Task<ResponseService<InfoImage>> AddAsync(InfoImageReq req, int currentUserId);
        Task<ResponseService<InfoImage>> UpdateAsync(InfoImage req, int currentUserId);
        Task<ResponseList> GetListAsync(InfoImageFilter filter, int limit = 25, int page = 1);
        Task<ResponseList> GetListByUserIdAsync(InfoImageUserFilter filter, int userID, int limit = 25, int page = 1);
        Task<InfoImage> GetDetailAsync(Guid id);
        Task<InfoImage> GetDetailAsync(string url);
        Task<MstDeletedRes> DeleteAsync(List<Guid> listId, int currentUserId);
        Task<MstDeletedRes> HardDeleteAsync(List<Guid> listId, int currentUserId);
    }
}
