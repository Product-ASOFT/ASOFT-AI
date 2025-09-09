using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using Microsoft.EntityFrameworkCore;

namespace ASOFT.CoreAI.Infrastructure
{
    /// <summary>
    /// Configuration
    /// </summary>
    public class ModuleCoreAIModelBuilderConfiguration : IModelBuilderConfiguration<BusinessDbContext>
    {
        /// <summary>
        /// Configure model
        /// </summary>
        /// <param name="modelBuilder"></param>
        public void ConfigureModel(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ModuleCoreAIModelBuilderConfiguration).Assembly);
        }
    }
}