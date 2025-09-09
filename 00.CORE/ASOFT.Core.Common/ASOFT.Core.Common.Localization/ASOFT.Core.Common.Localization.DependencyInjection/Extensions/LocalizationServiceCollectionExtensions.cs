using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASOFT.Core.Common.Localization.DependencyInjection.Extensions
{
    public static class LocalizationServiceCollectionExtensions
    {
        /// <summary>
        /// Thêm localization services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizationRequestCultures(this IServiceCollection services,
            Action<RequestLocalizationConfigurations> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            services.Configure<RequestLocalizationConfigurations>(options);
            services.TryAddScoped<IRequestCultureTransformer, RequestCultureTransformer>();
            return services;
        }
    }
}