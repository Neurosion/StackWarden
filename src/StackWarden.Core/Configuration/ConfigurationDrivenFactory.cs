using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StackWarden.Core.Extensions;

namespace StackWarden.Core.Configuration
{
    public abstract class ConfigurationDrivenFactory<TDefinition, TResult> : IFactory<TResult>
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly string _configPath;
        private readonly Dictionary<string, TDefinition> _configurationInstances = new Dictionary<string, TDefinition>();
        private readonly Dictionary<string, IEnumerable<TResult>> _builtInstances = new Dictionary<string, IEnumerable<TResult>>();

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
            if (!_configurationInstances.ContainsKey(name))
                _configurationInstances.Add(name, LoadConfiguration(name));

            var config = _configurationInstances[name];

            if (!_builtInstances.ContainsKey(name))
            {
                var instances = BuildFromConfig(config).ToList();

                foreach (var currentInstance in instances)
                    ApplyPostBuildConfiguration(config, currentInstance);

                _builtInstances.Add(name, instances);
            }

            return _builtInstances[name];
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