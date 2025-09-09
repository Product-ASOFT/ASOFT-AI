using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess.Extensions;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Base EF Core repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public abstract class EFBaseContext<TEntity, TContext> : IContext<TEntity, TContext> where TEntity : class
        where TContext : DbContext, IUnitOfWork
    {
        /// <summary>
        /// The database context.
        /// </summary>
        protected TContext Context { get; }

        /// <summary>
        /// The unit of work pattern.
        /// </summary>
        public TContext UnitOfWork => Context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">The input db context.</param>
        protected EFBaseContext(TContext context)
        {
            Context = Checker.NotNull(context, nameof(context));
        }

        /// <summary>
        /// Getting set of current entity.
        /// </summary>
        /// <returns></returns>
        protected virtual DbSet<TEntity> EntitySet() => Context.Set<TEntity>();

        /// <summary>
        /// Generic getting set of entity
        /// </summary>
        /// <typeparam name="TOtherEntity">The type of entity</typeparam>
        /// <returns></returns>
        protected virtual DbSet<TOtherEntity> EntitySet<TOtherEntity>() where TOtherEntity : class =>
            Context.Set<TOtherEntity>();

        /// <summary>
        /// Getting set of current entity with option for tracking.
        /// </summary>
        /// <param name="isTracking">should tracking or not.</param>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> EntitySet(bool isTracking) => EntitySet().Tracking(isTracking);

        /// <summary>
        /// Truy vấn theo generic type
        /// </summary>
        /// <param name="isTracking"></param>
        /// <typeparam name="TOtherEntity"></typeparam>
        /// <returns></returns>
        protected virtual IQueryable<TOtherEntity> EntitySet<TOtherEntity>(bool isTracking) where TOtherEntity : class
            => Context.Set<TOtherEntity>().Tracking(isTracking);

        /// <summary>
        /// Thêm mới entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task AddAsync(TEntity entity,
            CancellationToken cancellationToken = default)
            => await EntitySet().AddAsync(entity, cancellationToken);

        /// <summary>
        /// Cập nhật entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(TEntity entity,
            CancellationToken cancellationToken = default)
        {
            EntitySet().Update(entity);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Xóa entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(TEntity entity,
            CancellationToken cancellationToken = default)
        {
            EntitySet().Remove(entity);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Thêm mới nhiều entity
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
            => await EntitySet().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Thêm mới nhiều entity
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task AddRangeAsync(params TEntity[] entities)
            => await EntitySet().AddRangeAsync(entities).ConfigureAwait(false);

        /// <summary>
        /// Cập nhật nhiều entity
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            EntitySet().UpdateRange(entities);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Cập nhật nhiều entity
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task UpdateRangeAsync(params TEntity[] entities)
        {
            EntitySet().UpdateRange(entities);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Xóa nhiều entity
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            EntitySet().RemoveRange(entities);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Xóa nhiều entity
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task RemoveRangeAsync(params TEntity[] entities)
        {
            EntitySet().RemoveRange(entities);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Specification for custom query
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Query(ISourceQuery<TEntity> spec)
            => Checker.NotNull(spec, nameof(spec)).Query(EntitySet().ApplySpecificationQueryOptions(spec.QueryOptions));

        /// <summary>
        /// Specification for custom query
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> QueryAsync(ISourceQuery<TEntity> spec)
            => await Checker.NotNull(spec, nameof(spec))
                .Query(EntitySet().ApplySpecificationQueryOptions(spec.QueryOptions))
                .ToListAsync().ConfigureAwait(false);

        /// <summary>
        /// Specification for custom query
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> QueryFirstOrDefaultAsync(ISourceQuery<TEntity> spec)
            => await Checker.NotNull(spec, nameof(spec))
                .Query(EntitySet().ApplySpecificationQueryOptions(spec.QueryOptions))
                .FirstOrDefaultAsync().ConfigureAwait(false);
    }
}