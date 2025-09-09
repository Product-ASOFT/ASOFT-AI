// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.Business.Files.DataAccess.Interfaces;
using ASOFT.Core.Business.Files.DataAccess.Queries;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ASOFT.Core.Business.Files.DataAccess.Extensions
{
    public static class FileManagementInfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// Sử dụng InfrastructureServices của FileManagement
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFileManagementInfrastructureServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddScoped<IFileQueries, FileQueries>();
            services.AddTransient<IModelBuilderConfiguration<BusinessDbContext>, FileManagementModelBuilderConfiguration>();
            return services;
        }
    }
}
