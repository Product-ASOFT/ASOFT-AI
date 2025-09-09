using JetBrains.Annotations;

namespace ASOFT.Core.API.Configuration.Files
{
    /// <summary>
    /// Load configuration in file.
    /// </summary>
    public interface IConfigurationFileLoader
    {
        /// <summary>
        /// Base path for load file when file path is relative path.
        /// </summary>
        [NotNull]
        string BasePath { get; }

        /// <summary>
        /// File extension for load, e.g .json, .xml, ...
        /// </summary>
        [NotNull]
        string FileExtension { get; }

        /// <summary>
        /// Load content in file and convert to <see cref="ConfigurationSettings"/>.
        /// </summary>
        /// <param name="path">Absolute or relative path.</param>
        /// <returns></returns>
        ConfigurationSettings Load(string path);
    }
}