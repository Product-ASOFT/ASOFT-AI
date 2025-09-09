using Microsoft.Extensions.DependencyInjection;
using System;

namespace ASOFT.Core.Business.Devices.Business.Extensions
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ClientManagementApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// Thêm service cho tầng application
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientManagementApplicationServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            return services;
        }
    }
}