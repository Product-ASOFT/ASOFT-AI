using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASOFT.Core.Common.Localization.DependencyInjection
{
    public class RequestCultureTransformer : CultureTransformer<HttpContext>, IRequestCultureTransformer
    {
        public RequestCultureTransformer(IOptions<RequestLocalizationConfigurations> options,
            ILoggerFactory loggerFactory) : base(options.Value, loggerFactory)
        {
        }
    }
}