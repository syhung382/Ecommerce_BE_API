using Azure;
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
using Org.BouncyCastle.Ocsp;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

        public async Task<int> AddUserByRegisterAsync(MstUserRegisterReq userRequest)
        {
            var validate = await ValidateForm(userRequest.Email, userRequest.UserName, userRequest.Password);
            if (validate != 0) return validate;

            if(!FunctionUtils.IsGenderEnum(userRequest.Gender)) return (int)ErrorUserCode.InvalidGender;

            var req = new MstUser
            {
                Email = userRequest.Email,
                FullName = userRequest.FullName,
                UserName = userRequest.UserName,
                Password = EncryPawword(userRequest.Password),
                Avatar = "default/avt.jpg",
                IsActived = (int)ActiveEnum.No,
                Gender = userRequest.Gender,
                Role = (int)UserRoleEnum.User,
                IsFirstLogin = (int)IsFirstLoginEnum.FirstLoggedIn,
                DeleteFlag = false,
                CodeInvite = userRequest.UserName,
                IsBanned = (int)BannedEnum.No,
                Status = (int)UserStatusEnum.Active,
                CreatedAt = DateTime.Now,
                CreatedBy = 0,
            };

            if (!string.IsNullOrEmpty(userRequest.CodeInvite))
            {
                var userInvite = await getUserByCodeInviteAsync(userRequest.CodeInvite);
                if (userInvite != null) {
                    req.InviteUserId = userInvite.Id;
                }
            }

            var res = await _unitOfWork.Repository<MstUser>().AddAsync(req);
            await _unitOfWork.SaveChangesAsync();

            return (int)ErrorUserCode.Success;
        }

        public async Task<int> AddUserInfoAsync(MstUserReq req, int role, int currentUserId)
        {
            var validate = await ValidateForm(req.Email, req.UserName, req.Password);
            if (validate != 0) return validate;

            if (!FunctionUtils.IsGenderEnum(req.Gender)) return (int)ErrorUserCode.InvalidGender;
            if (!FunctionUtils.IsUserRoleEnum(req.Role)) return (int)ErrorUserCode.InvalidUserRole;
            if (role == (int)UserRoleEnum.Staff)
            {
                if (req.RoleAdmin == null || !FunctionUtils.IsAdminRoleEnum((int)req.RoleAdmin)) return (int)ErrorUserCode.InvalidAdminRole;
            }

            var request = new MstUser()
            {
                Avatar = req.Avatar,
                Email = req.Email,
                FullName = req.FullName,
                UserName = req.UserName,
                Password = EncryPawword(req.Password),
                Role = role,
                RoleAdmin = req.RoleAdmin,
                Gender = req.Gender,
                DeleteFlag = false,
                IsActived = (int)ActiveEnum.No,
                IsBanned = (int)BannedEnum.No,
                IsFirstLogin = (int)IsFirstLoginEnum.FirstLoggedIn,
                Status = (int)UserStatusEnum.Active,
                CreatedAt = DateTime.Now,
                CreatedBy = currentUserId
            };

            if (string.IsNullOrEmpty(req.Avatar))
            {
                request.Avatar = "default/avt.jpg";
            }

            var res = await _unitOfWork.Repository<MstUser>().AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return (int)ErrorUserCode.Success;
        }

        public async Task<ResponseList> getListAsync(MstUserFilter filter, int? role, int limit = 25, int page = 1)
        {
            var result = new ResponseList();

            var query = _unitOfWork.Repository<MstUser>().Where(x => x.DeleteFlag != true);
            if(role != null)
            {
                query = query.Where(x => x.Role == role);
            }
            if(!string.IsNullOrEmpty(filter.FullName))
            {
                var keyword = filter.FullName.ToLower();
                query = query.Where(x => x.FullName != null &&
                                 (x.FullName.ToLower().Contains(filter.FullName.ToLower())));
            }
            if(filter.Status != null)
            {
                query = query.Where(x => x.Status == filter.Status);
            }

            if (!string.IsNullOrEmpty(filter.TypeSort))
            {
                bool isDesc = filter.IsDesc ?? false;
                query = FunctionUtils.OrderByDynamic(query, filter.TypeSort, !isDesc);
            }
            else
            {
                query = query.OrderByDescending(o => o.CreatedAt);
            }

            var totalRow = await query.CountAsync();
            result.Paging = new Paging(totalRow, page, limit);
            int start = result.Paging.start;
            var responseList = await query.Skip(start).Take(limit).AsNoTracking().ToListAsync();
            result.ListData = responseList;

            return result;
        }

        public async Task<MstUser> getUserFromId(int id)
        {
            var res = await getUserByIdAsync(id);
            return res;
        }

        public async Task<MstUser> getUserFromUsernameAsync(string username)
        {
            var res = await getUserByUsernameAsync(username);

            return res;
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

        public async Task<int> UpdateUserInfoAsync(MstUserReq req, int role, int currentUserId)
        {
            var res = await _unitOfWork.Repository<MstUser>().Where(x => x.DeleteFlag != true && x.IsBanned != (int)BannedEnum.Yes && x.Id == req.Id)
                                                                          .FirstOrDefaultAsync();

            if (!FunctionUtils.IsUserRoleEnum(req.Role)) return (int)ErrorUserCode.InvalidUserRole;
            if (role == (int)UserRoleEnum.Staff)
            {
                if (req.RoleAdmin == null || !FunctionUtils.IsAdminRoleEnum((int)req.RoleAdmin)) return (int)ErrorUserCode.InvalidAdminRole;
            }

            if (res == null) return (int)ErrorUserCode.ItemNotFound;
            res.Avatar = req.Avatar;
            res.FullName = req.FullName;
            res.Gender = req.Gender;
            res.Role = role;
            res.RoleAdmin = req.RoleAdmin;

            if (string.IsNullOrEmpty(req.Avatar))
            {
                res.Avatar = "default/avt.jpg";
            }

            _unitOfWork.Repository<MstUser>().Update(res);
            await _unitOfWork.SaveChangesAsync();
            return (int)ErrorUserCode.Success;

        }

        public async Task<int> UpdateSessionAsync(int userId, string newSession)
        {
            var res = await _unitOfWork.Repository<MstUser>().Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (res == null) return (int)ErrorUserCode.ItemNotFound;

            res.CurrentSession = newSession;
            _unitOfWork.Repository<MstUser>().Update(res);
            await _unitOfWork.SaveChangesAsync();
            return (int)ErrorUserCode.Success;
        }

        public async Task<int> UpdateLastLoginDay(int userId)
        {
            var res = await _unitOfWork.Repository<MstUser>().Where(x => x.Id == userId).FirstOrDefaultAsync();

            if(res == null) return (int) ErrorUserCode.ItemNotFound;

            res.LastLoginDate = DateTime.Now;

            _unitOfWork.Repository<MstUser>().Update(res);
            await _unitOfWork.SaveChangesAsync() ;

            return (int)ErrorUserCode.Success;
        }

        public async Task<MstDeletedIntRes> DeleteAsync(List<int> listId, int currentUserId)
        {
            var result = new MstDeletedIntRes();

            var listResponse = await _unitOfWork.Repository<MstUser>()
                                             .Where(x => listId.Contains(x.Id) && 
                                                        !x.DeleteFlag && 
                                                        (x.RoleAdmin == null || x.RoleAdmin != (int)AdminRoleEnum.SuperAdmin))
                                             .ToListAsync();
            if (!listResponse.Any())
            {
                result.NotFoundIds = listId;
                return result;
            }

            var existingIds = listResponse.Select(p => p.Id).ToList();

            result.DeletedIds = existingIds;
            result.NotFoundIds = listId.Except(existingIds).ToList();

            foreach(var item in listResponse)
            {
                item.DeleteFlag = true;
                item.UpdatedAt = DateTime.Now;
                item.UpdatedBy = currentUserId;
            }
            _unitOfWork.Repository<MstUser>().UpdateRange(listResponse);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        #region "Private Methods"
        private string EncryPawword(string currentPasswod)
        {
            var KeyEncrypt = Configuration["Tokens:Key"];
            return FunctionUtils.CreateSHA256(KeyEncrypt, currentPasswod);
        }

        //validate
        private async Task<int> ValidateForm(string email, string username, string password)
        {
            if (string.IsNullOrEmpty(email) || !FunctionUtils.ValidateEmail(email))
            {
                return (int)ErrorUserCode.InvalidEmail;
            }
            if (string.IsNullOrEmpty(username))
            {
                return (int)ErrorUserCode.InvalidUsername;
            }
            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                return (int)ErrorUserCode.InvalidPassword;
            }
            if (await IsExistEmailAsync(email))
            {
                return (int)ErrorUserCode.InExistEmail;
            }
            if (await IsExistUsernameAsync(username))
            {
                return (int)ErrorUserCode.InExistUsername;
            }
            return (int)ErrorUserCode.Success;
        }
        
        //getUser
        private async Task<MstUser?> GetUserAsync(Expression<Func<MstUser, bool>> predicate)
        {
            return await _unitOfWork.Repository<MstUser>()
                                    .Where(predicate)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync();
        }
        private async Task<MstUser?> getUserByIdAsync(int id) => await GetUserAsync(x => x.Id == id);
        private async Task<MstUser?> getUserByEmailAsync(string email) => await GetUserAsync(x => x.Email == email);
        private async Task<MstUser?> getUserByUsernameAsync(string username) => await GetUserAsync(x => x.UserName == username);
        private async Task<MstUser?> getUserByCodeInviteAsync(string codeInvite) => await GetUserAsync(x => x.CodeInvite != null &&  x.CodeInvite == codeInvite);

        //check exist
        private async Task<bool> IsExistAsync(Expression<Func<MstUser, bool>> predicate)
        {
            return await _unitOfWork.Repository<MstUser>().AsNoTracking().AnyAsync(predicate);
        }
        private Task<bool> IsExistEmailAsync(string email) => IsExistAsync(x => x.Email == email);

        private Task<bool> IsExistUsernameAsync(string username) => IsExistAsync(x => x.UserName == username);

        #endregion "Private Methods"
    }
}
