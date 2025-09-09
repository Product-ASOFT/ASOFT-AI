using ASOFT.Core.Common.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace ASOFT.Core.API.Authentication.Extensions
{
    /// <summary>
    /// Phương thức mở rộng hỗ trợ add authenticate viewer
    /// </summary>
    public static class AuthenticationServiceCollectionExtensions
    {
        /// <summary>
        /// Thêm viewer service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddViewer(this IServiceCollection services)
            => Checker.NotNull(services, nameof(services)).AddScoped<IViewer, Viewer>();
    }
}