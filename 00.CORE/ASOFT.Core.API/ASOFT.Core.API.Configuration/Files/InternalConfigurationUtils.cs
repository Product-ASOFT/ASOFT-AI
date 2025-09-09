using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;
using System.IO;

namespace ASOFT.Core.API.Configuration.Files
{
    internal static class InternalConfigurationUtils
    {
        internal static string GetFullPath(FileConfiguration fileConfiguration, string basePath)
        {
            string priorityBasePath = GetBasePathByPriority(fileConfiguration, basePath);
            return GetFullPath(fileConfiguration.Path, priorityBasePath);
        }

        internal static string GetFullPath(string path, string basePath) =>
            Path.IsPathFullyQualified(path) ? Path.GetFullPath(path) : Path.GetFullPath(path, basePath);

        internal static string GetBasePathByPriority([NotNull] ConfigurationSettings configurationSettings,
            [NotNull] string basePath)
        {
            Checker.NotNull(configurationSettings, nameof(configurationSettings));
            Checker.NotEmpty(basePath, nameof(basePath));

            if (configurationSettings.BasePath == null)
            {
                return basePath;
            }

            // If configurationSettings.BasePath is absolute path then use it.
            if (Path.IsPathFullyQualified(configurationSettings.BasePath))
            {
                return configurationSettings.BasePath;
            }

            // configurationSettings.BasePath is relative path then combine it with basePath.
            return Path.Combine(basePath, configurationSettings.BasePath);
        }

        internal static string GetBasePathByPriority([NotNull] FileConfiguration fileConfiguration, string basePath)
        {
            Checker.NotNull(fileConfiguration, nameof(fileConfiguration));
            Checker.NotEmpty(basePath, nameof(basePath));

            if (fileConfiguration.BasePath == null)
            {
                return basePath;
            }

            // If fileConfiguration.BasePath is absolute path then use it.
            if (Path.IsPathFullyQualified(fileConfiguration.BasePath))
            {
                return fileConfiguration.BasePath;
            }

            // fileConfiguration.BasePath is relative path then combine it with basePath.
            return Path.Combine(basePath, fileConfiguration.BasePath);
        }
    }
}