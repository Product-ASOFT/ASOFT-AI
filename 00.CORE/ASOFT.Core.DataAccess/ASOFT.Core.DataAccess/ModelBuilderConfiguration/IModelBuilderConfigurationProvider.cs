using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;

namespace ASOFT.Core.DataAccess.ModelBuilderConfiguration
{
    /// <summary>
    /// Cung cấp danh sách thiết lập model cho EF Core để EF Core thiết lập cho model.
    /// </summary>
    public interface IModelBuilderConfigurationProvider : IDbContextOptionsExtension
    {
        /// <summary>
        /// Cung cấp danh sách thiết lập model cho EF Core để EF Core thiết lập cho model.
        /// </summary>
        IEnumerable<IModelBuilderConfiguration> ProvideModelBuilderConfigurations();
    }

    /// <summary>
    /// Cung cấp danh sách thiết lập model cho EF Core để EF Core thiết lập cho model.
    /// </summary>
    public interface IModelBuilderConfigurationProvider<T> : IModelBuilderConfigurationProvider where T : DbContext
    {
    }
}