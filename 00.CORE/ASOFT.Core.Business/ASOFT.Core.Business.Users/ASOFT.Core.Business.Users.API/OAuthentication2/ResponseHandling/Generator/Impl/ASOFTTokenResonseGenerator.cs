using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace ASOFT.Authentication.API.OAuthentication2.ResponseHandling.Generator.Impl
{
    public class ASOFTTokenResonseGenerator : TokenResponseGenerator
    {
        public ASOFTTokenResonseGenerator(ISystemClock clock,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService, IResourceStore resources,
            IClientStore clients,
            ILogger<TokenResponseGenerator> logger) : base(clock, tokenService, refreshTokenService, resources, clients,
            logger)
        {
        }
    }
}