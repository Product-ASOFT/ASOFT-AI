using ASOFT.Core.Common.InjectionChecker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ASOFT.Core.DataAccess.ModelBuilderConfiguration
{
    /// <summary>
    /// Cung cấp danh sách thiết lập model cho EF Core để EF Core thiết lập cho model.
    /// </summary>
    public class ModelBuilderConfigurationProvider<T> : IModelBuilderConfigurationProvider<T> where T : DbContext
    {
        private readonly IEnumerable<IModelBuilderConfiguration<T>> _modelBuilderConfigurations;

        /// <summary>
        /// Cung cấp danh sách thiết lập model cho EF Core để EF Core thiết lập cho model.
        /// </summary>
        /// <param name="modelBuilderConfigurations"></param>
        public ModelBuilderConfigurationProvider(IEnumerable<IModelBuilderConfiguration<T>> modelBuilderConfigurations)
        {
            Info = new InternalDbContextOptionsExtensionInfo(this, null);
            _modelBuilderConfigurations =
                Checker.NotNull(modelBuilderConfigurations, nameof(modelBuilderConfigurations));
        }

        /// <summary>
        /// Cung cấp danh sách thiết lập model cho EF Core để EF Core thiết lập cho model.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IModelBuilderConfiguration> ProvideModelBuilderConfigurations() =>
            _modelBuilderConfigurations;

        void IDbContextOptionsExtension.ApplyServices(IServiceCollection services)
        {

        }

        /// <inheritdoc />
        public void Validate(IDbContextOptions options)
        {
            // Do nothing
        }

        /// <inheritdoc />
        public DbContextOptionsExtensionInfo Info { get; }
    }
}