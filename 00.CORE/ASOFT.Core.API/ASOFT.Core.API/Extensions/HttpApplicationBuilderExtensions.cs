using ASOFT.Core.API.Https;
using ASOFT.Core.Common.InjectionChecker;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ASOFT.Core.API.Extensions
{
    /// <summary>
    /// Extension builder cho http
    /// </summary>
    public static class HttpApplicationBuilderExtensions
    {
        /// <summary>
        /// Api https
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiHttps(this IApplicationBuilder applicationBuilder)
        {
            Checker.NotNull(applicationBuilder, nameof(applicationBuilder));

            var httpsSettings = applicationBuilder.ApplicationServices.GetService<HttpsSettings>();

            if (httpsSettings?.Enabled == true)
            {
                applicationBuilder.UseHsts();
                applicationBuilder.UseHttpsRedirection();
            }

            return applicationBuilder;
        }
    }
}