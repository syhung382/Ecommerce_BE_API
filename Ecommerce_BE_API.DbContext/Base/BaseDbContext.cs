using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.DbContext.Base
{
    public class BaseDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public BaseDbContext()
        { }

        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<T> Repository<T>() where T : class
        {
            return Set<T>();
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}

