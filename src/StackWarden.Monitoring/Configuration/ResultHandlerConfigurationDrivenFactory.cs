using System.Collections.Generic;
using System.Linq;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.ResultHandling;

namespace StackWarden.Monitoring.Configuration
{
    public abstract class ResultHandlerConfigurationDrivenFactory<TDefinition, TResultHandler> : ConfigurationDrivenFactory<TDefinition, TResultHandler>, IMonitorResultHandlerFactory
        where TResultHandler : IMonitorResultHandler
    {
        protected override string ConfigExtension => "handlerconfig";

        IEnumerable<string> IFactory<IMonitorResultHandler>.SupportedValues => SupportedValues;

        protected ResultHandlerConfigurationDrivenFactory(string configPath, IConfigurationReader configurationReader) 
            :base(configPath, configurationReader)
        { }

        IMonitorResultHandler IFactory<IMonitorResultHandler>.Build(string name, bool useExistingInstance)
        {
            return Build(name, useExistingInstance);
        }

        IEnumerable<IMonitorResultHandler> IFactory<IMonitorResultHandler>.BuildAll()
        {
            return BuildAll().Cast<IMonitorResultHandler>();
        }
    }
}