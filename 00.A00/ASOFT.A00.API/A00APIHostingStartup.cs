using ASOFT.A00.API;
using ASOFT.A00.API.Controllers;
using ASOFT.A00.Application.Extensions;
using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.DataAccess.Queries;
using ASOFT.Core.API.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(A00APIHostingStartup))]

namespace ASOFT.A00.API
{
    public class A00APIHostingStartup : IHostingStartup
    {
        private readonly IConfiguration _configuration;
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
        }

        private static void ConfigureServices(WebHostBuilderContext webHostBuilderContext, IServiceCollection services)
        {
            services.AddApiMvc(webHostBuilderContext.HostingEnvironment)
                .AddApplicationPart(typeof(A00APIHostingStartup).Assembly);
            services.AddTransient<IJwtHandler, JwtHandler>();
            IConfiguration config = webHostBuilderContext.Configuration;
            services.AddA00ApplicationServices();
        }
    }
}