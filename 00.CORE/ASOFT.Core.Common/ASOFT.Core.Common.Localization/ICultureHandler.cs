using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ASOFT.Core.Common.Localization
{
    /// <summary>
    /// Xử lý culture
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface ICultureHandler<in TInput>
    {
        ValueTask<CultureInfo> HandleAsync(TInput input, IEnumerable<CultureInfo> supportedCultures, StringSegment cultureSegment);
    }
}