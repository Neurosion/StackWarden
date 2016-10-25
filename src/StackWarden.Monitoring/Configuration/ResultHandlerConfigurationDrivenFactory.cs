using System.Collections.Generic;
using System.Linq;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.ResultHandling;

namespace StackWarden.Monitoring.Configuration
{
    public abstract class ResultHandlerConfigurationDrivenFactory<TDefinition, TResultHandler> : ConfigurationDrivenFactory<TDefinition, TResultHandler>, IResultHandlerFactory
        where TResultHandler : IResultHandler
    {
        protected override string ConfigExtension => "handlerconfig";

        IEnumerable<string> IFactory<IResultHandler>.SupportedTypeValues => SupportedTypeValues;

        protected ResultHandlerConfigurationDrivenFactory(string configPath, IConfigurationReader configurationReader) 
            :base(configPath, configurationReader)
        { }

        IEnumerable<IResultHandler> IFactory<IResultHandler>.Build(string name)
        {
            return Build(name).Cast<IResultHandler>();
        }

        IEnumerable<IResultHandler> IFactory<IResultHandler>.Build()
        {
            return Build().Cast<IResultHandler>();
        }
    }
}