using ASOFT.Core.Common.Localization;
using ASOFT.Core.Common.Localization.DependencyInjection;
using ASOFT.Core.Common.Localization.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ASOFT.Core.API.Extensions
{
    /// <summary>
    /// Default request culture options provider
    /// </summary>
    public class CommonRequestCultureOptionsProvider
    {
        /// <summary>
        /// Thiết lập culture theo request
        /// </summary>
        /// <returns></returns>
        public virtual Action<RequestLocalizationConfigurations> ProvideOptionsConfiguration()
        {
            return configurations =>
            {
                var vnCulture = CultureInfo.GetCultureInfo(DefaultCultureNames.ViVn);
                var supportedCultures = new List<CultureInfo>
                {
                    vnCulture,
                    CultureInfo.GetCultureInfo(DefaultCultureNames.EnUs),
                    CultureInfo.GetCultureInfo(DefaultCultureNames.JaJp),
                    CultureInfo.GetCultureInfo(DefaultCultureNames.ZhCn)
                };
                configurations.AddInitialCultureHandlers(CultureHandler);
                configurations.AddInitialUICultureHandlers(CultureHandler);
                configurations.DefaultCultureResult = new CultureResult(vnCulture, vnCulture);
                configurations.SupportedCultures = supportedCultures;
                configurations.SupportedUICultures = supportedCultures;
            };
        }

        private static ValueTask<CultureInfo> CultureHandler(HttpContext context,
            IEnumerable<CultureInfo> supportedCultures, StringSegment cultureSegment)
        {
            if (cultureSegment.StartsWith(DefaultParentCultureNames.En, StringComparison.OrdinalIgnoreCase))
            {
                return new ValueTask<CultureInfo>(supportedCultures?.FirstOrDefault(m =>
                    string.Equals(m.Name, DefaultCultureNames.EnUs, StringComparison.OrdinalIgnoreCase)));
            }

            return default;
        }
    }
}