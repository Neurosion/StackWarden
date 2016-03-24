using System.ServiceProcess;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using log4net;
using StackWarden.Core.Extensions;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.Service
{
    public class StateMonitorFactory : MonitorConfigurationDrivenFactory<StateMonitorFactory.Configuration, StateMonitor>
    {
        public class Configuration : MonitorConfiguration
        {
            public IEnumerable<string> MachineNames { get; set; }
            public IEnumerable<string> ServiceNames { get; set; }
            public IEnumerable<string> ServiceNamePatterns { get; set; }
        }

        public override IEnumerable<string> SupportedValues => new[] { "Service.State" };

        public StateMonitorFactory(string configPath, IConfigurationReader configurationReader, IMonitorResultHandlerFactory resultHandlerFactory) 
            :base(configPath, configurationReader, resultHandlerFactory)
        { }
        
        protected override IEnumerable<StateMonitor> BuildFromConfig(Configuration config)
        {
            config.MachineNames.ThrowIfNullOrEmpty(nameof(config.MachineNames));

            if (config.ServiceNamePatterns.IsNullOrEmpty())
            {
                foreach (var currentMachineName in config.MachineNames)
                    foreach (var currentServiceName in config.ServiceNames)
                        yield return new StateMonitor(LogManager.GetLogger(typeof(StateMonitor)), currentMachineName, currentServiceName);
            }
                
            foreach (var currentMachineName in config.MachineNames)
            {
                var matchingServiceNames = config.ServiceNamePatterns
                                                 .SelectMany(p => ServiceController.GetServices(currentMachineName)
                                                                                   .Where(x => Regex.IsMatch(x.ServiceName, p))
                                                                                   .Select(x => x.ServiceName));

                matchingServiceNames.ThrowIfNullOrEmpty(nameof(matchingServiceNames),
                                                       $"No services found on '{currentMachineName}' with name matching patterns '{string.Join(", ", config.ServiceNamePatterns)}'.");

                foreach (var currentServiceName in matchingServiceNames)
                    yield return new StateMonitor(LogManager.GetLogger(typeof(StateMonitor)), currentMachineName, currentServiceName);
            }
        }
    }
}