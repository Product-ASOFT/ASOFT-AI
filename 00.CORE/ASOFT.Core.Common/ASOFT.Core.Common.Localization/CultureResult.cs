using System;
using System.Globalization;

namespace ASOFT.Core.Common.Localization
{
    /// <summary>
    /// Kết quả culture trả về
    /// </summary>
    public class CultureResult : ICultureResult
    {
        public CultureInfo Culture { get; }
        public CultureInfo UICulture { get; }

        public CultureResult(CultureInfo cultureInfo) : this(cultureInfo, cultureInfo)
        {
        }

        public CultureResult(string culture) : this(culture, culture)
        {
        }

        public CultureResult(string culture, string uiCulture)
            : this(new CultureInfo(culture), new CultureInfo(uiCulture))
        {
        }

        public CultureResult(CultureInfo culture, CultureInfo uiCulture)
        {
            Culture = culture ?? throw new ArgumentNullException(nameof(culture));
            UICulture = uiCulture ?? throw new ArgumentNullException(nameof(uiCulture));
        }
    }
}