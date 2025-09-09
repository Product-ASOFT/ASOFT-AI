namespace ASOFT.Core.API.Https
{
    /// <summary>
    /// Thiết lập https
    /// </summary>
    public class HttpsSettings
    {
        /// <summary>
        /// Https port mặc định
        /// </summary>
        public const int DefaultHttpsPort = 443;

        /// <summary>
        /// Section key
        /// </summary>
        public const string SectionKey = "HttpsSettings";

        /// <summary>
        /// Enable
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Https port
        /// </summary>
        public int? HttpsPort { get; set; }
    }
}