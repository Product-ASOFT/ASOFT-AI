using ASOFT.Core.API.Httpss.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ASOFT.Core.API.Httpss.ActionResults
{
    /// <summary>
    /// Api object result
    /// </summary>
    public class ApiObjectResult : ObjectResult
    {
        /// <summary>
        /// Api object result
        /// </summary>
        /// <param name="value"></param>
        public ApiObjectResult(object value) : base(value)
        {
        }

        /// <summary>
        /// Format result
        /// </summary>
        /// <param name="context"></param>
        public override void OnFormatting(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!StatusCode.HasValue)
            {
                base.OnFormatting(context);
                return;
            }

            if (!(Value is BaseResponse responseV2) || responseV2.StatusCode.HasValue)
            {
                base.OnFormatting(context);
                return;
            }

            responseV2.StatusCode = StatusCode.Value;
            base.OnFormatting(context);
        }
    }
}