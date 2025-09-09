using System;

namespace ASOFT.Core.DataAccess.Relational
{
    /// <summary>
    /// Attribute cho thiết lập tên store
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class StoreAttribute : Attribute
    {
        /// <summary>
        /// Tên store
        /// </summary>
        public string StoreName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storeName">Tên của store</param>
        public StoreAttribute(string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentNullException(nameof(storeName));
            }

            StoreName = storeName;
        }
    }
}