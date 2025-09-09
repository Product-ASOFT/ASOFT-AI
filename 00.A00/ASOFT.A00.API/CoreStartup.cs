using ASOFT.A00.API.BaseQuartz;
using ASOFT.A00.Business;
using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.DataAccess.Queries;
using ASOFT.A00.DataAccess.Utilities;
using ASOFT.API.Core.Middleware;
using ASOFT.Core.API.Extensions;
using ASOFT.Core.API.Logging;
using ASOFT.Core.API.Versions;
using ASOFT.Core.Common.Localization.DependencyInjection.Extensions;
using ASOFT.Core.Common.Security.Identity.Extensions;
using ASOFT.Core.DataAccess.Extensions;
using ASOFT.Core.DataAccess;
using AutoMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.IO.Compression;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

// ReSharper disable All

namespace ASOFT.A00.API
{
    /// <summary>
    /// Server startup file
    /// </summary>
    public class CoreStartup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        /// <summary>
        /// Startup dùng chung.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="hostingEnvironment"></param>
        public CoreStartup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            DefaultDapperConfigurations.Configure();
            services.AddApplicationInsightsTelemetry();
            services.AddApiForwardedHeaderOptions();

            // Add custom mvc for ASOFT
            services.AddApiMvc(_hostingEnvironment).AddFluentValidation();

            // Add allow controller name end with "Async"
            services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });

            // Api versions
            services.AddSupportApiVersions();
            services.AddApiHttps(_configuration);
            services.AddIdentity();

            // Add custom localization for ASOFT
            services.AddLocalizationRequestCultures(new CommonRequestCultureOptionsProvider()
                .ProvideOptionsConfiguration());

            // Page number page size helper
            services.AddNumberSizePaging();
            services.ConfigureApiBehaviorOptions();
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.AddSwaggers(_configuration);

            services.AddCommonInfrastructureServices(_configuration);

            // Scan những assembly của ASOFT
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()
            //    .Where(m => m.GetName().Name.StartsWith("ASOFT", StringComparison.OrdinalIgnoreCase)));
            var allAsoftAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(m => m.GetName().Name.StartsWith("ASOFT", StringComparison.OrdinalIgnoreCase))
            .ToList();
            foreach (var asm in allAsoftAssemblies)
            {
                try
                {
                    Console.WriteLine($"Testing AutoMapper with: {asm.FullName}");
                    services.AddAutoMapper(asm);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR with {asm.FullName}: {ex.Message}");
                    throw;
                }
            }
            // MediatR
            services.TryAddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddMediatR(new[] { typeof(CoreStartup).Assembly }, configs => { configs.AsScoped(); });
            services.AddHttpContextAccessor();

            services.AddMemoryCache();

            services.AddCors(options =>
            {
                options.AddPolicy(name: "CorsPolicy",
                    policy =>
                    {
                        policy.AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed(_ => true)
                            .AllowCredentials();
                    });
            });

            services.AddControllers();
            services.AddSignalR();

            // [Tấn Thành] - [04/01/2020] - Begin add
            // AsoftEnvironment
            services.AddSingleton<IASOFTEnvironment, ASOFTEnvironmentQueries>();
            services.AddScoped<IASOFTCommonQueries, ASOFTCommonQueries>();

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IConfiguration configuration, IASOFTEnvironment environment)
        {
            // Use forward headers for reverse proxy
            app.UseForwardedHeaders();

            // Only use developer exception page when in development mode
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //add Middleware
            app.Use(async (context, next) =>
            {
                var origin = context.Request.Headers["Origin"];
                if (!string.IsNullOrEmpty(origin))
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                }

                await next.Invoke();
            });
            app.UseMiddleware<APIKeyMiddleware>();

            // Use localization per request
            app.UseRequestCultureLocalization();

            // Use https
            app.UseApiHttps();

            // Use response zip
            app.UseResponseCompression();

            // Use server static files
            app.UseStaticFiles();

            app.UseRouting();

            // Use cors
            app.UseCors("CorsPolicy");

            // Use authentication
            app.UseAuthentication();

            app.UseAuthorization();

            // Use documents for API
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "API Documents";
                c.SwaggerEndpoint($"/swagger/{SupportApiVersions.CurrentVersionStr}/swagger.json", "Docs");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/_routes", (EndpointDataSource es) =>
                    Results.Json(es.Endpoints
                        .OfType<RouteEndpoint>()
                        .Select(e => new { e.RoutePattern.RawText })));
            });

            // Set biến môi trường
            environment.SetEnvironment();

            // [Tấn Thành] - [25/01/2021] - Begin Add
            var scheduler = app.ApplicationServices.GetService<IScheduler>();
            //QuartzServiceUtility.StartJob<AutomationJobBusiness>(scheduler, configuration);
            // [Tấn Thành] - [25/01/2021] - End Add
        }
    }
}