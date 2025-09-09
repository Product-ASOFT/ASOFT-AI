using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace ASOFT.Core.Common.Localization.DependencyInjection
{
    public class CultureHolder : ICultureHolder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ICultureResult CultureResult => GetCultureResult();

        public CultureHolder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        private ICultureResult GetCultureResult()
        {
            var ctx = _httpContextAccessor.HttpContext;

            if (ctx == null)
            {
                return null;
            }

            var cultureResult = ctx.Features.Get<ICultureResultFeature>()?.CultureResult
                                ?? ctx.RequestServices
                                    .GetRequiredService<IOptions<RequestLocalizationConfigurations>>()
                                    .Value.DefaultCultureResult;
            return cultureResult;
        }
    }
}