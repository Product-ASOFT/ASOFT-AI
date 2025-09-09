using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using Microsoft.EntityFrameworkCore;

namespace ASOFT.Core.Business.Common.DataAccess
{
    public class CoreCommonModelBuilderConfiguration : IModelBuilderConfiguration<BusinessDbContext>
    {
        /// <summary>
        /// Configure model
        /// </summary>
        /// <param name="modelBuilder"></param>
        public void ConfigureModel(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreCommonModelBuilderConfiguration).Assembly);
        }
    }
}
