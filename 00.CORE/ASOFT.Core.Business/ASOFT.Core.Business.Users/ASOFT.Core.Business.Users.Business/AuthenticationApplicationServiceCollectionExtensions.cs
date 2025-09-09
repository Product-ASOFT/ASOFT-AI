using ASOFT.Core.Business.Users.Business.Business;
using ASOFT.Core.Business.Users.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ASOFT.Core.Business.Users.Business.Extensions
{
    public static class AuthenticationApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationApplicationServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddScoped<IAuthenticationBusiness, AuthenticationBusiness>();
            services.AddScoped<IMenuBusiness, MenuBusiness>();
            services.AddScoped<IScreenPermissionBusiness ,ScreenPermissionBusiness>();

            return services;
        }
    }
}