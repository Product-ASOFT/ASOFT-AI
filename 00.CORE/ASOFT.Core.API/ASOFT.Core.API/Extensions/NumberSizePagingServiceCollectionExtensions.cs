using ASOFT.Core.API.Paging;
using Microsoft.Extensions.DependencyInjection;

namespace ASOFT.Core.API.Extensions
{
    /// <summary>
    /// Service collection extension
    /// </summary>
    public static class NumberSizePagingServiceCollectionExtensions
    {
        /// <summary>
        /// Number size paging services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNumberSizePaging(this IServiceCollection services)
            => services.AddScoped<INumberSizePagingAdapter, NumberSizePagingAdapter>();
    }
}