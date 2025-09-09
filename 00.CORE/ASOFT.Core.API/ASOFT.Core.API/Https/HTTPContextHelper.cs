using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASOFT.A00.API.Middleware
{
    public class HTTPContextHelper
    {
        IHttpContextAccessor _httpContextAccessor;
        public HTTPContextHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string HandleAboutRequest()
        {
            // handle the request  
            string token = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];
            return token;
        }
    }
}
