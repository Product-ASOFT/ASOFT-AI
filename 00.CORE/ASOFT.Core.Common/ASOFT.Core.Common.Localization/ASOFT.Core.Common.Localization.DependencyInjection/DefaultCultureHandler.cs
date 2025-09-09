using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ASOFT.Core.Common.Localization.DependencyInjection
{
    /// <summary>
    /// Lớp mặc định cho xử lý culture <see cref="RequestLocalizationConfigurations"/>.
    /// </summary>
    public class DefaultCultureHandler : ICultureHandler<HttpContext>
    {
        public ValueTask<CultureInfo> HandleAsync(HttpContext input, IEnumerable<CultureInfo> supportedCultures,
            StringSegment cultureSegment)
            => new ValueTask<CultureInfo>(supportedCultures?.FirstOrDefault(m =>
                StringSegment.Equals(m.Name, cultureSegment, StringComparison.OrdinalIgnoreCase)));
    }
}