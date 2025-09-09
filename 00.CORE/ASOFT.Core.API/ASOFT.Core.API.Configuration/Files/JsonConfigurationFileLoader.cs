using ASOFT.Core.API.Configuration.Files;
using ASOFT.Core.Common.InjectionChecker;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace ASOFT.Core.API.Configuration.Files
{
    /// <summary>
    /// Json configuration file loader.
    /// </summary>
    public class JsonConfigurationFileLoader : IConfigurationFileLoader
    {
        /// <inheritdoc />
        public string BasePath { get; }

        /// <inheritdoc />
        public string FileExtension { get; }

        /// <summary>
        /// Constructor of <see cref="JsonConfigurationFileLoader"/>.
        /// </summary>
        /// <param name="basePath">The absolute or relative path.</param>
        public JsonConfigurationFileLoader(string basePath)
        {
            BasePath = Checker.NotEmpty(basePath, nameof(basePath));
            FileExtension = ConfigurationConstants.JsonFileExtension;
        }

        /// <summary>
        /// Load <see cref="ConfigurationSettings"/> from json file.
        /// </summary>
        /// <param name="path">An absolute or relative path.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">Thrown when path is not found.</exception>
        /// <exception cref="ArgumentException">Thrown when file extension not valid.</exception>
        public ConfigurationSettings Load(string path)
        {
            Checker.NotEmpty(path, nameof(path));
            var fullPath = InternalConfigurationUtils.GetFullPath(path, BasePath);

            // Validate path.
            InternalConfigurationChecker.FileMustBeExisted(fullPath);
            InternalConfigurationChecker.MakeSureExtensionFileValid(fullPath, FileExtension);

            return JsonConvert.DeserializeObject<ConfigurationSettings>(File.ReadAllText(fullPath, Encoding.UTF8));
        }
    }
}