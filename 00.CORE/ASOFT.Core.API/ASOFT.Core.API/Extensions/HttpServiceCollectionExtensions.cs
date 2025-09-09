using ASOFT.Core.API.Https;
using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASOFT.Core.API.Extensions
{
    /// <summary>
    /// Http api service collection extension
    /// </summary>
    public static class HttpServiceCollectionExtensions
    {
        /// <summary>
        /// Add api https services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiHttps([NotNull] this IServiceCollection services,
            [NotNull] IConfiguration configuration)
        {
            Checker.NotNull(services, nameof(services));
            Checker.NotNull(configuration, nameof(configuration));

            var httpsSettings = new HttpsSettings();
            configuration.GetSection(HttpsSettings.SectionKey).Bind(httpsSettings);

            if (httpsSettings.Enabled)
            {
                AddApiHttps(services, httpsSettings);
            }

            return services;
        }

        /// <summary>
        /// Add api http services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpSettings"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiHttps([NotNull] this IServiceCollection services,
            [NotNull] HttpsSettings httpSettings)
        {
            Checker.NotNull(services, nameof(services));
            Checker.NotNull(httpSettings, nameof(httpSettings));

            services.AddSingleton(sp => httpSettings);
            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = httpSettings.HttpsPort ?? HttpsSettings.DefaultHttpsPort;
                options.RedirectStatusCode = ApiStatusCodes.TemporaryRedirect307;
            });

            return services;
        }
    }
}