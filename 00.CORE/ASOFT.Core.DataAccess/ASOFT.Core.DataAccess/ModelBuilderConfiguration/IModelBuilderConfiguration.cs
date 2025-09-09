using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace ASOFT.Core.DataAccess.ModelBuilderConfiguration
{
    /// <summary>
    /// Thiết lập cho EF core model
    /// </summary>
    public interface IModelBuilderConfiguration
    {
        /// <summary>
        /// Thiết lập cho entity.
        /// </summary>
        /// <param name="modelBuilder"></param>
        void ConfigureModel([NotNull] ModelBuilder modelBuilder);
    }

    /// <summary>
    /// Thiết lập cho EF core model
    /// </summary>
    public interface IModelBuilderConfiguration<T> : IModelBuilderConfiguration where T : DbContext
    {
    }
}