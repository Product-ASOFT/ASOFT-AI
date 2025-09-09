using System.Collections.Generic;
using System.Globalization;

namespace ASOFT.Core.Common.Localization
{
    /// <summary>
    /// Thiếp lập cho xử lý request cultures
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface ILocalizationConfigurations<TInput>
    {
        /// <summary>
        /// Culture mặc định.
        /// </summary>
        ICultureResult DefaultCultureResult { get; }

        /// <summary>
        /// Có fallback parent culture hay không.
        /// Ví dụ: Khi set <see cref="FallbackToParentCultures"/> là <code>true</code>
        /// Nếu client request gửi lên server có culture là 'en-GB' mà server không
        /// hỗ trợ thì sẽ tự động fallback về culture 'en' là cha của culture 'en-GB'.
        /// </summary>
        bool FallbackToParentCultures { get; }

        /// <summary>
        /// Có fallback parent ui culture hay không.
        /// Ví dụ: Khi set <see cref="FallbackToParentUICultures"/> là <code>true</code>
        /// Nếu client request gửi lên server có ui culture là 'en-GB' mà server không
        /// hỗ trợ thì sẽ tự động fallback về culture 'en' là cha của culture 'en-GB'.
        /// </summary>
        bool FallbackToParentUICultures { get; }

        /// <summary>
        /// Danh sách culture hỗ trợ
        /// </summary>
        IList<CultureInfo> SupportedCultures { get; }

        /// <summary>
        /// Danh sách ui culture hỗ trợ
        /// </summary>
        IList<CultureInfo> SupportedUICultures { get; }

        /// <summary>
        /// Handle culture
        /// </summary>
        IList<ICultureHandler<TInput>> CultureHandlers { get; }

        /// <summary>
        /// Handle ui culture
        /// </summary>
        IList<ICultureHandler<TInput>> UICultureHandlers { get; }

        /// <summary>
        /// Danh sách culture provier.
        /// </summary>
        IList<IRequestCultureProvider<TInput>> CultureProviders { get; }
    }
}