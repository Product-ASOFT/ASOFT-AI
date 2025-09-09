using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using Microsoft.EntityFrameworkCore;

namespace ASOFT.A00.DataAccess
{
    /// <summary>
    /// Configuration
    /// </summary>
    public class ModuleA00ModelBuilderConfiguration : IModelBuilderConfiguration<BusinessDbContext>
    {
        /// <summary>
        /// Configure model
        /// </summary>
        /// <param name="modelBuilder"></param>
        public void ConfigureModel(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ModuleA00ModelBuilderConfiguration).Assembly);
        }
    }
}