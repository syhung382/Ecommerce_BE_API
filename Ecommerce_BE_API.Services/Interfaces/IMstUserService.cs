using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
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
        Task<MstUsers> AddUserInfoAsync(MstUserRegisterReq userRequest);
        Task<MstUsers> AddUserInfoAsync(MstUserRegisterReq userRequest, MstUsers userInvite, int? currentId, bool IsActive = false);
        Task<MstUsers> SyncUserInfoByUsernamePasswordAsync(LoginReq loginReq);
        Task<MstUsers> UpdateUserInfoAsync(MstUsers user);

    }
}
