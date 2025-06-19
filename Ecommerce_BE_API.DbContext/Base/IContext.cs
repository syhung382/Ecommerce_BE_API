using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Ecommerce_BE_API.DbContext.Common;

namespace Ecommerce_BE_API.DbContext.Base
{
    public interface IContext : IDisposable
    {
        //T CreateDbContext<T>(IOptions<ConnectStringsSetting> connectOps) where T : class;
        DbSet<T> Repository<T>() where T : class;
        int SaveChange();
        Task<int> SaveChangeAsync();
    }
}
