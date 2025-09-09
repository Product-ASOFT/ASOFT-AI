using ASOFT.Core.API.Configuration.Files;
using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ASOFT.Core.API.Configuration.Extensions
{
    /// <summary>
    /// Web host builder extension for <see cref="ConfigurationSettings"/>.
    /// </summary>
    public static class ConfigurationWebHostBuilderExtensions
    {
        /// <summary>
        /// Use configuration settings file for load all files to <see cref="ConfigurationBuilder"/>.
        /// </summary>
        /// <param name="webHostBuilder"></param>
        /// <param name="filePath">The path to load file contain <see cref="ConfigurationSettings"/>. Path can be absolute or relative path.</param>
        /// <returns></returns>
        public static IWebHostBuilder UseConfigurationSettingsFile([NotNull] this IWebHostBuilder webHostBuilder,
            [NotNull] string filePath)
        {
            return UseConfigurationSettingsFile(webHostBuilder, filePath, Directory.GetCurrentDirectory());
        }

        /// <summary>
        /// Use configuration settings file for load all files to <see cref="ConfigurationBuilder"/>.
        /// </summary>
        /// <param name="webHostBuilder"></param>
        /// <param name="filePath">The path to load file contain <see cref="ConfigurationSettings"/>. Path can be absolute or relative path.</param>
        /// <param name="basePath">The base path.</param>
        /// <returns></returns>
        public static IWebHostBuilder UseConfigurationSettingsFile([NotNull] this IWebHostBuilder webHostBuilder,
            [NotNull] string filePath, [NotNull] string basePath)
        {
            Checker.NotEmpty(basePath, nameof(basePath));

            var configurationFileLoaderProvider = ConfigurationFileLoaderProvider.CreateDefault(basePath);
            return UseConfigurationSettingsFile(webHostBuilder, filePath, configurationFileLoaderProvider);
        }

        /// <summary>
        /// Use configuration settings file for load all files to <see cref="ConfigurationBuilder"/>.
        /// </summary>
        /// <param name="webHostBuilder"></param>
        /// <param name="filePath">The path to load file contain <see cref="ConfigurationSettings"/>. Path can be absolute or relative path.</param>
        /// <param name="configurationFileLoaderProvider">The provider provide file loader to load <see cref="ConfigurationSettings"/>.</param>
        public static IWebHostBuilder UseConfigurationSettingsFile([NotNull] this IWebHostBuilder webHostBuilder,
            [NotNull] string filePath, [NotNull] IConfigurationFileLoaderProvider configurationFileLoaderProvider)
        {
            Checker.NotNull(webHostBuilder, nameof(webHostBuilder));
            Checker.NotEmpty(filePath, nameof(filePath));
            Checker.NotNull(configurationFileLoaderProvider, nameof(configurationFileLoaderProvider));

            // Get file extension, e.g .json, .xml...
            var fileExtension = Path.GetExtension(filePath);
            // Get configuration file loader by file extension.
            var configurationFileLoader = configurationFileLoaderProvider.Provide(fileExtension);

            // Thrown error when base path of file loader is null or white space.
            Checker.NotEmpty(configurationFileLoader.BasePath, nameof(configurationFileLoader.BasePath));

            // Load configuration settings.
            var configurationSettings = configurationFileLoader.Load(filePath);

            // Make sure configuration settings is not null.
            Checker.NotNull(configurationSettings, nameof(configurationSettings));

            webHostBuilder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                string environment = Checker.NotEmpty(context.HostingEnvironment.EnvironmentName,
                    nameof(context.HostingEnvironment.EnvironmentName));
                string basePath = InternalConfigurationUtils.GetBasePathByPriority(configurationSettings,
                    configurationFileLoader.BasePath);

                // Load from folders when folderConfigurations is not null.
                if (configurationSettings.FolderConfigurations != null)
                {
                    AddConfigurationFromFolders(environment, configurationBuilder,
                        configurationSettings.FolderConfigurations,
                        basePath);
                }

                // Load from files when file configurations is not null.
                if (configurationSettings.FileConfigurations != null)
                {
                    AddConfigurationFromFiles(environment, configurationBuilder,
                        configurationSettings.FileConfigurations, basePath);
                }
            });

            return webHostBuilder;
        }

        private static void AddConfigurationFromFolders(string environment, IConfigurationBuilder configurationBuilder,
            IEnumerable<FolderConfiguration> folderConfigurations, string basePath)
        {
            // Filter folders configuration is disabled.
            foreach (var folderConfiguration in folderConfigurations.Where(m =>
                !m.IsDisabled &&
                (m.Environment == null || environment.Equals(m.Environment, StringComparison.Ordinal))))
            {
                // Validate path and search pattern.
                Checker.NotEmpty(folderConfiguration.Path, nameof(folderConfiguration.Path));
                Checker.NotEmpty(folderConfiguration.SearchPattern, nameof(folderConfiguration.SearchPattern));

                // Get full folder path by priority.
                string fullFolderPath = InternalConfigurationUtils.GetFullPath(folderConfiguration, basePath);
                // Get all files by folder path, search pattern and search option.
                var files = Directory.EnumerateFiles(fullFolderPath, folderConfiguration.SearchPattern,
                    folderConfiguration.SearchOption);

                AddConfigurationFromFolder(configurationBuilder, folderConfiguration, files);
            }
        }

        private static void AddConfigurationFromFolder(IConfigurationBuilder configurationBuilder,
            FolderConfiguration folderConfiguration, IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                // Get file extension.
                var fileExtension = Path.GetExtension(file);

                // If file is json file.
                if (ConfigurationConstants.JsonFileExtension.Equals(fileExtension, StringComparison.Ordinal))
                {
                    configurationBuilder.AddJsonFile(file, folderConfiguration.IsOptional,
                        folderConfiguration.IsReloadOnChange);
                }
                // If file is xml file.
                else if (ConfigurationConstants.XmlFileExtension.Equals(fileExtension, StringComparison.Ordinal))
                {
                    configurationBuilder.AddXmlFile(file, folderConfiguration.IsOptional,
                        folderConfiguration.IsReloadOnChange);
                }
            }
        }

        private static void AddConfigurationFromFiles(string environment, IConfigurationBuilder configurationBuilder,
            IEnumerable<FileConfiguration> fileConfigurations, string basePath)
        {
            // Filter file configuration is disabled.
            foreach (var fileConfiguration in fileConfigurations.Where(m =>
                !m.IsDisabled &&
                (m.Environment == null || environment.Equals(m.Environment, StringComparison.Ordinal))))
            {
                Checker.NotEmpty(fileConfiguration.Path, nameof(fileConfiguration.Path));

                // Get file extension.
                var fileExtension = Path.GetExtension(fileConfiguration.Path);

                // If file is json file.
                if (ConfigurationConstants.JsonFileExtension.Equals(fileExtension, StringComparison.Ordinal))
                {
                    // Get full folder path by priority.
                    string fullFilePath = InternalConfigurationUtils.GetFullPath(fileConfiguration, basePath);
                    configurationBuilder.AddJsonFile(fullFilePath, fileConfiguration.IsOptional,
                        fileConfiguration.IsReloadOnChange);
                }
                // If file is xml file.
                else if (ConfigurationConstants.XmlFileExtension.Equals(fileExtension, StringComparison.Ordinal))
                {
                    // Get full folder path by priority.
                    string fullFilePath = InternalConfigurationUtils.GetFullPath(fileConfiguration, basePath);
                    configurationBuilder.AddXmlFile(fullFilePath, fileConfiguration.IsOptional,
                        fileConfiguration.IsReloadOnChange);
                }
            }
        }
    }
}