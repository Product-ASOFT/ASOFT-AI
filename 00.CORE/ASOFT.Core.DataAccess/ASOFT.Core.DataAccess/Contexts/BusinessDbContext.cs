using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.EFCore.Relational;
using Microsoft.EntityFrameworkCore;
using ASOFT.Core.DataAccess;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Business db context
    /// </summary>
    public class BusinessDbContext : RelationalDbContext, IBusinessUnitOfWork
    {
        /// <inheritdoc />
        public BusinessDbContext(DbContextOptions<BusinessDbContext> options) : this(options as DbContextOptions)
        {
        }

        /// <inheritdoc />
        protected BusinessDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(28, 8);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Phương thức này sẽ tìm tất cả những class implements IEntityTypeConfiguration<T> 
            // để thực thi trong Assembly chỉ định.
            // Dùng typeof(BusinessDbContext).Assembly để lấy đúng Assembly tránh gọi GetType().Assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BusinessDbContext).Assembly);
        }
    }
}