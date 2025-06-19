using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Ecommerce_BE_API.DbContext.Base;

namespace Ecommerce_BE_API.DbContext.Models
{
    public partial class Ecommerce_BE_APIContext : Microsoft.EntityFrameworkCore.DbContext, IContext, IDisposable
    {
        private bool _disposed = false;

        public Ecommerce_BE_APIContext()
        {
        }

        public Ecommerce_BE_APIContext(DbContextOptions<Ecommerce_BE_APIContext> options)
            : base(options)
        {
        }

        // <summary>

        public DbSet<T> Repository<T>() where T : class
        {
            return Set<T>();
        }

        public int SaveChange()
        {
            return base.SaveChanges();
        }

        public async Task<int> SaveChangeAsync()
        {
            return await base.SaveChangesAsync();
        }

        // <summary>

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

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
            }
            _disposed = true;
        }

        
    }
}
