using System.Collections.Generic;

namespace ASOFT.Core.API.Paging
{
    /// <summary>
    /// Number size paging entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NumberSizePagingEntity<T> : AbstractPagingEntity<T>
    {
        /// <summary>
        /// Số trang hiện tại
        /// </summary>
        public int CurrentPageNumber { get; }

        /// <summary>
        /// Số dòng trên 1 trang hiện tại
        /// </summary>
        public int CurrentPageSize { get; }

        /// <summary>
        /// Number size paging entity
        /// </summary>
        /// <param name="items"></param>
        /// <param name="totalCount"></param>
        /// <param name="currentPageNumber"></param>
        /// <param name="currentPageSize"></param>
        public NumberSizePagingEntity(IEnumerable<T> items, long totalCount, int currentPageNumber, int currentPageSize)
            : base(items, totalCount)
        {
            CurrentPageNumber = currentPageNumber;
            CurrentPageSize = currentPageSize;
        }

        /// <summary>
        /// Có trang kế hay không
        /// </summary>
        public override bool HasNextPage => TotalCount > CurrentPageNumber * CurrentPageSize;

        /// <summary>
        /// Có tồn tại trang trước hay không
        /// </summary>
        public override bool HasPreviousPage =>
            CurrentPageNumber - 1 > 0 && TotalCount >= CurrentPageNumber * CurrentPageSize;

        /// <summary>
        /// Trang tiếp theo
        /// </summary>
        public virtual int NextPageNumber => CurrentPageNumber + 1;

        /// <summary>
        /// Trang trước
        /// </summary>
        public virtual int PreviousPageNumber => CurrentPageNumber - 1;
    }
}