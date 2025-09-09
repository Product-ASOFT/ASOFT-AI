// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.API.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ASOFT.Core.Business.Devices.API
{
    /// <summary>
    /// Start up dùng cho notification api
    /// </summary>
    public class DeviceInfoHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        private static void ConfigureServices(WebHostBuilderContext ctx, IServiceCollection services)
        {
            services.AddApiMvc(ctx.HostingEnvironment)
             .AddApplicationPart(typeof(DeviceInfoHostingStartup).Assembly);
            //services.AddNotificationApplicationServices();
            //var serviceRegister = new ActiveMQServiceCollectionAdapter(services);
            //services.AddAmazonSNSServices(ctx.Configuration.GetSection("AWSSNSCredential"));

            //ASOFTActiveMQBus.RegisterBus(serviceRegister, sv =>
            //{
            //    return new ConnectionConfiguration
            //    {
            //        Url = ctx.Configuration["ActiveMQHost:Url"],
            //        Password = ctx.Configuration["ActiveMQHost:Password"],
            //        UserName = ctx.Configuration["ActiveMQHost:UserName"],
            //    };
            //}, action => { });
            //services.AddHostedService<ProducerBackgroundStartup>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        //private class ServiceProviderAdapter : IServiceResolver
        //{
        //    private readonly IServiceProvider _serviceProvider;

        //    public ServiceProviderAdapter(IServiceProvider serviceProvider)
        //    {
        //        _serviceProvider = serviceProvider;
        //    }

        //    public TService Resolve<TService>()
        //    {
        //        return _serviceProvider.GetService<TService>();
        //    }

        //    public IServiceResolverScope CreateScope()
        //    {
        //        return new MicrosoftServiceResolverScope(_serviceProvider);
        //    }
        //}

        //private class MicrosoftServiceResolverScope : IServiceResolverScope
        //{
        //    private readonly IServiceScope _serviceScope;

        //    public MicrosoftServiceResolverScope(IServiceProvider serviceProvider)
        //    {
        //        _serviceScope = serviceProvider.CreateScope();
        //    }

        //    public IServiceResolverScope CreateScope()
        //    {
        //        return new MicrosoftServiceResolverScope(_serviceScope.ServiceProvider);
        //    }

        //    public void Dispose()
        //    {
        //        _serviceScope?.Dispose();
        //    }

        //    public TService Resolve<TService>()
        //    {
        //        return _serviceScope.ServiceProvider.GetService<TService>();
        //    }
        //}

    }
}