using System.Collections.Generic;

namespace ASOFT.Core.API.Paging
{
    /// <summary>
    /// Response model cho paging
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingResponseModel<T>
    {
        /// <summary>
        /// Data
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Thông tin paging
        /// </summary>
        public PagingLinks Paging { get; set; }

        /// <summary>
        /// Tổng số lượng
        /// </summary>
        public long? TotalCount { get; set; }
    }
}