using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace ASOFT.Core.Common.Localization.Internal
{
    internal static class LocalizeLoggerExtensions
    {
        private static readonly Action<ILogger, string, IList<StringSegment>, Exception> _unsupportedCulture;
        private static readonly Action<ILogger, string, IList<StringSegment>, Exception> _unsupportedUICulture;
        private static readonly Action<ILogger, Exception> _cultureProvidersEmpty;

        static LocalizeLoggerExtensions()
        {
            _unsupportedCulture = LoggerMessage.Define<string, IList<StringSegment>>(
                LogLevel.Warning,
                1,
                "{requestCultureProvider} returned the following unsupported cultures '{cultures}'.");
            _unsupportedUICulture = LoggerMessage.Define<string, IList<StringSegment>>(
                LogLevel.Warning,
                2,
                "{requestCultureProvider} returned the following unsupported UI Cultures '{uiCultures}'.");
            _cultureProvidersEmpty = LoggerMessage.Define(LogLevel.Warning,
                3, "Culture providers are empty.");
        }

        public static void UnsupportedCultures(this ILogger logger, string requestCultureProvider, IList<StringSegment> cultures)
        {
            _unsupportedCulture(logger, requestCultureProvider, cultures, null);
        }

        public static void UnsupportedUICultures(this ILogger logger, string requestCultureProvider, IList<StringSegment> uiCultures)
        {
            _unsupportedUICulture(logger, requestCultureProvider, uiCultures, null);
        }

        public static void CultureProvidersEmpty(this ILogger logger)
        {
            _cultureProvidersEmpty(logger, null);
        }
    }
}