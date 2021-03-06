﻿using System.Collections.Generic;
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

        IEnumerable<IMonitorResultHandler> IFactory<IMonitorResultHandler>.Build(string name)
        {
            return Build(name).Cast<IMonitorResultHandler>();
        }

        IEnumerable<IMonitorResultHandler> IFactory<IMonitorResultHandler>.Build()
        {
            return Build().Cast<IMonitorResultHandler>();
        }
    }
}