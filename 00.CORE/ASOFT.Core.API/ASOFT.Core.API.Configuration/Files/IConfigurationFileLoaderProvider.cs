using JetBrains.Annotations;

namespace ASOFT.Core.API.Configuration.Files
{
    /// <summary>
    /// Helper provider configuration file loader by type.
    /// </summary>
    public interface IConfigurationFileLoaderProvider
    {
        /// <summary>
        /// Provider <see cref="IConfigurationFileLoader"/> by file type.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        [NotNull]
        IConfigurationFileLoader Provide(string fileExtension);
    }
}