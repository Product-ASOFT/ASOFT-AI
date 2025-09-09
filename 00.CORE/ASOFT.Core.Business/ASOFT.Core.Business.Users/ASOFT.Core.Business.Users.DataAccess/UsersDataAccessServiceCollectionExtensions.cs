using ASOFT.Core.Business.Users.DataAccsess.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;
using ASOFT.Core.Business.Users.DataAccess.Interfaces;
using ASOFT.Core.Business.Users.DataAccsess.Interfaces;
using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.DataAccess.Queries;

namespace ASOFT.Core.Business.Users.DataAccsess.Extensions
{
    public static class UsersDataAccessServiceCollectionExtensions
    {
        public static IServiceCollection AddUsersDataAccessServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IAuthenticationQueries, AuthenticationQueries>();
            services.AddScoped<IScreenPermissionQueries, ScreenPermissionQueries>();
            services.AddScoped<IUserInfoQueries, UserInfoQueries>();
            services.AddScoped<IMenuQueries, MenuQueries>();


            return services;
        }
    }
}