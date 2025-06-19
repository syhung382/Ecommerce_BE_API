using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Ecommerce_BE_API.DbContext.Base;

namespace Ecommerce_BE_API.DbContext.Common
{
    public class GenericDbContext<T> : IGenericDbContext<T> where T : Microsoft.EntityFrameworkCore.DbContext, IContext, IDisposable
    {
        private bool _disposed = false;
        private readonly T _dbContext;

        public GenericDbContext(T dbContext)
        {
            _dbContext = dbContext;
        }

        public DatabaseFacade Database { get { return _dbContext.Database; } }

        public T GetContext()
        {
            return _dbContext;
        }

        public DbSet<TEntity> Repository<TEntity>() where TEntity : class
        {
            return _dbContext.Repository<TEntity>();
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_disposed) return;
            if (isDisposing)
            {
                _dbContext.Dispose();
            }
            _disposed = true;
        }
    }
}
