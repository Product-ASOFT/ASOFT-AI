//############################################################
//# Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved. 
//#
//# History:
//# 	Date Time 	Updated 	Comment
//# 	2020/04/20  Cường Thịnh Updated
//############################################################
using ASOFT.Core.API.Versions;
using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace ASOFT.Core.API.Extensions
{
    /// <summary>
    /// Common services
    /// </summary>
    public static class CommonServiceCollectionExtensions
    {
        /// <summary>
        /// Add common services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggers([NotNull] this IServiceCollection services,
            IConfiguration configuration)
        {
            Checker.NotNull(services, nameof(services));
            ConfigureSwagger(services);
            return services;
        }

        private static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"{SupportApiVersions.CurrentVersionStr}", new OpenApiInfo
                {
                    Title = "API Documents",
                    Version = SupportApiVersions.CurrentVersionStr,
                    License = new OpenApiLicense
                    {
                        Name = "ASOFT® CORPORATION",
                        Url = new Uri("https://www.asoft.com.vn")
                    }
                });
                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });
                c.DocInclusionPredicate((name, api) => true);
                c.CustomSchemaIds(x => x.FullName);
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
         
        }
    }
}