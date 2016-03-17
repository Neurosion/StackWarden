using StackWarden.Core.Configuration;
using StackWarden.Monitoring.ResultHandling;

namespace StackWarden.Monitoring.Configuration
{
    public abstract class MonitorResultHandlerFactory<TDefinition, TMonitorResultHandler> : ConfigurationDrivenFactory<TDefinition, TMonitorResultHandler>
        where TMonitorResultHandler: IMonitorResultHandler
    {
        protected override string ConfigExtension => "handlerconfig";

        protected MonitorResultHandlerFactory(string configPath, IConfigurationReader configurationReader)
            :base(configPath, configurationReader)
        { }
    }
}