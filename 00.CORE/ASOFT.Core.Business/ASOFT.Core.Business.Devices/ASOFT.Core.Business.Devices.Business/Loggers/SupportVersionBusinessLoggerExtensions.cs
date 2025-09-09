using System;
using ASOFT.Core.Common.InjectionChecker;
using Microsoft.Extensions.Logging;

namespace ASOFT.Core.Business.Devices.Business.Loggers
{
    public static class SupportVersionBusinessLoggerExtensions
    {
        private static readonly Action<ILogger, string, string, Exception> InfoGettingSupportVersionLogger =
            LoggerMessage.Define<string, string>(LogLevel.Information, new EventId(0),
                "Getting support versions with clientID: '{ClientID}' and platform: '{Platform}'.");

        private static readonly Action<ILogger, string, string, Exception> WarningSupportVersionNotFoundLogger =
            LoggerMessage.Define<string, string>(LogLevel.Warning, new EventId(0),
                "Cannot find version with clientID: '{ClientID}' and platform: '{Platform}'.");

        /// <summary>
        /// Log message when version is not found
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="clientID">The id of client.</param>
        /// <param name="platform">The platform of client.</param>
        public static void WarningSupportVersionNotFound(this ILogger logger, string clientID,
            string platform)
        {
            Checker.NotNull(logger, nameof(logger));
            WarningSupportVersionNotFoundLogger(logger, clientID, platform, null);
        }

        /// <summary>
        /// Log info message when getting support version.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="clientID"></param>
        /// <param name="platform"></param>
        public static void InfoGettingSupportVersion(this ILogger logger, string clientID, string platform)
        {
            Checker.NotNull(logger, nameof(logger));
            InfoGettingSupportVersionLogger(logger, clientID, platform, null);
        }
    }
}