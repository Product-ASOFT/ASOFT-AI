using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace ASOFT.Core.Common.Localization
{
    /// <summary>
    /// Lớp request culture mặc định
    /// </summary>
    public class RequestCulture : IRequestCulture
    {
        public IList<StringSegment> Cultures { get; }
        public IList<StringSegment> UICultures { get; }

        public RequestCulture(StringSegment culture)
            : this(new List<StringSegment> { culture }, new List<StringSegment> { culture })
        {

        }

        public RequestCulture(StringSegment culture, StringSegment uiCulture)
            : this(new List<StringSegment> { culture }, new List<StringSegment> { uiCulture })
        {

        }

        public RequestCulture(IList<StringSegment> cultures)
            : this(cultures, cultures)
        {

        }

        public RequestCulture(IList<StringSegment> cultures, IList<StringSegment> uiCultures)
        {
            Cultures = cultures ?? throw new ArgumentNullException(nameof(cultures));
            UICultures = uiCultures ?? throw new ArgumentNullException(nameof(uiCultures));
        }
    }
}