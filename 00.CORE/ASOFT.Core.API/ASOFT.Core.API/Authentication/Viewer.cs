using ASOFT.Core.Common.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace ASOFT.Core.API.Authentication
{
    /// <summary>
    /// The current user info
    /// </summary>
    public class Viewer : IViewer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<Viewer> _logger;

        private string _userID;
        private string _userName;
        private CultureInfo _culture;

        /// <inheritdoc />
        public virtual string ID => _userID ?? (_userID = GetClaimValueByTypes(_logger,
                                        _httpContextAccessor.HttpContext.User,
                                        ClaimTypes.NameIdentifier, 
                                        DefaultClaimTypes.UserID,
                                        "sub"));

        /// <inheritdoc />
        public virtual string Name => _userName ??
                                      (_userName = GetClaimValueByTypes(_logger, _httpContextAccessor.HttpContext.User,
                                          ClaimTypes.Name));

        /// <inheritdoc />
        public virtual CultureInfo Culture => _culture ?? (_culture = GetCulture());

        /// <summary>
        /// The viewer
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logger"></param>
        public Viewer(IHttpContextAccessor httpContextAccessor, ILogger<Viewer> logger)
        {
            _httpContextAccessor = Checker.NotNull(httpContextAccessor, nameof(httpContextAccessor));
            _logger = Checker.NotNull(logger, nameof(logger));
        }

        /// <summary>
        /// Get current culture
        /// </summary>
        /// <returns></returns>
        private CultureInfo GetCulture()
        {
            var cultureFeature = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
            return cultureFeature == null ? CultureInfo.CurrentCulture : cultureFeature.RequestCulture.Culture;
        }

        /// <summary>
        /// Get claim by claim types
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="claimsPrincipal"></param>
        /// <param name="claimTypes"></param>
        /// <returns></returns>
        private static string GetClaimValueByTypes(ILogger logger, ClaimsPrincipal claimsPrincipal,
            params string[] claimTypes)
        {
            var claim = claimsPrincipal.FindFirst(m => claimTypes.Contains(m.Type));

            if (claim == null)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError($"Cannot find claim with types: {string.Join(",", claimTypes)}");
                }

                return null;
            }

            var value = claim.Value;

            if (string.IsNullOrWhiteSpace(value))
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Value of claim type: {ClaimType} is null or empty.", claim.Type);
                }
            }

            return value;
        }
    }
}