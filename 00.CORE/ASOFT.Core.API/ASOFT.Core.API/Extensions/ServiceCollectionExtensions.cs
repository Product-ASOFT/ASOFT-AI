using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

namespace ASOFT.Core.API.Extensions
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Xóa service ra khỏi <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static IServiceCollection RemoveService<TService>([NotNull] this IServiceCollection services,
            ServiceLifetime serviceLifetime)
            where TService : class
        {
            Checker.NotNull(services, nameof(services));
            var registeredServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(TService));
            if (registeredServiceDescriptor != null)
            {
                services.Remove(registeredServiceDescriptor);
            }

            return services;
        }

        /// <summary>
        /// Thay instance của service bằng 1 service khác.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TReplaceImplementation"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static IServiceCollection ReplaceService<TService, TReplaceImplementation>(
            [NotNull] this IServiceCollection services, ServiceLifetime serviceLifetime)
            where TService : class
            where TReplaceImplementation : class, TService
        {
            Checker.NotNull(services, nameof(services));
            return services.Replace(ServiceDescriptor.Describe(typeof(TService), typeof(TReplaceImplementation),
                serviceLifetime));
        }

        /// <summary>
        /// Thay instance của singleton service bằng một service khác.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TReplaceImplementation"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ReplaceSingleton<TService, TReplaceImplementation>(
            [NotNull] this IServiceCollection services)
            where TService : class
            where TReplaceImplementation : class, TService
        {
            Checker.NotNull(services, nameof(services));
            return services.ReplaceService<TService, TReplaceImplementation>(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Thay instance của scoped service bằng một service khác.
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TReplaceImplementation"></typeparam>
        /// <returns></returns>
        public static IServiceCollection ReplaceScoped<TService, TReplaceImplementation>(
            [NotNull] this IServiceCollection services)
            where TService : class
            where TReplaceImplementation : class, TService
        {
            Checker.NotNull(services, nameof(services));
            return services.ReplaceService<TService, TReplaceImplementation>(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Thay instance của transient service bằng một server khác.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TReplaceImplementation"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ReplaceTransient<TService, TReplaceImplementation>(
            [NotNull] this IServiceCollection services)
            where TService : class
            where TReplaceImplementation : class, TService
        {
            Checker.NotNull(services, nameof(services));
            return services.ReplaceService<TService, TReplaceImplementation>(ServiceLifetime.Transient);
        }
    }
}