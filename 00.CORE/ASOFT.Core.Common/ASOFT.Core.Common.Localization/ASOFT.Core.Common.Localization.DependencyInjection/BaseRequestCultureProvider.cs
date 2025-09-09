using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ASOFT.Core.Common.Localization.DependencyInjection
{
    /// <summary>
    /// Base class cho
    /// </summary>
    public abstract class BaseRequestCultureProvider : IRequestCultureProvider<HttpContext>
    {
        protected static readonly ValueTask<IRequestCulture> NullRequestCulture = default;

        public abstract ValueTask<IRequestCulture> ProvideRequestCultureAsync(HttpContext input);
    }
}