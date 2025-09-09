using ASOFT.A00.Business;
using ASOFT.A00.Business.Interfaces;
using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.DataAccess.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Linq;

namespace ASOFT.A00.API.BaseQuartz
{
    public class CallQuartz
    {
        public static void UseQuartz(IServiceCollection services,params Type[] jobs)
        {
            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            services.Add(jobs.Select(jobType => new ServiceDescriptor(jobType, jobType, ServiceLifetime.Singleton)));

            services.AddSingleton(provider =>
            {
                var sFactory = new StdSchedulerFactory();
                var scheduler = sFactory.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                scheduler.Start();
                return scheduler;
            });
        }
    }
}
