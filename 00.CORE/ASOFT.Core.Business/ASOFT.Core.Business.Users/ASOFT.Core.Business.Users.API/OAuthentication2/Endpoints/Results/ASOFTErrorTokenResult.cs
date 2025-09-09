using ASOFT.API.Core.ApiResponse;
using ASOFT.API.Core.Errors;
using ASOFT.Contract;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ASOFT.Authentication.API.OAuthentication2.Endpoints.Results
{
    public class ASOFTErrorTokenResult : IEndpointResult
    {
        private readonly ErrorModel _errorModel;

        public ASOFTErrorTokenResult(ErrorModel errorModel)
        {
            _errorModel = Checker.NotNull(errorModel, nameof(errorModel));
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.SetNoCache();

            var mvcOptions = context.RequestServices.GetRequiredService<IOptionsSnapshot<MvcJsonOptions>>();
            var errorResponse = ErrorResponseCreator.Create(_errorModel);
            var json = JsonConvert.SerializeObject(errorResponse, mvcOptions.Value.SerializerSettings);
            await context.Response.WriteJsonAsync(json);
        }
    }
}