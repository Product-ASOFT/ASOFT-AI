// #################################################################
// # Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	Updated		    Content                
// #    09/07/2021  Văn Tài			Coding - Bổ sung AT1302Queries, PeriodQueries.
// #    13/09/2023  Thành Sang		Coding - Bổ sung IVNPTEInvoiceQueries, VNPTEInvoiceQueries
// #    27/05/2024  Minh Nhựt		Coding - Bổ sung CalendarQueries
// #    15/08/2024  Văn Sơn		    Coding - Bổ sung CIT0219Queries
// #    15/08/2024  Văn Sơn		    Coding - Bổ sung OT1302Queries
// ##################################################################

using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.DataAccess.Queries;
using ASOFT.A00.DataAccess.Utilities;
using ASOFT.A00.Entities;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ASOFT.A00.DataAccess.Extensions
{
    /// <summary>
    /// Các phương thức extension cho <see cref="IServiceCollection"/>.
    /// </summary>
    public static class A00DataAccessCollectionExtensions
    {
        /// <summary>
        /// Thêm các dịch vụ của A00 Infrastructure vào <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddA00InfrastructureServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            services.AddTransient<IModelBuilderConfiguration<BusinessDbContext>, ModuleA00ModelBuilderConfiguration>();
            services.AddScoped<IOAuthQueries, OAuthQueries>();
            services.AddScoped<IASOFTCommonQueries, ASOFTCommonQueries>();
            services.AddScoped<IST2101Queries, ST2101Queries>();
            return services;
        }
    }
}