using ASOFT.Core.API.Extensions;
using ASOFT.Core.Business.Users.API;
using ASOFT.Core.Business.Users.Business.Extensions;
using ASOFT.Core.Business.Users.DataAccsess.Extensions;
using ASOFT.Core.Common.Security;
using ASOFT.Core.Common.Security.Identity.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

[assembly: HostingStartup(typeof(UserBusinessAPIHostingStartup))]

namespace ASOFT.Core.Business.Users.API
{
    public class UserBusinessAPIHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
        }

        private static void ConfigureServices(WebHostBuilderContext ctx, IServiceCollection services)
        {
            services.AddApiMvc(ctx.HostingEnvironment)
                .AddApplicationPart(typeof(UserBusinessAPIHostingStartup).Assembly);
            services.AddUsersDataAccessServices();
            services.AddAuthenticationApplicationServices();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var jwtSettings = new JwtSettings();
            ctx.Configuration.GetSection(JwtSettings.SectionKey).Bind(jwtSettings);

            var tokenValidationParams = CreateTokenValidationParameters(jwtSettings);

            services.AddSingleton<IJwtHelper>(sp => new JwtHelper(jwtSettings, tokenValidationParams));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = jwtSettings.Audience;
                    options.SaveToken = jwtSettings.SaveToken;
                    options.TokenValidationParameters = tokenValidationParams;
                });

            services.AddMvcCore(options =>
            {
                var requiredUserPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(requiredUserPolicy));
            });

            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }

        /// <summary>
        /// Create token validation parameters
        /// </summary>
        /// <param name="jwtSettings"></param>
        /// <returns></returns>
        private static TokenValidationParameters CreateTokenValidationParameters(JwtSettings jwtSettings)
        {
            return new TokenValidationParameters
            {
                ValidIssuer = jwtSettings.Issuer,
                ValidateIssuer = true,

                ValidAudience = jwtSettings.Audience,
                ValidateAudience = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ValidateIssuerSigningKey = true,

                ValidateLifetime = true
            };
        }
    }
}