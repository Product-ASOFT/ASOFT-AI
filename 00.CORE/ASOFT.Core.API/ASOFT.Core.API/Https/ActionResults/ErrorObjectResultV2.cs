using ASOFT.Core.API.Httpss.ApiResponse;
using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.API.Https;
using System;

namespace ASOFT.Core.API.Httpss.ActionResults
{
    /// <summary>
    /// Error object result
    /// </summary>
    public class ErrorObjectResultV2 : ApiObjectResult
    {
        /// <summary>
        /// Error object result
        /// </summary>
        /// <param name="error"></param>
        public ErrorObjectResultV2(ErrorModelV2 error) : this(error,
            error?.StatusCode ?? ApiStatusCodes.BadRequest400)
        {
        }

        /// <summary>
        /// Error object result
        /// </summary>
        /// <param name="error"></param>
        /// <param name="errorStatusCode"></param>
        public ErrorObjectResultV2(ErrorModelV2 error, int errorStatusCode) : this(new ErrorResponse(error),
            errorStatusCode)
        {
        }

        /// <summary>
        /// Error object result
        /// </summary>
        /// <param name="errorResponse"></param>
        public ErrorObjectResultV2(ErrorResponse errorResponse) : this(errorResponse, ApiStatusCodes.BadRequest400)
        {
        }

        /// <summary>
        /// Error object result
        /// </summary>
        /// <param name="errorResponse"></param>
        /// <param name="errorStatusCode"></param>
        public ErrorObjectResultV2(ErrorResponse errorResponse, int errorStatusCode) : base(errorResponse)
            => StatusCode = GetAndCheckErrorStatusCode(errorStatusCode);

        private static int GetAndCheckErrorStatusCode(in int statusCode)
            => ApiStatusCodes.IsErrorStatusCode(statusCode)
                ? statusCode
                : throw new InvalidOperationException(
                    $"Current status code with value: '{statusCode}' must be error status code.");
    }
}