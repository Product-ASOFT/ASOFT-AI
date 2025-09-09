using Microsoft.AspNetCore.Http;

namespace ASOFT.Core.Common.Localization.DependencyInjection
{
    public interface IRequestCultureTransformer : ICultureTransformer<HttpContext>
    {

    }
}