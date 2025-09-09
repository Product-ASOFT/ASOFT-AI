using ASOFT.Core.API.Httpss.ApiResponse;
using ASOFT.Core.API.Https;
using System;

namespace ASOFT.Core.API.Httpss.ActionResults
{
    /// <summary>
    /// Success object result of T
    /// </summary>
    public class SuccessObjectResultV2<T> : ApiObjectResult
    {
        /// <summary>
        /// Success object result
        /// </summary>
        /// <param name="value"></param>
        public SuccessObjectResultV2(T value) : this(new SuccessResponse<T>(value))
        {
        }

        /// <summary>
        /// Success object result
        /// </summary>
        /// <param name="value"></param>
        /// <param name="successStatusCode"></param>
        public SuccessObjectResultV2(T value, int successStatusCode) : this(new SuccessResponse<T>(value),
            successStatusCode)
        {
        }

        /// <summary>
        /// Success object result
        /// </summary>
        /// <param name="value"></param>
        public SuccessObjectResultV2(SuccessResponse<T> value) : this(value, ApiStatusCodes.Ok200)
        {
        }

        /// <summary>
        /// Success object result
        /// </summary>
        /// <param name="value"></param>
        /// <param name="successStatusCode"></param>
        public SuccessObjectResultV2(SuccessResponse<T> value, int successStatusCode) : base(value)
            => StatusCode = GetAndCheckSuccessStatusCode(successStatusCode);

        private static int GetAndCheckSuccessStatusCode(in int statusCode)
            => ApiStatusCodes.IsSuccessStatusCode(statusCode)
                ? statusCode
                : throw new InvalidOperationException(
                    $"Current status code with value: '{statusCode}' must be success status code.");
    }
}