using ASOFT.Core.Common.InjectionChecker;
using LinqKit;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Specification cho câu truy vấn lọc dữ liệu.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterQuery<T> : SourceQuery<T>
    {
        /// <summary>
        /// Filter expression
        /// </summary>
        public Expression<Func<T, bool>> Expression { get; }

        /// <summary>
        /// Specification cho câu truy vấn lọc dữ liệu.
        /// </summary>
        /// <param name="filter"></param>
        public FilterQuery(Expression<Func<T, bool>> filter)
            => Expression = filter ?? throw new ArgumentNullException(nameof(filter));

        /// <summary>
        /// Specification cho câu truy vấn lọc dữ liệu.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filter"></param>
        public FilterQuery(string id, Expression<Func<T, bool>> filter) : base(id)
            => Expression = filter ?? throw new ArgumentNullException(nameof(filter));

        /// <summary>
        /// Get queryable filter.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected override IQueryable<T> GetQuery(IQueryable<T> source) => source.Where(Expression);

        /// <summary>
        /// Implicit expression cho phép convert <see cref="FilterQuery{T}"></see> sang <see cref="Expression"></see>
        /// </summary>
        /// <param name="filterSpec"></param>
        public static implicit operator Expression<Func<T, bool>>(FilterQuery<T> filterSpec)
            => filterSpec.Expression;

        /// <summary>
        /// Không sử dụng operator này
        /// </summary>
        /// <param name="filterSpec"></param>
        /// <returns></returns>
        public static bool operator false(FilterQuery<T> filterSpec) => false;

        /// <summary>
        /// Không sử dụng operator này
        /// </summary>
        /// <param name="filterSpec"></param>
        /// <returns></returns>
        public static bool operator true(FilterQuery<T> filterSpec) => false;

        /// <summary>
        /// && Operator
        /// </summary>
        /// <param name="filterSpec1"></param>
        /// <param name="filterSpec2"></param>
        /// <returns></returns>
        public static FilterQuery<T> operator &(FilterQuery<T> filterSpec1,
            FilterQuery<T> filterSpec2)
            => new FilterQuery<T>(filterSpec1.Expression.And(filterSpec2.Expression));

        /// <summary>
        /// || Operator
        /// </summary>
        /// <param name="filterSpec1"></param>
        /// <param name="filterSpec2"></param>
        /// <returns></returns>
        public static FilterQuery<T> operator |(FilterQuery<T> filterSpec1,
            FilterQuery<T> filterSpec2)
            => new FilterQuery<T>(filterSpec1.Expression.Or(filterSpec2.Expression));

        /// <summary>
        /// ! Operator
        /// </summary>
        /// <param name="filterSpec"></param>
        /// <returns></returns>
        public static FilterQuery<T> operator !(FilterQuery<T> filterSpec)
            => new FilterQuery<T>(filterSpec.Expression.Not());
    }
}