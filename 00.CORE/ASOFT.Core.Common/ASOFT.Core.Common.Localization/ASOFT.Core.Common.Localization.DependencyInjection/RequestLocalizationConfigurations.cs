using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace ASOFT.Core.Common.Localization.DependencyInjection
{
    public class RequestLocalizationConfigurations : ILocalizationConfigurations<HttpContext>
    {
        private ICultureResult _defaultCultureResult =
            new CultureResult(CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);

        private IList<CultureInfo> _supportedCultures = new List<CultureInfo>
        {
            CultureInfo.CurrentCulture
        };

        private IList<CultureInfo> _supportedUICultures = new List<CultureInfo>
        {
            CultureInfo.CurrentUICulture
        };

        /// <summary>
        /// Specify configuration for <see cref="RequestCultureTransformer" />
        /// </summary>
        /// <returns></returns>
        public ICultureResult DefaultCultureResult
        {
            get => _defaultCultureResult;
            set => _defaultCultureResult = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool FallbackToParentCultures { get; set; } = true;

        public bool FallbackToParentUICultures { get; set; } = true;

        public IList<CultureInfo> SupportedCultures
        {
            get => _supportedCultures;
            set => _supportedCultures = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IList<CultureInfo> SupportedUICultures
        {
            get => _supportedUICultures;
            set => _supportedUICultures = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IList<ICultureHandler<HttpContext>> CultureHandlers { get; }
            = new List<ICultureHandler<HttpContext>>
            {
                new DefaultCultureHandler()
            };

        public IList<ICultureHandler<HttpContext>> UICultureHandlers { get; } =
            new List<ICultureHandler<HttpContext>>();

        public IList<IRequestCultureProvider<HttpContext>> CultureProviders { get; }
            = new List<IRequestCultureProvider<HttpContext>>
            {
                new AcceptLanguageHeaderRequestCultureProvider(),
                new QueryStringRequestCultureProvider()
            };
    }
}