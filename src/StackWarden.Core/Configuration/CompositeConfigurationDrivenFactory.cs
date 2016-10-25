using System;
using System.Collections.Generic;
using System.Linq;
using StackWarden.Core.Extensions;

namespace StackWarden.Core.Configuration
{
    public abstract class CompositeConfigurationDrivenFactory<TDefinition, TResult> : ConfigurationDrivenFactory<TDefinition, TResult>
        where TDefinition: ICompositeConfiguration
    {
        protected List<IFactory<TResult>> SubFactories { get; }

        public override IEnumerable<string> SupportedTypeValues => SubFactories.SelectMany(x => x.SupportedTypeValues);
        
        protected CompositeConfigurationDrivenFactory(string configPath, IConfigurationReader configurationReader, IEnumerable<IFactory<TResult>> factories)
            :base(configPath, configurationReader)
        {
            SubFactories = factories.ThrowIfNullOrEmpty(nameof(factories))
                                    .ToList();
        }

        protected override TDefinition LoadConfiguration(string name)
        {
            var config = base.LoadConfiguration(name);
            config.Name = name;

            return config;
        }

        protected override IEnumerable<TResult> BuildFromConfig(TDefinition config)
        {
            var subFactory = SubFactories.FirstOrDefault(x => x.SupportedTypeValues.Contains(config.Type));

            if (subFactory == null)
                throw new NotSupportedException($"{typeof(TResult).Name} type '{config.Type}' does not have a factory.");

            var instance = subFactory.Build(config.Name);

            return instance;
        }
    }
}