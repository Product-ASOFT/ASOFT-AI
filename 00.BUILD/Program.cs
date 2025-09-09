using ASOFT.A00.API;
using ASOFT.Core.API.Extensions;
using ASOFT.Core.Business.Common.API;
using ASOFT.Core.Business.Devices.API;
using ASOFT.Core.Business.Files.API;
using ASOFT.Core.Business.Users.API;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using ASOFT.OO.API;

namespace ASOFT.TaskManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.ConfigureKestrel(options => {
                      options.AddServerHeader = false;
                      options.Limits.MinRequestBodyDataRate = null;
                  });

                  webBuilder.UseContentRoot(Directory.GetCurrentDirectory());

                  webBuilder.ConfigureAppConfiguration((hostingContext, configure) =>
                  {
                      configure.SetBasePath(Directory.GetCurrentDirectory());
                      configure.AddEnvironmentVariables();
                  });

                  webBuilder.UseStartup<CoreStartup>();

                  webBuilder.UseASOFTSerilog();

                  webBuilder.UseSetting(WebHostDefaults.PreventHostingStartupKey, "true");
                  webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                  {
                      config.SetBasePath(Directory.GetCurrentDirectory());

                      config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                      config.AddEnvironmentVariables();
                  });
                  foreach (var hostingStartup in new IHostingStartup[]
                  {
                        new CommonHostingStartUp(),
                        new UserBusinessAPIHostingStartup(),
                        new AIHostingStartup(),
                        new DeviceInfoHostingStartup(),
                        new FileManagementHostingStartup(),
                        new A00APIHostingStartup(),
                  })
                  {
                      hostingStartup.Configure(webBuilder);
                  }
              });

    }
}


