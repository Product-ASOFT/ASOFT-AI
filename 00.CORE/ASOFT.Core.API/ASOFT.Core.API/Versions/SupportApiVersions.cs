using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

// ReSharper disable All

namespace ASOFT.Core.API.Versions
{
    /// <summary>
    /// Support api versions
    /// </summary>
    public static class SupportApiVersions
    {
        /// <summary>
        /// Version reader
        /// </summary>
        public const string VersionReaderName = "api-version";

        /// <summary>
        /// Current version
        /// </summary>
        public const string CurrentVersionStr = V_1_0_Str;

        /// <summary>
        /// Version 1.0 by string
        /// </summary>
        public const string V_1_0_Str = "1.0";

        /// <summary>
        /// Version 2.0 by string
        /// </summary>
        public const string V_2_0_Str = "2.0";

        /// <summary>
        /// Version 1.0
        /// </summary>
        public static readonly ApiVersion V_1_0 = ApiVersion.Parse(V_1_0_Str);

        /// <summary>
        /// Version 2.0
        /// </summary>
        public static readonly ApiVersion V_2_0 = ApiVersion.Parse(V_2_0_Str);

        /// <summary>
        /// Url segment api version reader
        /// </summary>
        public static readonly IApiVersionReader UrlSegmentApiVersionReader = new UrlSegmentApiVersionReader();

        /// <summary>
        /// Query api version reader
        /// </summary>
        public static readonly IApiVersionReader QueryApiVersionReader =
            new QueryStringApiVersionReader(VersionReaderName);

        /// <summary>
        /// Header api version reader
        /// </summary>
        public static readonly IApiVersionReader HeaderApiVersionReader = new HeaderApiVersionReader(VersionReaderName);

        /// <summary>
        /// Combine api version reader
        /// </summary>
        public static readonly IApiVersionReader CombineApiVersion =
            ApiVersionReader.Combine(UrlSegmentApiVersionReader, QueryApiVersionReader, HeaderApiVersionReader);
    }
}