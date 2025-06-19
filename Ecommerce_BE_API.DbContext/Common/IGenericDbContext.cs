using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Ecommerce_BE_API.DbContext.Base;

namespace Ecommerce_BE_API.DbContext.Common
{
    public interface IGenericDbContext<T> where T : Microsoft.EntityFrameworkCore.DbContext, IContext, IDisposable
    {
        DatabaseFacade Database { get; }
        DbSet<T> Repository<T>() where T : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void Dispose();
        T GetContext();
    }
}
