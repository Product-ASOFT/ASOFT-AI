using Newtonsoft.Json;
using System.Collections.Generic;

namespace ASOFT.Core.API.Httpss.Errors
{
    /// <summary>
    /// Error model version 2
    /// </summary>
    public class ErrorModelV2
    {
        /// <summary>
        /// Error model v2
        /// </summary>
        public ErrorModelV2()
        {
        }

        /// <summary>
        /// Error model v2
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <param name="details"></param>
        /// <param name="extensions"></param>
        public ErrorModelV2(string code = null, string description = null, string message = null,
            int? statusCode = null,
            IDictionary<string, object> details = null,
            IDictionary<string, object> extensions = null)
        {
            Code = code;
            Description = description;
            Message = message;
            StatusCode = statusCode;
            Details = details;
            Extensions = extensions;
        }

        /// <summary>
        /// Status code
        /// </summary>
        [JsonIgnore]
        public int? StatusCode { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Details
        /// </summary>
        public IDictionary<string, object> Details { get; set; }

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, object> Extensions { get; set; }
    }
}