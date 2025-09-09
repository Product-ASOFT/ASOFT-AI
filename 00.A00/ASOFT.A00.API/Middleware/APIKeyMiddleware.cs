// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    18/12/2020      Đoàn Duy      Tạo mới
// ##################################################################

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.API.Core.Middleware
{
    public class APIKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public APIKeyMiddleware(RequestDelegate next, ILoggerFactory logger)
        {
            _next = next;
            _logger = logger.CreateLogger(GetType());
        }

        /// <summary>
        /// Xử mý middleware kiểm tra API-Key cho mỗi request
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            string token = httpContext.Request.Headers["api-key"];
            string requestPath = httpContext.Request.Path.Value;

            //Nếu là các trang index thì ko cần kiểm tra
            if (!requestPath.Contains("/api/") || requestPath.Contains("getAvatar") || requestPath.Contains("getReviewImage") || requestPath.Contains("A00/Chat/GetFile")
                || requestPath.Contains("getCheckinImage") || requestPath.Contains("getNewsFile") || requestPath.Contains("Webhook") || requestPath.Contains("OAuth") || requestPath.Contains("SignIn"))
            {
                // var org = httpContext.Response.Body;
                // var resq = await ResToStringAsync(httpContext.Request);
                //_logger.LogInformation(resq);
                await _next(httpContext);
                //var resq2 = await ResposeToText(httpContext.Response);
                //_logger.LogInformation(resq2);

            }
            //Nếu api key không đúng
            else if (token != MWConstants.API_KEY)
            {
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync("Access denied!");
            }
            else
            {
                //Tiếp tục request
                await _next(httpContext);
            }

        }
    }
}
