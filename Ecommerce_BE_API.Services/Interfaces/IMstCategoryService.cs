using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.Services.Utils.Response;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.Services.Interfaces
{
    public interface IMstCategoryService
    {
        Task<int> AddCategoryAsync(MstCategoryReq req, int currentUserId);
        Task<int> UpdateCategoryAsync(MstCategory req, int currentUserId);
        Task<ResponseList> GetListCategoryAsync(MstCategoryFilter filter, int limit = 25, int page = 1);
        Task<MstCategory> GetDetailCategoryAsync(Guid id);
        Task<CategoryDelRes> DeleteCategoryAsync(List<Guid> listId, int currentUserId);
    }
}
