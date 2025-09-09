using ASOFT.Core.DataAccess;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace ASOFT.Core.DataAccess.Extensions
{
    /// <summary>
    /// The extension class for adding database connection to services <see cref="IServiceCollection"/>.
    /// </summary>
    public static class DatabaseServiceCollectionExtensions
    {
        /// <summary>
        /// Add database connection to services <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection <see cref="ServiceCollection"/> to add database connection.</param>
        /// <param name="configuration"></param>
        /// <param name="serviceLifetime">Loại vòng đời của service.</param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultDbConnectionProvider([NotNull] this IServiceCollection services,
            IConfiguration configuration,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            AddDbConnectionProvider(services, configuration.GetSection("DbConnectionStrings"), serviceLifetime);
            return services;
        }

        /// <summary>
        /// Add database connection to services <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection <see cref="ServiceCollection"/> to add database connection.</param>
        /// <param name="dbConnectionAccessor"></param>
        /// <param name="serviceLifetime">Loại vòng đời của service.</param>
        /// <returns></returns>
        public static IServiceCollection AddDbConnectionProvider([NotNull] this IServiceCollection services,
            [NotNull] Func<IServiceProvider, DbConnectionStrings> dbConnectionAccessor,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (dbConnectionAccessor == null)
            {
                throw new ArgumentNullException(nameof(dbConnectionAccessor));
            }

            switch (serviceLifetime)
            {
                case ServiceLifetime.Scoped:
                    services.TryAddScoped(sp => CreateDbConnectionProvider(sp, dbConnectionAccessor));
                    break;
                case ServiceLifetime.Singleton:
                    services.TryAddSingleton(sp => CreateDbConnectionProvider(sp, dbConnectionAccessor));
                    break;
                case ServiceLifetime.Transient:
                    services.TryAddTransient(sp => CreateDbConnectionProvider(sp, dbConnectionAccessor));
                    break;
                default:
                    return services;
            }

            return services;
        }

        /// <summary>
        /// Add database connection to services <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection <see cref="ServiceCollection"/> to add database connection.</param>
        /// <param name="settings"></param>
        /// <param name="serviceLifetime">Loại vòng đời của service.</param>
        /// <returns></returns>
        public static IServiceCollection AddDbConnectionProvider([NotNull] this IServiceCollection services,
            [NotNull] Action<DbConnectionStrings> settings,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            services.Configure(settings);
            AddServiceConnections(services, serviceLifetime);
            return services;
        }

        /// <summary>
        /// Add database connection to services <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection <see cref="ServiceCollection"/> to add database connection.</param>
        /// <param name="configuration">The configuration <see cref="IConfiguration"/> for binding to DbConnectionSettings</param>
        /// <param name="serviceLifetime">Loại vòng đời của service.</param>
        /// <returns></returns>
        public static IServiceCollection AddDbConnectionProvider([NotNull] this IServiceCollection services,
            [NotNull] IConfiguration configuration,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.Configure<DbConnectionStrings>(configuration);
            AddServiceConnections(services, serviceLifetime);
            return services;
        }

        private static void AddServiceConnections(IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Scoped:
                    services.TryAddScoped(CreateScopeDbConnectionProvider);
                    break;
                case ServiceLifetime.Singleton:
                    services.TryAddSingleton(CreateTransientDbConnectionProvider);
                    break;
                case ServiceLifetime.Transient:
                    services.TryAddTransient(CreateTransientDbConnectionProvider);
                    break;
                default:
                    return;
            }
        }

        private static IDbConnectionProvider CreateScopeDbConnectionProvider(IServiceProvider serviceProvider)
            => CreateDbConnectionProvider(serviceProvider, sp =>
                sp.GetRequiredService<IOptionsSnapshot<DbConnectionStrings>>().Value);

        private static IDbConnectionProvider CreateTransientDbConnectionProvider(IServiceProvider serviceProvider)
            => CreateDbConnectionProvider(serviceProvider,
                sp => sp.GetRequiredService<IOptionsMonitor<DbConnectionStrings>>().CurrentValue);

        private static IDbConnectionProvider CreateDbConnectionProvider(
            IServiceProvider serviceProvider,
            Func<IServiceProvider, DbConnectionStrings> connectionAccessor)
        {
            var dbConnectionStrings = connectionAccessor(serviceProvider);
            return new DbConnectionProvider(dbConnectionStrings.ConnectionStrings);
        }
    }
}