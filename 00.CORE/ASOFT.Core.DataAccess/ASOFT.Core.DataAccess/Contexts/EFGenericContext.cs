using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.Relational;
using ASOFT.Core.DataAccess.Relational.Context;
using Dapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Generic repository cho SQL Server
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class EFGenericContext<TEntity, TContext> :
        EFBaseContext<TEntity, TContext>,
        IBulkRepository<TEntity> where TEntity : class
        where TContext : DbContext, IRelationalUnitOfWork
    {
        IRelationalUnitOfWork IRootContext<IRelationalUnitOfWork>.UnitOfWork => Context;

        /// <summary>
        /// Generic repository cho SQL Server
        /// </summary>
        /// <param name="context"></param>
        public EFGenericContext(TContext context) : base(context)
        {
        }

        /// <summary>
        /// Thêm mới số lượng lớn entity.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="bulkOptions"></param>
        /// <returns></returns>
        public virtual async Task BulkAddAsync(IEnumerable<TEntity> entities, BulkOptions bulkOptions = default)
            => await Context.BulkInsertAsync(Checker.NotNull(entities, nameof(entities)).AsList(),
               ConvertToBulkConfig(bulkOptions)).ConfigureAwait(false);

        /// <summary>
        /// Thêm mới hoặc cập nhật số lượng lớn entity.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="bulkOptions"></param>
        /// <returns></returns>
        public virtual async Task BulkAddOrUpdateAsync(IEnumerable<TEntity> entities, BulkOptions bulkOptions = default)
            => await Context.BulkInsertOrUpdateAsync(Checker.NotNull(entities, nameof(entities)).AsList(),
               ConvertToBulkConfig(bulkOptions)).ConfigureAwait(false);

        /// <summary>
        /// Cập nhật số lượng lớn entity.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="bulkOptions"></param>
        /// <returns></returns>
        public virtual async Task BulkUpdateAsync(IEnumerable<TEntity> entities, BulkOptions bulkOptions = default)
            => await Context.BulkUpdateAsync(Checker.NotNull(entities, nameof(entities)).AsList(),
                ConvertToBulkConfig(bulkOptions)).ConfigureAwait(false);

        /// <summary>
        /// Xóa số lượng lớn entity.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="bulkOptions"></param>
        /// <returns></returns>
        public virtual async Task BulkDeleteAsync(IEnumerable<TEntity> entities, BulkOptions bulkOptions = default)
            => await Context.BulkDeleteAsync(Checker.NotNull(entities, nameof(entities)).AsList(),
                ConvertToBulkConfig(bulkOptions)).ConfigureAwait(false);

        private static BulkConfig ConvertToBulkConfig(BulkOptions bulkOptions)
        {
            Checker.NotNull(bulkOptions, nameof(bulkOptions));
            return new BulkConfig
            {
                PropertiesToExclude = bulkOptions.ExcludeProperties,
                PropertiesToInclude = bulkOptions.IncludeProperties,
                UpdateByProperties = bulkOptions.UpdateProperties,
                BatchSize = bulkOptions.BatchSize,
                BulkCopyTimeout = bulkOptions.BulkCopyTimeout
            };
        }
    }
}