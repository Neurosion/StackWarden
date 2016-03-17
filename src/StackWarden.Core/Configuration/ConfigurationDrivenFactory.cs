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

        private readonly Dictionary<string, TResult> _builtInstances = new Dictionary<string, TResult>();

        protected ConfigurationDrivenFactory(string configPath, IConfigurationReader configurationReader)
        {
            _configPath = configPath;
            _configurationReader = configurationReader;
        }

        public TResult Build(string name, bool useExistingInstance = false)
        {
            if (!useExistingInstance)
                return BuildFromConfigName(name);

            if (!_builtInstances.ContainsKey(name))
                _builtInstances.Add(name, BuildFromConfigName(name));

            return _builtInstances[name];
        }

        public IEnumerable<TResult> BuildAll()
        {
            var foundConfigurations = Directory.GetFiles(_configPath, $"*.{ConfigExtension}")
                                               .Select(Path.GetFileNameWithoutExtension);
            var builtResults = foundConfigurations.Select(x => Build(x));

            return builtResults;
        }

        protected abstract TResult BuildFromConfig(TDefinition config);

        protected virtual TDefinition LoadConfiguration(string name)
        {
            var pathSeparator = _configPath.EndsWith(@"\") ? "" : @"\";
            var fullPath = $"{_configPath}{pathSeparator}{name}.{ConfigExtension}";
            var config = _configurationReader.Read(fullPath, Definition);

            return config;
        }

        private TResult BuildFromConfigName(string name)
        {
            var config = LoadConfiguration(name);
            var instance = BuildFromConfig(config);
            ApplyPostBuildConfiguration(config, instance);

            return instance;
        }

        protected virtual void ApplyPostBuildConfiguration(TDefinition config, TResult instance) { }
    }
}