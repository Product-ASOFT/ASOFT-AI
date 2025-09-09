using ASOFT.A00.DataAccess.Queries;
using ASOFT.Core.Business.Common.DataAccess.Contexts;
using ASOFT.Core.Business.Common.DataAccess.Helpers;
using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.DataAccess.Queries;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace ASOFT.Core.Business.Common.DataAccess
{
    public static class CommonDataAccessServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddCommonCoreDataAccessServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IModelBuilderConfiguration<BusinessDbContext>, CoreCommonModelBuilderConfiguration>();
            services.AddScoped<IFollowerQueries, FollowerQueries>();
            services.AddScoped<ICRMT90031Context, CRMT90031Context>();
            services.AddScoped<ICRMT90031_RELContext, CRMT90031_RELContext>();
            services.AddScoped<ICommentQueries, CommentQueries>();
            services.AddScoped<IVoucherQueries, VoucherQueries>();
            services.AddScoped<IPermissionQueries, PermissionQueries>();
            services.AddScoped<IUtilQueries, UtilQueries>();
            services.TryAddScoped(typeof(IHistoryQueries<>), typeof(HistoryQueries<>));


            return services;
        }
    }
}
