using ASOFT.Core.Business.Common.Business.EventListeners;
using ASOFT.Core.Business.Common.Business.Interfaces;
using ASOFT.Core.Business.Common.DataAccess;
using ASOFT.Core.Business.Common.Entities.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace ASOFT.Core.Business.Common.Business
{
    public static class CommonBusinessServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreCommonApplicationServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IVoucherBusiness, VoucherBusiness>();
            services.TryAddScoped(typeof(ICommonExcute<,>), typeof(CommonExcute<,>));
            services.TryAddScoped(typeof(IHistoryBusiness<>), typeof(HistoryBusiness<>));
            services.TryAddScoped(typeof(ICommonExcuteWithDetail<,,>), typeof(CommonExcuteWithDetail<,,>));
            services.AddSingleton<ICacheManagerBusiness, CacheManagerBusiness>();

            MediatR.Registration.ServiceRegistrar.AddMediatRClasses(services,
                new[] { typeof(FollowerListener).Assembly });
            services.AddCommonCoreDataAccessServices();
            return services;
        }
    }
}
