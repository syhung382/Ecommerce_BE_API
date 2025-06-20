using Ecommerce_BE_API.DbContext.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

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

        public virtual DbSet<MstUser> MstUsers { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MstUser>(entity =>
            {
                entity.ToTable("MstUser");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Avatar).HasMaxLength(250);
                entity.Property(e => e.CodeInvite)
                    .HasMaxLength(10)
                    .IsFixedLength();
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.CurrentSession)
                    .HasMaxLength(10)
                    .IsFixedLength();
                entity.Property(e => e.Email).HasMaxLength(250);
                entity.Property(e => e.FullName).HasMaxLength(250);
                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
                entity.Property(e => e.Password).HasMaxLength(250);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.Property(e => e.UserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

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
