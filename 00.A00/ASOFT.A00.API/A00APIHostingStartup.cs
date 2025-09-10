using ASOFT.A00.API;
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
            IConfiguration config = webHostBuilderContext.Configuration;
        }
    }
}