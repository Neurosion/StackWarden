using System.Collections.Generic;
using System.Linq;
using StackWarden.Core.Configuration;

namespace StackWarden.Monitoring.Configuration
{
    public class CompositeMonitorFactory : CompositeConfigurationDrivenFactory<CompositeMonitorFactory.Configuration, IMonitor>, IMonitorFactory
    {
        public class Configuration : ICompositeConfiguration
        {
            public string Type { get; set; }
            public string Name { get; set; }
        }
        
        protected override string ConfigExtension => "monitorconfig";

        public override IEnumerable<string> SupportedValues => SubFactories.SelectMany(x => x.SupportedValues);

        public CompositeMonitorFactory(string configPath, IConfigurationReader configurationReader, IEnumerable<IMonitorFactory> factories)
            :base(configPath, configurationReader, factories)
        { }
    }
}