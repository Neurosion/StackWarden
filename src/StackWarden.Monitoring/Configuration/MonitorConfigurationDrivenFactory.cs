using System.Collections.Generic;
using System.Linq;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;

namespace StackWarden.Monitoring.Configuration
{
    public abstract class MonitorConfigurationDrivenFactory<TDefinition, TMonitor> : ConfigurationDrivenFactory<TDefinition, TMonitor>, IMonitorFactory
        where TMonitor: IMonitor
        where TDefinition: IMonitorConfiguration
    {
        private readonly IResultHandlerFactory _resultHandlerFactory;

        protected override string ConfigExtension => "monitorconfig";

        protected MonitorConfigurationDrivenFactory(string configPath, IConfigurationReader configurationReader, IResultHandlerFactory resultHandlerFactory)
            : base(configPath, configurationReader)
        {
            _resultHandlerFactory = resultHandlerFactory.ThrowIfNull(nameof(resultHandlerFactory));
        }

        protected override void ApplyPostBuildConfiguration(TDefinition config, TMonitor instance)
        {
            if (config.Interval.HasValue)
                instance.Interval = config.Interval.Value;

            if (config.Tags?.Any() ?? false)
                instance.Tags.AddRange(config.Tags);

            foreach (var currentHandler in config.Handlers.SelectMany(x => _resultHandlerFactory.Build(x)))
                instance.Updated += r => currentHandler.Handle(r);

            if (!string.IsNullOrWhiteSpace(config.DisplayName))
                instance.Name = config.DisplayName;
        }

        // Explicit IFactory<IMonitor> implementation
        IEnumerable<string> IFactory<IMonitor>.SupportedTypeValues => SupportedTypeValues;

        IEnumerable<IMonitor> IFactory<IMonitor>.Build(string name)
        {
            return Build(name).Cast<IMonitor>();
        }

        IEnumerable<IMonitor> IFactory<IMonitor>.Build()
        {
            return Build().Cast<IMonitor>();
        }
    }
}