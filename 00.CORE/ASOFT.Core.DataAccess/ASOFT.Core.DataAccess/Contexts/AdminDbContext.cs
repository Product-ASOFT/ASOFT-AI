using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.EFCore.Relational;
using Microsoft.EntityFrameworkCore;
using ASOFT.Core.DataAccess;

namespace ASOFT.Core.DataAccess
{
    public class AdminDbContext : RelationalDbContext, IAdminUnitOfWork
    {
        /// <inheritdoc />
        public AdminDbContext(DbContextOptions<AdminDbContext> options) : this(options as DbContextOptions)
        {
        }

        /// <inheritdoc />
        protected AdminDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Phương thức này sẽ tìm tất cả những class implements IEntityTypeConfiguration<T> 
            // để thực thi trong Assembly chỉ định.
            // Dùng typeof(BusinessDbContext).Assembly để lấy đúng Assembly tránh gọi GetType().Assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdminDbContext).Assembly);
        }
    }
}
