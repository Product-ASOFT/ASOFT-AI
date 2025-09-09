using System.Threading;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Options cho việc truy vấn.
    /// </summary>
    public interface IQueryOptions
    {
        /// <summary>
        /// Loại bỏ các điều kiện truy vấn mặc định.
        /// </summary>
        bool IgnoreDefaultQueryFilter { get; }

        /// <summary>
        /// set <code>false</code> để tăng performance của câu truy vấn.
        /// </summary>
        bool IsTracking { get; }

        /// <summary>
        /// Đính tag trên câu truy vấn.
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// Cancellation token
        /// </summary>
        CancellationToken CancellationToken { get; }
    }
}