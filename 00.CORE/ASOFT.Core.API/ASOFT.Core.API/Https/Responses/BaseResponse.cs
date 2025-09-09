using Newtonsoft.Json;
using System.Collections.Generic;

namespace ASOFT.Core.API.Httpss.ApiResponse
{
    /// <summary>
    /// Base response v2
    /// </summary>
    public abstract class BaseResponse
    {
        /// <summary>
        /// Status code
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// Extensions data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, object> Extensions { get; set; }
    }
}