using System.Collections.Generic;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.ResultHandling;

namespace StackWarden.Monitoring.Configuration
{
    public class CompositeResultHandlerFactory : CompositeConfigurationDrivenFactory<CompositeResultHandlerFactory.Configuration, IMonitorResultHandler>, IMonitorResultHandlerFactory
    {
        public class Configuration : ICompositeConfiguration
        {
            public string Type { get; set; }
            public string Name { get; set; }
        }

        protected override string ConfigExtension => "handlerconfig";

        public CompositeResultHandlerFactory(string configPath, IConfigurationReader configurationReader, IEnumerable<IMonitorResultHandlerFactory> factories) 
            :base(configPath, configurationReader, factories)
        { }
    }
}