using Microsoft.Extensions.Logging;
using System;

namespace ASOFT.Core.API.Logging
{
    internal static class InternalLoggerBehaviorExtensions
    {
        private static readonly Action<ILogger, string, Exception> _requestCommandLogger =
            LoggerMessage.Define<string>(LogLevel.Information, 0,
                "---- Handling command with request type {RequestType}.");

        private static readonly Action<ILogger, string, Exception> _responseCommandLogger =
            LoggerMessage.Define<string>(LogLevel.Information, 1,
                "---- Handled command with response type {ResponseType}.");

        private static readonly Action<ILogger, string, Exception> _responseCommandIsNullLogger =
            LoggerMessage.Define<string>(LogLevel.Warning, 2,
                "---- Command response type {ResponseType} value is null.");

        public static void LogRequestCommand(this ILogger logger, string requestTypeName)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _requestCommandLogger(logger, requestTypeName, null);
        }

        public static void LogResponseCommand(this ILogger logger, string responseTypeName)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _responseCommandLogger(logger, responseTypeName, null);
        }

        public static void LogResponseCommandIsNull(this ILogger logger, string responseTypeName)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _responseCommandIsNullLogger(logger, responseTypeName, null);
        }
    }
}