using System.Collections.Generic;

namespace ASOFT.Core.DataAccess.Relational
{
    /// <summary>
    /// Option cho bulk operators.
    /// </summary>
    public struct BulkOptions
    {
        /// <summary>
        /// Size batch
        /// </summary>
        public int BatchSize { get; }

        /// <summary>
        /// Timeout
        /// </summary>
        public int? BulkCopyTimeout { get; }

        /// <summary>
        /// Chỉ định loại bỏ những property nào.
        /// </summary>
        public List<string> ExcludeProperties { get; }

        /// <summary>
        /// Chỉ định sử dụng những property nào.
        /// </summary>
        public List<string> IncludeProperties { get; }

        /// <summary>
        /// Chỉ định những property nào cần được update.
        /// </summary>
        public List<string> UpdateProperties { get; }

        /// <summary>
        /// Constructor cho bulk options
        /// </summary>
        /// <param name="batchSize"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="excludeProperties"></param>
        /// <param name="includeProperties"></param>
        /// <param name="updateProperties"></param>
        public BulkOptions(int batchSize = 2000, int? bulkCopyTimeout = null, List<string> excludeProperties = null,
            List<string> includeProperties = null, List<string> updateProperties = null)
        {
            BatchSize = batchSize;
            BulkCopyTimeout = bulkCopyTimeout;
            ExcludeProperties = excludeProperties;
            IncludeProperties = includeProperties;
            UpdateProperties = updateProperties;
        }
    }
}