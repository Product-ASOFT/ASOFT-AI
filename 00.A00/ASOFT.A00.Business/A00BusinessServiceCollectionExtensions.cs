// #################################################################
// # Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	Updated		    Content                
// #    09/07/2021  Văn Tài			Coding - Bổ sung AT1302Business, PeriodBusiness.
// #    13/09/2023  Thành Sang		Coding - Bổ sung IVNPTEInvoiceBusiness, VNPTEInvoiceBusiness
// #    27/05/2024  Minh Nhựt       Coding - Bổ sung CalendarBusiness
// #    12/08/2024  Văn Sơn         Coding - Bổ sung ShopeeAPIBusiness
// ##################################################################

using ASOFT.A00.Business;
using ASOFT.A00.Business.Interfaces;
using ASOFT.A00.DataAccess.Extensions;
using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.DataAccess.Queries;
using ASOFT.Core.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ASOFT.A00.Application.Extensions
{
    public static class A00BusinessServiceCollectionExtensions
    {
        public static IServiceCollection AddA00ApplicationServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddA00InfrastructureServices();
            return services;
        }
    }
}