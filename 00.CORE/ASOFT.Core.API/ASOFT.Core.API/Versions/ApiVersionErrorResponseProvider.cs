using ASOFT.Core.API.Httpss.ApiResponse;
using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.API.Https;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace ASOFT.Core.API.Versions
{
    /// <summary>
    /// Custom error response provider
    /// </summary>
    public class ApiVersionErrorResponseProvider : DefaultErrorResponseProvider
    {
        /// <summary>
        /// Tạo error content
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override object CreateErrorContent(ErrorResponseContext context)
            => new ErrorResponse<ErrorModelV2>(new ErrorModelV2(DefaultErrorCodes.NotSupportApiVersion,
                context.MessageDetail, context.Message, ApiStatusCodes.BadRequest400));
    }
}