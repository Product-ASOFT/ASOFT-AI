using ASOFT.API.Core.ApiResponse;
using ASOFT.Authentication.Models;
using ASOFT.Contract;
using IdentityModel;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace ASOFT.Authentication.API.OAuthentication2.Endpoints.Results
{
    public class ASOFTTokenResult : IEndpointResult
    {
        private readonly TokenResponse _tokenResponse;

        public ASOFTTokenResult(TokenResponse tokenResponse)
        {
            _tokenResponse = Checker.NotNull(tokenResponse, nameof(tokenResponse));
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.SetNoCache();

            var mvcOptions = context.RequestServices.GetRequiredService<IOptionsSnapshot<MvcJsonOptions>>();
            var jsonSerializer = JsonSerializer.Create(mvcOptions.Value.SerializerSettings);

            // Create default response
            var response = SuccessResponseCreator.Create(new TokenModel
            {
                AccessToken = _tokenResponse.AccessToken,
                ExpiresIn = _tokenResponse.AccessTokenLifetime,
                IDToken = _tokenResponse.IdentityToken,
                RefreshToken = _tokenResponse.RefreshToken,
                Scope = _tokenResponse.Scope,
                TokenType = OidcConstants.TokenResponse.BearerTokenType
            });

            var responseJObject = JObject.FromObject(response, jsonSerializer);

            // Get data jObject and merge more attribute to data response
            if (responseJObject.TryGetValue(nameof(response.Data), StringComparison.OrdinalIgnoreCase,
                    out JToken value) && value is JObject dataJObject && null != _tokenResponse.Custom)
            {
                var extensionsJObject = JObject.FromObject(_tokenResponse.Custom, jsonSerializer);
                dataJObject.Merge(extensionsJObject, new JsonMergeSettings
                {
                    MergeNullValueHandling = MergeNullValueHandling.Merge,
                    PropertyNameComparison = StringComparison.Ordinal,
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

            await context.Response.WriteJsonAsync(responseJObject.ToString());
        }
    }
}