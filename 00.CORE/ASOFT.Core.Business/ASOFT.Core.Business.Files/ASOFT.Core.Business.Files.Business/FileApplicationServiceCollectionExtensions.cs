// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.Business.Files.Business.Interfaces;
using ASOFT.Core.Business.Files.DataAccess.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ASOFT.Core.Business.Files.Business.Extensions
{
    public static class FileApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// add application service cho FileManagement
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFileManagementApplicationServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddScoped<IFileBusiness, FilesBusiness>();

            services.AddFileManagementInfrastructureServices();
 
            return services;
        }
    }
}
