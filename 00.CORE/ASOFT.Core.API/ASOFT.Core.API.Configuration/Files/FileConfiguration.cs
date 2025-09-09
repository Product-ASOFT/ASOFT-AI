namespace ASOFT.Core.API.Configuration
{
    /// <summary>
    /// File configuration info
    /// </summary>
    public class FileConfiguration
    {
        /// <summary>
        /// Path for load configuration file. The path can be absolute or relative path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// File is optional.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// File is reload onchange.
        /// </summary>
        public bool IsReloadOnChange { get; set; }

        /// <summary>
        /// File is disabled.
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// File has base path.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Current environment for apply.
        /// If <see cref="Environment"/> is null. It's will be loaded default.
        /// Otherwise it will be loaded when matches with Environment of current application.
        /// </summary>
        public string Environment { get; set; }
    }
}