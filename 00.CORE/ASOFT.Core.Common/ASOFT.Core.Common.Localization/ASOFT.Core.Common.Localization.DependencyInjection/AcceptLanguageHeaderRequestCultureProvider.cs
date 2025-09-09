using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASOFT.Core.Common.Localization.DependencyInjection
{
    /// <summary>
    /// Sử dụng language header gửi lên từ client
    /// </summary>
    public class AcceptLanguageHeaderRequestCultureProvider : BaseRequestCultureProvider
    {
        /// <summary>
        /// The maximum number of values in the Accept-Language header to attempt to create a <see cref="System.Globalization.CultureInfo"/>
        /// from for the current request.
        /// Defaults to <c>3</c>.
        /// </summary>
        public int MaximumAcceptLanguageHeaderValuesToTry { get; set; } = 3;

        public override ValueTask<IRequestCulture> ProvideRequestCultureAsync(HttpContext input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var acceptLanguageHeader = input.Request.GetTypedHeaders().AcceptLanguage;

            if (acceptLanguageHeader == null || acceptLanguageHeader.Count == 0)
            {
                return NullRequestCulture;
            }

            var languages = acceptLanguageHeader.AsEnumerable();

            if (MaximumAcceptLanguageHeaderValuesToTry > 0)
            {
                languages = languages.Take(MaximumAcceptLanguageHeaderValuesToTry);
            }

            var orderedLanguages = languages
                .OrderByDescending(h => h, StringWithQualityHeaderValueComparer.QualityComparer)
                .Select(x => x.Value).ToList();

            return orderedLanguages.Count > 0
                ? new ValueTask<IRequestCulture>(new RequestCulture(orderedLanguages))
                : NullRequestCulture;
        }
    }
}