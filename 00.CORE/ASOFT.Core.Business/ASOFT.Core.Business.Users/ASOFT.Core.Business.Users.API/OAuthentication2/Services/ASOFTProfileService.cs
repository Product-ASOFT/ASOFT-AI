using ASOFT.AT.Entity;
using ASOFT.Authentication.API.OAuthentication2.Models;
using ASOFT.Contract;
using ASOFT.Security.Core.Identity;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASOFT.Authentication.API.OAuthentication2.Services
{
    /// <summary>
    /// Build user profile for token
    /// </summary>
    public class ASOFTProfileService : DefaultProfileService
    {
        private readonly ITemporaryUserHolder _temporaryUserHolder;

        public ASOFTProfileService(ILogger<ASOFTProfileService> logger, ITemporaryUserHolder temporaryUserHolder) :
            base(logger)
        {
            _temporaryUserHolder = Checker.NotNull(temporaryUserHolder, nameof(temporaryUserHolder));
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await base.GetProfileDataAsync(context);

            // Get user from scope request
            var user = _temporaryUserHolder.GetUser();

            if (user != null)
            {
                context.AddRequestedClaims(GetMoreClaims(user));
            }
        }

        /// <summary>
        /// Get more claims on user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected virtual List<Claim> GetMoreClaims(AT1405 user)
        {
            return new List<Claim>
            {
                new Claim(DefaultClaimTypes.UserID, user.UserID),
                new Claim(DefaultClaimTypes.UserName, user.UserName)
            };
        }
    }
}