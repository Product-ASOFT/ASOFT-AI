using System.Threading;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Options cho việc truy vấn
    /// </summary>
    public class QueryOptions : IQueryOptions
    {
        /// <inheritdoc />
        public bool IgnoreDefaultQueryFilter { get; set; }

        /// <inheritdoc />
        public bool IsTracking { get; set; }

        /// <inheritdoc />
        public string Tag { get; set; }

        /// <inheritdoc />
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Default
        /// </summary>
        public QueryOptions()
        {
            CancellationToken = default;
        }

        /// <summary>
        /// Default
        /// </summary>
        /// <param name="ignoreDefaultQueryFilter"></param>
        /// <param name="isTracking"></param>
        /// <param name="tag"></param>
        /// <param name="cancellationToken"></param>
        public QueryOptions(bool ignoreDefaultQueryFilter = false, bool isTracking = false,
            string tag = null,
            CancellationToken cancellationToken = default)
        {
            IgnoreDefaultQueryFilter = ignoreDefaultQueryFilter;
            IsTracking = isTracking;
            Tag = tag;
            CancellationToken = cancellationToken;
        }
    }
}