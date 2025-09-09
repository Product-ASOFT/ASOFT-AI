using System.Collections.Generic;

namespace ASOFT.Core.API.Configuration
{
    /// <summary>
    /// Configuration settings.
    /// </summary>
    public class ConfigurationSettings
    {
        /// <summary>
        /// Base path of configuration settings. Can be null.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// List of <see cref="FolderConfiguration"/>.
        /// </summary>
        public IEnumerable<FolderConfiguration> FolderConfigurations { get; set; }


        /// <summary>
        /// List of <see cref="FileConfiguration"/>.
        /// </summary>
        public IEnumerable<FileConfiguration> FileConfigurations { get; set; }
    }
}