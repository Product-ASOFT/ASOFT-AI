using System.Globalization;

namespace ASOFT.Core.Common.Localization
{
    /// <summary>
    /// Kết quả tra ra từ <see cref="ICultureTransformer{TInput}"/>
    /// </summary>
    public interface ICultureResult
    {
        CultureInfo Culture { get; }
        CultureInfo UICulture { get; }
    }
}