using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace ASOFT.Core.API.Extensions
{
    /// <summary>
    /// Logger web host builder extensions
    /// </summary>
    public static class LoggerWebHostBuilderExtensions
    {
        /// <summary>
        /// Use Serilog
        /// </summary>
        /// <param name="webHostBuilder"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseASOFTSerilog([NotNull] this IWebHostBuilder webHostBuilder)
        {
            Checker.NotNull(webHostBuilder, nameof(webHostBuilder));
            ConfigureSerilogService(webHostBuilder);
            return webHostBuilder;
        }

        /// <summary>
        /// Use Serilog
        /// </summary>
        /// <param name="webHostBuilder"></param>
        /// <param name="configurationAction"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseASOFTSerilog([NotNull] this IWebHostBuilder webHostBuilder,
            [NotNull] Action<WebHostBuilderContext, LoggerConfiguration> configurationAction)
        {
            Checker.NotNull(webHostBuilder, nameof(webHostBuilder));
            Checker.NotNull(configurationAction, nameof(configurationAction));
            ConfigureSerilogService(webHostBuilder, configurationAction);
            return webHostBuilder;
        }

        private static void ConfigureSerilogService(IWebHostBuilder webHostBuilder,
            Action<WebHostBuilderContext, LoggerConfiguration> configurationAction = null)
        {
            webHostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                var loggingLevelSwitch =
                    new LoggingLevelSwitch(GetLogEventLevelOrDefault(hostingContext,
                        out IConfigurationSection section));
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
                loggerConfiguration.MinimumLevel.ControlledBy(loggingLevelSwitch);

                if (section != null)
                {
                    ChangeToken.OnChange(() => section.GetReloadToken(),
                        () =>
                        {
                            loggingLevelSwitch.MinimumLevel =
                                GetLogEventLevelOrDefault(hostingContext, out IConfigurationSection _);
                        });
                }

                configurationAction?.Invoke(hostingContext, loggerConfiguration);
            });
        }

        private static LogEventLevel GetLogEventLevelOrDefault(WebHostBuilderContext hostingContext,
            out IConfigurationSection section)
        {
            section = hostingContext.Configuration.GetSection("SeriLog:MinimumLevel");

            if (!section.Exists())
            {
                return LogEventLevel.Information;
            }

            if (Enum.TryParse(section.Value, out LogEventLevel logEventLevel)) return logEventLevel;

            var nestedSection = hostingContext.Configuration.GetSection("SeriLog:MinimumLevel:Default");

            if (!nestedSection.Exists())
            {
                return LogEventLevel.Information;
            }

            return !Enum.TryParse(nestedSection.Value, out logEventLevel) ? LogEventLevel.Information : logEventLevel;
        }
    }
}