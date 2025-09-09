using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ASOFT.Core.Common.Localization.DependencyInjection
{
    public class RequestLocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLocalizationMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var serviceProvider = context.RequestServices;
            var cultureTransformer = serviceProvider.GetRequiredService<IRequestCultureTransformer>();
            var cultureResult = await cultureTransformer.TransformAsync(context).ConfigureAwait(false);

            if (cultureResult != null)
            {
                context.Features.Set<ICultureResultFeature>(new CultureResultFeature(cultureResult));
                SetCurrentThreadCulture(cultureResult);
            }
            else
            {
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger(GetType());
                logger.LogWarning("Culture result is not found.");
            }

            await _next(context);
        }

        private static void SetCurrentThreadCulture(ICultureResult cultureResult)
        {
            if (cultureResult.Culture != null)
            {
                CultureInfo.CurrentCulture = cultureResult.Culture;
            }

            if (cultureResult.UICulture != null)
            {
                CultureInfo.CurrentUICulture = cultureResult.UICulture;
            }
        }
    }
}