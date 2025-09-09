// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.API.Extensions;
using ASOFT.Core.Business.Files.Business.Extensions;
using ASOFT.Core.Business.Files.DataAccess.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ASOFT.Core.Business.Files.API
{
    public class FileManagementHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
        }

        private static void ConfigureServices(WebHostBuilderContext ctx, IServiceCollection services)
        {
            services.AddApiMvc(ctx.HostingEnvironment)
             .AddApplicationPart(typeof(FileManagementHostingStartup).Assembly);
            services.AddFileManagementApplicationServices();
            services.AddFileManagementInfrastructureServices();
        }
    }
}
