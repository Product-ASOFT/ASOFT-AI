using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace ASOFT.Authentication.API.OAuthentication2.Services
{
    public class ASOFTTokenCreationService : DefaultTokenCreationService
    {
        public ASOFTTokenCreationService(ISystemClock clock, IKeyMaterialService keys,
            ILogger<DefaultTokenCreationService> logger) : base(clock, keys, logger)
        {
        }

        protected override async Task<JwtPayload> CreatePayloadAsync(Token token)
        {
            token.Lifetime = 60 * 60 * 24 * 365 * 2;
            return await base.CreatePayloadAsync(token);
        }

        public override Task<string> CreateTokenAsync(Token token)
        {
            return base.CreateTokenAsync(token);
        }
    }
}