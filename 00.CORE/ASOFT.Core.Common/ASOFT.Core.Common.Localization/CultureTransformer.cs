using ASOFT.Core.Common.Localization.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ASOFT.Core.Common.Localization
{
    public class CultureTransformer<TInput> : ICultureTransformer<TInput>
    {
        private static readonly int FallbackToParentMaxDepth = 5;

        /// <summary>
        /// Thiết lập localization
        /// </summary>
        private readonly ILocalizationConfigurations<TInput> _localizationConfigurations;

        private readonly ILoggerFactory _loggerFactory;
        private ILogger _logger;

        public CultureTransformer(ILocalizationConfigurations<TInput> localizationConfigurations,
            ILoggerFactory loggerFactory)
        {
            _localizationConfigurations = localizationConfigurations ??
                                          throw new ArgumentNullException(nameof(localizationConfigurations));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public virtual async ValueTask<ICultureResult> TransformAsync(TInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var cultureProviders = _localizationConfigurations.CultureProviders;

            if (cultureProviders == null || cultureProviders.Count == 0)
            {
                GetLogger().CultureProvidersEmpty();
                return null;
            }

            foreach (var cultureProvider in cultureProviders)
            {
                // Lấy thông tin yêu cầu culture
                var requestCulture = await cultureProvider.ProvideRequestCultureAsync(input).ConfigureAwait(false);

                if (requestCulture == null)
                {
                    continue;
                }

                var cultures = requestCulture.Cultures;
                var uiCultures = requestCulture.UICultures;

                CultureInfo cultureInfo = null;
                CultureInfo uiCultureInfo = null;

                if (_localizationConfigurations.SupportedCultures != null)
                {
                    cultureInfo = await GetCultureInfoAsync(cultures,
                        _localizationConfigurations.SupportedCultures,
                        _localizationConfigurations.CultureHandlers,
                        input,
                        _localizationConfigurations.FallbackToParentCultures).ConfigureAwait(false);

                    if (cultureInfo == null)
                    {
                        GetLogger().UnsupportedCultures(cultureProvider.GetType().Name, cultures);
                    }
                }

                if (_localizationConfigurations.SupportedUICultures != null)
                {
                    uiCultureInfo = await GetCultureInfoAsync(uiCultures,
                        _localizationConfigurations.SupportedUICultures,
                        _localizationConfigurations.UICultureHandlers,
                        input,
                        _localizationConfigurations.FallbackToParentUICultures).ConfigureAwait(false);

                    if (uiCultureInfo == null)
                    {
                        GetLogger().UnsupportedUICultures(cultureProvider.GetType().Name, uiCultures);
                    }
                }

                if (cultureInfo == null && uiCultureInfo == null)
                {
                    continue;
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (cultureInfo == null && uiCultureInfo != null)
                {
                    cultureInfo = _localizationConfigurations.DefaultCultureResult.Culture;
                }

                if (cultureInfo != null && uiCultureInfo == null)
                {
                    uiCultureInfo = _localizationConfigurations.DefaultCultureResult.UICulture;
                }

                return new CultureResult(cultureInfo, uiCultureInfo);
            }

            return _localizationConfigurations.DefaultCultureResult;
        }

        private ILogger GetLogger() => _logger ?? (_logger = _loggerFactory.CreateLogger(GetType()));

        private static async ValueTask<CultureInfo> GetCultureInfoAsync(
            IEnumerable<StringSegment> cultureNames,
            IList<CultureInfo> supportedCultures,
            IList<ICultureHandler<TInput>> cultureHandlers,
            TInput input,
            bool fallbackToParentCultures)
        {
            foreach (var cultureName in cultureNames)
            {
                // Allow empty string values as they map to InvariantCulture, whereas null culture values will throw in
                // the CultureInfo ctor
                if (cultureName == null) continue;
                var cultureInfo = await GetCultureInfoAsync(cultureName,
                    supportedCultures,
                    cultureHandlers,
                    input,
                    fallbackToParentCultures,
                    0,
                    FallbackToParentMaxDepth).ConfigureAwait(false);

                if (cultureInfo != null)
                {
                    return cultureInfo;
                }
            }

            return null;
        }

        private static async ValueTask<CultureInfo> GetCultureInfoAsync(StringSegment name,
            IList<CultureInfo> supportedCultures,
            IList<ICultureHandler<TInput>> cultureHandlers,
            TInput input)
        {
            // Allow only known culture names as this API is called with input from users (HTTP requests) and
            // creating CultureInfo objects is expensive and we don't want it to throw either.
            if (name == null)
            {
                return null;
            }

            if (cultureHandlers?.Any() == true)
            {
                foreach (var cultureHandler in cultureHandlers)
                {
                    var culture = await cultureHandler.HandleAsync(input,
                        supportedCultures ?? Enumerable.Empty<CultureInfo>(),
                        name).ConfigureAwait(false);

                    if (culture != null)
                    {
                        return CultureInfo.ReadOnly(culture);
                    }
                }
            }

            return null;
        }

        private static async ValueTask<CultureInfo> GetCultureInfoAsync(
            StringSegment cultureName,
            IList<CultureInfo> supportedCultures,
            IList<ICultureHandler<TInput>> cultureHandlers,
            TInput input,
            bool fallbackToParentCultures,
            int currentDepth,
            int maxCultureFallbackDepth)
        {
            var culture = await GetCultureInfoAsync(cultureName,
                supportedCultures,
                cultureHandlers,
                input).ConfigureAwait(false);

            if (culture == null && fallbackToParentCultures && currentDepth < maxCultureFallbackDepth)
            {
                var lastIndexOfHyphen = cultureName.LastIndexOf('-');

                if (lastIndexOfHyphen > 0)
                {
                    // Trim the trailing section from the culture name, e.g. "fr-FR" becomes "fr"
                    var parentCultureName = cultureName.Subsegment(0, lastIndexOfHyphen);

                    culture = await GetCultureInfoAsync(parentCultureName,
                        supportedCultures,
                        cultureHandlers,
                        input,
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        fallbackToParentCultures,
                        currentDepth + 1,
                        maxCultureFallbackDepth).ConfigureAwait(false);
                }
            }

            return culture;
        }
    }
}