using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ASOFT.Core.Common.Localization.DependencyInjection
{
    public class QueryStringRequestCultureProvider : BaseRequestCultureProvider
    {
        public string QueryStringKey { get; set; } = "culture";
        public string UIQueryStringKey { get; set; } = "ui-culture";

        public override ValueTask<IRequestCulture> ProvideRequestCultureAsync(HttpContext input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var request = input.Request;
            if (!request.QueryString.HasValue)
            {
                return NullRequestCulture;
            }

            string queryCulture = null;
            string queryUICulture = null;

            if (!string.IsNullOrWhiteSpace(QueryStringKey))
            {
                queryCulture = request.Query[QueryStringKey];
            }

            if (!string.IsNullOrWhiteSpace(UIQueryStringKey))
            {
                queryUICulture = request.Query[UIQueryStringKey];
            }

            if (queryCulture == null && queryUICulture == null)
            {
                // No values specified for either so no match
                return NullRequestCulture;
            }

            if (queryCulture != null && queryUICulture == null)
            {
                // Value for culture but not for UI culture so default to culture value for both
                queryUICulture = queryCulture;
            }

            if (queryCulture == null && queryUICulture != null)
            {
                // Value for UI culture but not for culture so default to UI culture value for both
                queryCulture = queryUICulture;
            }

            return new ValueTask<IRequestCulture>(new RequestCulture(queryCulture, queryUICulture));
        }
    }
}