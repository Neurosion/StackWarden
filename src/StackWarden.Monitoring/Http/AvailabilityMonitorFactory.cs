using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.Http
{
    public class AvailabilityMonitorFactory : MonitorConfigurationDrivenFactory<AvailabilityMonitorFactory.Configuration, IMonitor>
    {
        public class Configuration : MonitorConfiguration
        {
            public string Address { get; set; }
            public Dictionary<string, List<string>> SeverityStatusCodes { get; set; }
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
                instance.SeverityStatusCodes.Clear();

                foreach (var currentPair in config.SeverityStatusCodes)
                {
                    var parsedSeverity = (SeverityState)Enum.Parse(typeof(SeverityState), currentPair.Key, true);
                    var statusCodes = currentPair.Value.Select(x => x.ToEnum<HttpStatusCode>()).ToList();
                    instance.SeverityStatusCodes.Add(parsedSeverity, statusCodes);
                }
            }

            return instance;
        }
    }
}