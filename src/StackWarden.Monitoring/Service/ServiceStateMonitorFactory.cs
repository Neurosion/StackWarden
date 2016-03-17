using System.Collections.Generic;
using log4net;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.Service
{
    public class StateMonitorFactory : MonitorConfigurationDrivenFactory<StateMonitorFactory.Configuration, StateMonitor>
    {
        public class Configuration : MonitorConfiguration
        {
            public string Machine { get; set; }
            public string Service { get; set; }
        }

        public override IEnumerable<string> SupportedValues => new[] { "Service.State" };

        public StateMonitorFactory(string configPath, IConfigurationReader configurationReader, IMonitorResultHandlerFactory resultHandlerFactory) 
            :base(configPath, configurationReader, resultHandlerFactory)
        { }
        
        protected override StateMonitor BuildFromConfig(Configuration config)
        {
            var instance = new StateMonitor(LogManager.GetLogger(typeof(StateMonitor)), config.Machine, config.Service);

            return instance;
        }
    }
}