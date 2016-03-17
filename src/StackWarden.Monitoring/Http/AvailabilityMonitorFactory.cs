using System.Collections.Generic;
using System.Net;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.Http
{
    public class AvailabilityMonitorFactory : MonitorConfigurationDrivenFactory<AvailabilityMonitorFactory.Configuration, IMonitor>
    {
        public class Configuration : MonitorConfiguration
        {
            public string Address { get; set; }
            public Dictionary<SeverityState, List<HttpStatusCode>> SeverityStatusCodes { get; set; }
        }

        public override IEnumerable<string> SupportedValues => new[] { "Http.Availability" };

        public AvailabilityMonitorFactory(string configPath, IConfigurationReader configurationReader, IMonitorResultHandlerFactory resultHandlerFactory) 
            :base(configPath, configurationReader, resultHandlerFactory)
        { }
        
        protected override IMonitor BuildFromConfig(Configuration config)
        {
            var log = LogManager.GetLogger(typeof (AvailabilityMonitor));
            var instance = new AvailabilityMonitor(log, config.Address);

            if (config.SeverityStatusCodes != null)
            {
                // todo: assign values
            }

            return instance;
        }
    }
}