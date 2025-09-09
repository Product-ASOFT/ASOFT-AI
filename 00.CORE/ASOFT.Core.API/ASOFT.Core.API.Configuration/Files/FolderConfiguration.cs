using System.IO;

namespace ASOFT.Core.API.Configuration
{
    /// <summary>
    /// Configuration files in folder
    /// </summary>
    public class FolderConfiguration : FileConfiguration
    {
        /// <summary>
        /// Pattern for search files in folder.
        /// </summary>
        public string SearchPattern { get; set; }

        /// <summary>
        /// Option for search files. Search all or top directory.
        /// </summary>
        public SearchOption SearchOption { get; set; }
    }
}