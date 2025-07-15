using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
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
    public interface IMstUserService
    {
        Task<int> AddUserByRegisterAsync(MstUserRegisterReq userRequest);
        Task<int> AddUserInfoAsync(MstUserReq request, int role, int currentUserId);
        Task<int> UpdateUserInfoAsync(MstUser user, int role, int currentUserId);
        Task<MstUser> SyncUserInfoByUsernamePasswordAsync(LoginReq loginReq);
        Task<MstUser> getUserFromId(int id);
        Task<MstUser> getUserFromUsernameAsync(string username);
        Task<ResponseList> getListAsync(MstUserFilter filter, int? role, int limit = 25, int page = 1);
        Task<MstDeletedIntRes> DeleteAsync(List<int> listId, int currentUserId);
    }
}
