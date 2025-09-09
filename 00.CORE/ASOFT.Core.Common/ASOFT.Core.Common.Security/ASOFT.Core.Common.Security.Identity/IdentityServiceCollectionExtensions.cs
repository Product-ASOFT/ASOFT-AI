using ASOFT.Core.Common.InjectionChecker;
using Microsoft.Extensions.DependencyInjection;

namespace ASOFT.Core.Common.Security.Identity.Extensions
{
    /// <summary>
    /// Phương thức mở rộng hỗ trợ add authenticate viewer
    /// </summary>
    public static class IdentityServiceCollectionExtensions
    {
        /// <summary>
        /// Thêm viewer service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentity(this IServiceCollection services)
            => Checker.NotNull(services, nameof(services)).AddScoped<IIdentity, Identity>();
    }
}