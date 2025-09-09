using System;

namespace ASOFT.Core.DataAccess.Extensions
{
    /// <summary>
    /// Extension class cho specification
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Thiết lập option cho câu truy vấn
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specification"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T WithQueryOptions<T>(this T specification, Action<QueryOptions> action)
            where T : IQuery
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var queryOptions = new QueryOptions();
            action(queryOptions);
            return specification.WithQueryOptions(queryOptions);
        }

        /// <summary>
        /// Thiết lập option cho câu truy vấn
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specification"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public static T WithQueryOptions<T>(this T specification, IQueryOptions queryOptions)
            where T : IQuery
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            if (queryOptions == null)
            {
                throw new ArgumentNullException(nameof(queryOptions));
            }

            specification.ReplaceQueryOptions(queryOptions);
            return specification;
        }
    }
}