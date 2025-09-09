using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ASOFT.Core.DataAccess.Extensions
{
    /// <summary>
    /// Extension cho <see cref="IQueryable{T}"></see>
    /// </summary>
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> Tracking<TEntity>([NotNull] this IQueryable<TEntity> source, bool isTracking)
            where TEntity : class
        {
            Checker.NotNull(source, nameof(source));
            return isTracking ? source.AsTracking() : source.AsNoTracking();
        }

        public static IQueryable<TEntity> TagWithNull<TEntity>([NotNull] this IQueryable<TEntity> source,
            [CanBeNull] string tag) where TEntity : class
        {
            Checker.NotNull(source, nameof(source));
            if (tag == null)
            {
                return source;
            }

            return source.TagWith(tag);
        }

        public static IQueryable<TEntity> TagWithNull<TEntity>([NotNull] this IQueryable<TEntity> source,
            [CanBeNull] string tag,
            [NotNull] string defaultValue) where TEntity : class
        {
            Checker.NotNull(source, nameof(source));
            Checker.NotNull(defaultValue, nameof(defaultValue));
            if (tag == null)
            {
                return source.TagWith(defaultValue);
            }

            return source.TagWith(tag);
        }

        public static IQueryable<TEntity> IgnoreQueryFilters<TEntity>([NotNull] this IQueryable<TEntity> source,
            bool ignore) where TEntity : class
        {
            Checker.NotNull(source, nameof(source));
            return ignore ? source.IgnoreQueryFilters() : source;
        }

        public static IQueryable<TEntity> ApplySpecificationQueryOptions<TEntity>(
            [NotNull] this IQueryable<TEntity> source, [NotNull] IQueryOptions queryOptions)
            where TEntity : class
        {
            Checker.NotNull(source, nameof(source));
            Checker.NotNull(queryOptions, nameof(queryOptions));
            return source.Tracking(queryOptions.IsTracking)
                .IgnoreQueryFilters(queryOptions.IgnoreDefaultQueryFilter)
                .TagWithNull(queryOptions.Tag);
        }
    }
}