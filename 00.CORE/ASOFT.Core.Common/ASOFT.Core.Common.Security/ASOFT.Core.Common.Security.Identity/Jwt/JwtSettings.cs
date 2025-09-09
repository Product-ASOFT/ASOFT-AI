namespace ASOFT.Core.Common.Security.Identity.Jwt
{
    /// <summary>
    /// Configuration settings for jwt
    /// </summary>
    public class JwtSettings
    {
        public const string SectionKey = "Jwt";

        /// <summary>
        /// Jwt key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Jwt issuer
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Jwt audience
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// The host authentication
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Is save token in memory
        /// </summary>
        public bool SaveToken { get; set; }

        public bool RefreshOnIssuerKeyNotFound { get; set; }
        public bool IncludeErrorDetails { get; set; }
    }
}