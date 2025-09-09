using ASOFT.Core.API.Extensions;
using ASOFT.Core.Business.Common.Business;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASOFT.Core.Business.Common.API
{
    public class CommonHostingStartUp : IHostingStartup
    {
        private readonly IConfiguration _configuration;
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
        }

        private static void ConfigureServices(WebHostBuilderContext webHostBuilderContext, IServiceCollection services)
        {
            services.AddApiMvc(webHostBuilderContext.HostingEnvironment)
                .AddApplicationPart(typeof(CommonHostingStartUp).Assembly);
            IConfiguration config = webHostBuilderContext.Configuration;
            services.AddCoreCommonApplicationServices();
        }
    }
}
