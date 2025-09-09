using System;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Base class cho query specification
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseQuery<T> : IQuery<T>
    {
        /// <inheritdoc />
        public string Id { get; protected set; }

        /// <summary>
        /// Query options
        /// </summary>
        public IQueryOptions QueryOptions { get; private set; } = new QueryOptions();

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        protected BaseQuery()
        {
        }

        /// <summary>
        /// Với id
        /// </summary>
        /// <param name="id"></param>
        protected BaseQuery(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Thay thế bằng query options mới
        /// </summary>
        /// <param name="queryOptions"></param>
        public void ReplaceQueryOptions(IQueryOptions queryOptions)
        {
            QueryOptions = queryOptions ?? throw new ArgumentNullException(nameof(queryOptions));
        }
    }
}