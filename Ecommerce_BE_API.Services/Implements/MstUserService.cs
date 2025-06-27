using Ecommerce_BE_API.DbContext.Common;
using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Requests;
using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Ecommerce_BE_API.Services.Utils;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_BE_API.Services.Implements
{
    public class MstUserService : IMstUserService
    {
        private readonly ILoggerService _logger;
        private readonly IGenericDbContext<Ecommerce_BE_APIContext> _unitOfWork;

        public MstUserService(ILoggerService logger, IGenericDbContext<Ecommerce_BE_APIContext> unitOfWork, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public async Task<MstUser> AddUserInfoAsync(MstUserRegisterReq userRequest)
        {
            var KeyEncrypt = Configuration["Tokens:Key"];

            var req = new MstUser
            {
                Email = userRequest.Email,
                FullName = userRequest.FullName,
                UserName = userRequest.UserName,
                Password = FunctionUtils.CreateSHA256(KeyEncrypt, userRequest.Password),
                IsActived = (int)ActiveEnum.No,
                Gender = userRequest.Gender,
                Role = (int)UserRoleEnum.User,
                IsFirstLogin = (int)IsFirstLoginEnum.FirstLoggedIn,
                DeleteFlag = false,
                IsBanned = (int)BannedEnum.No,
                Status = (int)UserStatusEnum.Active,
                CreatedAt = DateTime.Now,
                CreatedBy = 0,
            };
            var res = await _unitOfWork.Repository<MstUser>().AddAsync(req);
            await _unitOfWork.SaveChangesAsync();

            return res.Entity;
        }

        public async Task<MstUser> AddUserInfoAsync(MstUserRegisterReq userRequest, MstUser userInvite, int? currentId, bool IsActive = false)
        {
            var KeyEncrypt = Configuration["Tokens:Key"];

            var req = new MstUser
            {
                Email = userRequest.Email,
                FullName = userRequest.FullName,
                UserName = userRequest.UserName,
                Password = FunctionUtils.CreateSHA256(KeyEncrypt, userRequest.Password),
                IsActived = (int)ActiveEnum.No,
                Gender = userRequest.Gender,
                InviteUserId = userInvite.Id,
                Role = (int)UserRoleEnum.User,
                IsFirstLogin = (int)IsFirstLoginEnum.FirstLoggedIn,
                DeleteFlag = false,
                IsBanned = (int)BannedEnum.No,
                Status = (int)UserStatusEnum.Active,
                CreatedAt = DateTime.Now,
                CreatedBy = currentId ?? 0,
            };
            var res = await _unitOfWork.Repository<MstUser>().AddAsync(req);
            await _unitOfWork.SaveChangesAsync();

            return res.Entity;
        }
        public async Task<MstUser> SyncUserInfoByUsernamePasswordAsync(LoginReq loginReq)
        {
            var KeyEncrypt = Configuration["Tokens:KeyUser"];
            var password = FunctionUtils.CreateSHA256(KeyEncrypt, loginReq.Password);
            var userRes = await _unitOfWork.Repository<MstUser>()
                                            .Where(x => x.UserName == loginReq.UserName
                                                    && x.Password == password
                                                    && x.Status == (int)UserStatusEnum.Active)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync();

            return userRes;
        }

        public async Task<MstUser> UpdateUserInfoAsync(MstUser user)
        {
            var res = _unitOfWork.Repository<MstUser>().Update(user);
            await _unitOfWork.SaveChangesAsync();
            return res.Entity;
        }
    }
}
