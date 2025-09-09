using ASOFT.Core.API.Httpss.ActionResults;
using ASOFT.Core.API.Versions;
using ASOFT.Core.API.Https;
using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using SupportApiVersions = ASOFT.Core.API.Versions.SupportApiVersions;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace ASOFT.Core.API.Extensions
{
    /// <summary>
    /// Service collection extension
    /// </summary>
    public static class ApiServiceCollectionExtensions
    {
        /// <summary>
        /// Thiết lập ApiBehaviorOptions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureApiBehaviorOptions([NotNull] this IServiceCollection services,
            Action<ApiBehaviorOptions> setupAction = null)
        {
            Checker.NotNull(services, nameof(services));
            services.Configure<ApiBehaviorOptions>(options =>
            {
                setupAction?.Invoke(options);
            });
            return services;
        }

        /// <summary>
        /// Thiết lập cho version api
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddSupportApiVersions([NotNull] this IServiceCollection services,
            Action<ApiVersioningOptions> setupAction = null)
        {
            Checker.NotNull(services, nameof(services));
            services.AddApiVersioning(options =>
            {
                options.RouteConstraintName = SupportApiVersions.VersionReaderName;
                options.ApiVersionReader = SupportApiVersions.CombineApiVersion;
                options.DefaultApiVersion = SupportApiVersions.V_1_0;
                options.ErrorResponses = new ApiVersionErrorResponseProvider();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.RegisterMiddleware = true;
                options.ReportApiVersions = true;
                setupAction?.Invoke(options);
            });
            return services;
        }

        /// <summary>
        /// Thiết lập forwarded header
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiForwardedHeaderOptions([NotNull] this IServiceCollection services,
            Action<ForwardedHeadersOptions> setupAction = null)
        {
            Checker.NotNull(services, nameof(services));
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedForHeaderName = ApiHeaderNames.ASOFTXForwardedFor;
                options.ForwardedHostHeaderName = ApiHeaderNames.ASOFTXForwardedHost;
                options.ForwardedProtoHeaderName = ApiHeaderNames.ASOFTXForwardedProto;
                options.ForwardedHeaders = ForwardedHeaders.All;
                setupAction?.Invoke(options);
            });
            return services;
        }

        /// <summary>
        /// Setup api MVC
        /// </summary>
        /// <param name="services"></param>
        /// <param name="hostingEnvironment"></param>
        /// <returns></returns>
        public static IMvcBuilder AddApiMvc([NotNull] this IServiceCollection services,
            [NotNull] IWebHostEnvironment hostingEnvironment)
        {
            Checker.NotNull(services, nameof(services));
            Checker.NotNull(hostingEnvironment, nameof(hostingEnvironment));

            var mvcBuilder = services.AddControllers().AddNewtonsoftJson(options =>
                {
                    options.UseCamelCasing(true);
                    options.UseMemberCasing();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = hostingEnvironment.IsDevelopment()
                        ? NullValueHandling.Include
                        : NullValueHandling.Ignore;
                    DefaultContractResolver contractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy
                        {
                            ProcessDictionaryKeys = false
                        },

                    };
                    options.SerializerSettings.ContractResolver = contractResolver;

                })
                //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(type);
                });

            return mvcBuilder;
        }
    }
}