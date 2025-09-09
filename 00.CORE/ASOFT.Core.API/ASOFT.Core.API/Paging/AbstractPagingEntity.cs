using System.Collections.Generic;
using System.Linq;

namespace ASOFT.Core.API.Paging
{
    /// <summary>
    /// Abstract paging entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractPagingEntity<T>
    {
        /// <summary>
        /// Tổng số dòng
        /// </summary>
        public long TotalCount { get; }

        /// <summary>
        /// Items
        /// </summary>
        public IEnumerable<T> Items { get; protected set; }

        /// <summary>
        /// Abstract paging entity
        /// </summary>
        /// <param name="items"></param>
        /// <param name="totalCount"></param>
        protected AbstractPagingEntity(IEnumerable<T> items, long totalCount)
        {
            Items = items ?? Enumerable.Empty<T>();
            TotalCount = totalCount;
        }

        /// <summary>
        /// Có trang kế tiếp hay không
        /// </summary>
        public abstract bool HasNextPage { get; }

        /// <summary>
        /// Có trang trước hay không
        /// </summary>
        public abstract bool HasPreviousPage { get; }
    }
}