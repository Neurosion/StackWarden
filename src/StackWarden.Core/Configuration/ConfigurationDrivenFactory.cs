using StackWarden.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StackWarden.Core.Configuration
{
    public abstract class ConfigurationDrivenFactory<TDefinition, TResult> : IFactory<TResult>
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly string _configPath;

        protected abstract string ConfigExtension { get; }
        protected virtual TDefinition Definition => Activator.CreateInstance<TDefinition>();
        public abstract IEnumerable<string> SupportedValues { get; }

        protected ConfigurationDrivenFactory(string configPath, IConfigurationReader configurationReader)
        {
            _configPath = configPath;
            _configurationReader = configurationReader.ThrowIfNull(nameof(configurationReader));
        }

        public IEnumerable<TResult> Build(string name)
        {
            var config = LoadConfiguration(name);
            var instances = BuildFromConfig(config);

            foreach (var currentInstance in instances)
                ApplyPostBuildConfiguration(config, currentInstance);

            return instances;
        }

        public IEnumerable<TResult> Build()
        {
            var foundConfigurations = Directory.GetFiles(_configPath, $"*.{ConfigExtension}")
                                               .Select(Path.GetFileNameWithoutExtension);
            var builtResults = foundConfigurations.SelectMany(x => Build(x));

            return builtResults;
        }

        protected abstract IEnumerable<TResult> BuildFromConfig(TDefinition config);

        protected virtual TDefinition LoadConfiguration(string name)
        {
            var pathSeparator = _configPath.EndsWith(@"\") ? "" : @"\";
            var fullPath = $"{_configPath}{pathSeparator}{name}.{ConfigExtension}";
            var config = _configurationReader.Read(fullPath, Definition);

            return config;
        }

        protected virtual void ApplyPostBuildConfiguration(TDefinition config, TResult instance) { }
    }
}