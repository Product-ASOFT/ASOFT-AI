using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ASOFT.Core.DataAccess.Extensions
{
    public static class DbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseModelBuilderConfiguration(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] IModelBuilderConfigurationProvider modelConfigurationProvider)
        {
            Checker.NotNull(optionsBuilder, nameof(optionsBuilder));
            Checker.NotNull(modelConfigurationProvider, nameof(modelConfigurationProvider));
            ((IDbContextOptionsBuilderInfrastructure) optionsBuilder).AddOrUpdateExtension(modelConfigurationProvider);
            return optionsBuilder;
        }
    }
}