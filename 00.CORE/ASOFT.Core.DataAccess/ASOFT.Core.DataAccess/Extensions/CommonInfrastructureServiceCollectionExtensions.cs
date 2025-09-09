using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using ASOFT.Core.DataAccess;
using ASOFT.Data.Core.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace ASOFT.Core.DataAccess.Extensions
{
    /// <summary>
    /// Extension cho dịch vụ chung
    /// </summary>
    public static class CommonInfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// Thiết lập các dịch vụ chung
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommonInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            Checker.NotNull(services, nameof(services));
            Checker.NotNull(configuration, nameof(configuration));

            AddCommonDbServices(services, configuration);
            AddCommonServices(services);
            return services;
        }

        /// <summary>
        /// Add <see cref="BusinessDbContext"></see> services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void AddCommonDbServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultDbConnectionProvider(configuration);

            // Model configuration provider for provide configuration database model
            services.TryAddTransient<IModelBuilderConfigurationProvider<BusinessDbContext>,
                ModelBuilderConfigurationProvider<BusinessDbContext>>();

            services.AddDbContext<BusinessDbContext>((serviceProvider, options) =>
            {
                // Get connection from db connection provider
                var connectionProvider = serviceProvider.GetRequiredService<IDbConnectionProvider>();
                options.EnableDetailedErrors();

                // Custom SQL Server for ASOFT
                options.UseASOFTSqlServer(connectionProvider.ProvideConnectionString(CommonConnectionKeys.Business));

                // Use logging
                options.UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());

                // Use configuration for model
                options.UseModelBuilderConfiguration(
                    serviceProvider.GetRequiredService<IModelBuilderConfigurationProvider<BusinessDbContext>>());
            });

            // Model configuration provider for provide configuration database model
            services.TryAddTransient<IModelBuilderConfigurationProvider<AdminDbContext>,
                ModelBuilderConfigurationProvider<AdminDbContext>>();

            services.AddDbContext<AdminDbContext>((serviceProvider, options) =>
            {
                // Get connection from db connection provider
                var connectionProvider = serviceProvider.GetRequiredService<IDbConnectionProvider>();
                options.EnableDetailedErrors();

                // Custom SQL Server for ASOFT
                options.UseASOFTSqlServer(connectionProvider.ProvideConnectionString(CommonConnectionKeys.Admin));

                // Use logging
                options.UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());

                // Use configuration for model
                options.UseModelBuilderConfiguration(
                    serviceProvider.GetRequiredService<IModelBuilderConfigurationProvider<AdminDbContext>>());
            });
        }

        private static void AddCommonServices(IServiceCollection services)
        {
            services.TryAddScoped<IMessageContext, MessageContext>();
            services.TryAddScoped<ILanguageContext, LanguageContext>();
            services.TryAddScoped<IBusinessUnitOfWork>(sp => sp.GetRequiredService<BusinessDbContext>());
            services.TryAddScoped(typeof(IBusinessContext<>), typeof(BusinessContext<>));
            services.TryAddScoped<IConfigQueries, ConfigQueries>();
            services.TryAddScoped<IAdminUnitOfWork>(sp => sp.GetRequiredService<AdminDbContext>());
            services.TryAddScoped(typeof(IAdminContext<>), typeof(AdminContext<>));
        }
    }
}