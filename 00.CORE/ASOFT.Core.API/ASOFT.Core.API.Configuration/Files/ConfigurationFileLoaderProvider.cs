using ASOFT.Core.Common.InjectionChecker;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ASOFT.Core.API.Configuration.Files
{
    /// <summary>
    /// Default configuration file loader provider.
    /// </summary>
    public class ConfigurationFileLoaderProvider : IConfigurationFileLoaderProvider
    {
        private readonly ConcurrentDictionary<string, IConfigurationFileLoader> _configurationFileLoaders;
        private readonly IReadOnlyDictionary<string, Func<IConfigurationFileLoader>> _configurationProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationProvider">Dictionary function provide <see cref="IConfigurationFileLoader"/>.</param>
        public ConfigurationFileLoaderProvider(
            IReadOnlyDictionary<string, Func<IConfigurationFileLoader>> configurationProvider)
        {
            _configurationProvider = Checker.NotNull(configurationProvider, nameof(configurationProvider));
            _configurationFileLoaders = new ConcurrentDictionary<string, IConfigurationFileLoader>();
        }

        /// <inheritdoc cref="IConfigurationFileLoaderProvider.Provide"/>
        public IConfigurationFileLoader Provide(string fileExtension)
        {
            Checker.NotEmpty(fileExtension, nameof(fileExtension));

            // If file type configuration file loader is existed then get from dictionary.
            if (_configurationFileLoaders.TryGetValue(fileExtension,
                out IConfigurationFileLoader configurationFileLoader))
            {
                return configurationFileLoader;
            }

            // Get file configuration function by file type from provider
            if (_configurationProvider.TryGetValue(fileExtension,
                out Func<IConfigurationFileLoader> configurationFileLoaderAccessor))
            {
                configurationFileLoader = configurationFileLoaderAccessor();
                _configurationFileLoaders[fileExtension] = configurationFileLoader;
                return configurationFileLoader;
            }

            throw new NotSupportedException($"File type `{fileExtension}` is not supported.");
        }

        /// <summary>
        /// Create default <see cref="IConfigurationFileLoaderProvider"/>.
        /// </summary>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static IConfigurationFileLoaderProvider CreateDefault(string basePath)
        {
            Checker.NotEmpty(basePath, nameof(basePath));

            return new ConfigurationFileLoaderProvider(new Dictionary<string, Func<IConfigurationFileLoader>>
            {
                {ConfigurationConstants.JsonFileExtension, () => new JsonConfigurationFileLoader(basePath)}
            });
        }
    }
}