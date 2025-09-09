using ASOFT.Authentication.API.OAuthentication2.Endpoints;
using ASOFT.Authentication.API.OAuthentication2.Models;
using ASOFT.Authentication.API.OAuthentication2.Services;
using ASOFT.Authentication.API.OAuthentication2.Store;
using ASOFT.Authentication.API.OAuthentication2.Validators;
using ASOFT.Contract;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ASOFT.Authentication.API.OAuthentication2.Extensions
{
    public static class OAuth2ServiceCollectionExtensions
    {
        public static IServiceCollection AddOAuth2Service(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Checker.NotNull(services, nameof(services));

            services.AddTransient<IStartupFilter, AuthenticationStartupFilter>();

            // Add identity server
            var identityServerBuilder = services.AddIdentityServer(options =>
            {
                options.Endpoints = CreateEndpointsOptions();
            });

            // Add json web token
            //services.AddAuthentication().AddJwtBearer(AuthenticationSchemes.OAuth2Scheme, options =>
            //{
            //    var jwtSection = configuration.GetSection("IdentityServer:Jwt");
            //    options.Authority = jwtSection?.GetValue<string>("Authority");
            //    options.RequireHttpsMetadata = jwtSection?.GetValue<bool>("RequireHttpsMetadata") ?? false;
            //    options.Audience = jwtSection?.GetValue<string>("Audience");
            //});

            services.AddAuthentication()
                .AddIdentityServerAuthentication(AuthenticationSchemes.OAuth2Scheme, options =>
                {
                    var jwtSection = configuration.GetSection("IdentityServer:Jwt");
                    options.Authority = jwtSection?.GetSection("Authority").Value;
                    options.RequireHttpsMetadata = jwtSection?.GetValue<bool>("RequireHttpsMetadata") ?? false;
                    options.ApiName = jwtSection?.GetValue<string>("Audience");
                    options.ApiSecret = "As@123456";
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                });

            services.AddMvcCore(options =>
            {
                options.AllowCombiningAuthorizeFilters = true;
                var requiredUserPolicy = new AuthorizationPolicyBuilder(AuthenticationSchemes.OAuth2Scheme)
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(requiredUserPolicy));
            });

            identityServerBuilder.AddEndpoint<ASOFTTokenEndpoint>(ASOFTTokenEndpoint.Name, ASOFTTokenEndpoint.Path);

            // Api resources
            var resources = new List<ApiResource>();
            configuration.GetSection("IdentityServer:ApiResources").Bind(resources);
            resources.ForEach(m =>
            {
                foreach (var apiSecret in m.ApiSecrets)
                {
                    apiSecret.Value = apiSecret.Value.Sha512();
                }
            });
            identityServerBuilder.AddInMemoryApiResources(resources);

            // Add clients and encode client secret
            var clients = new List<Client>();
            configuration.GetSection("IdentityServer:Clients").Bind(clients);
            foreach (var client in clients)
            {
                foreach (var clientSecret in client.ClientSecrets)
                {
                    // Hash client secret
                    clientSecret.Value = clientSecret.Value.Sha512();
                }
            }

            // Add memory clients
            identityServerBuilder.AddInMemoryClients(clients);

            // Signin credential
            identityServerBuilder.AddDeveloperSigningCredential();

            // Add token creator service
            services.AddTransient<ITokenCreationService, ASOFTTokenCreationService>();

            // Add temporary user holder
            services.AddScoped<ITemporaryUserHolder, TemporaryUserHolder>();

            // Add user profile service when authenticated
            identityServerBuilder.AddProfileService<ASOFTProfileService>();

            // Add password validator
            identityServerBuilder.AddResourceOwnerValidator<ASOFTResourcesOwnerPasswordValidator>();

            services.AddTransient<IReferenceTokenStore, ASOFTReferenceTokenStore>();

            return services;
        }

        private static EndpointsOptions CreateEndpointsOptions()
        {
            // Disabled all endpoints
            return new EndpointsOptions
            {
                EnableTokenEndpoint = false,
                EnableAuthorizeEndpoint = false,
                EnableCheckSessionEndpoint = false,
                EnableDeviceAuthorizationEndpoint = false,
                EnableEndSessionEndpoint = false,
                EnableUserInfoEndpoint = false,
                EnableTokenRevocationEndpoint = false,
                EnableJwtRequestUri = false,
                //EnableIntrospectionEndpoint = false
            };
        }
    }
}