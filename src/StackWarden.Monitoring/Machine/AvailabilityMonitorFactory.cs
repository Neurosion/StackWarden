using System.Collections.Generic;
using log4net;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.Machine
{
    public class AvailabilityMonitorFactory : MonitorConfigurationDrivenFactory<AvailabilityMonitorFactory.Configuration, IMonitor>
    {
        public class Configuration : MonitorConfiguration
        {
            public string Address { get; set; }
            public int? WarningThreshold { get; set; }
            public int? ErrorThreshold { get; set; }
        }

        public override IEnumerable<string> SupportedTypeValues => new[] { "Machine.Availability" };

        public AvailabilityMonitorFactory(string configPath, IConfigurationReader configurationReader, IResultHandlerFactory resultHandlerFactory)
            :base(configPath, configurationReader, resultHandlerFactory)
        { }

        protected override IEnumerable<IMonitor> BuildFromConfig(Configuration config)
        {
            var log = LogManager.GetLogger(typeof (AvailabilityMonitor));
            var instance = new AvailabilityMonitor(log, config.Address);

            if (config.ErrorThreshold.HasValue)
                instance.ErrorThreshold = config.ErrorThreshold.Value;

            if (config.WarningThreshold.HasValue)
                instance.WarningThreshold = config.WarningThreshold.Value;

            yield return instance;
        }
    }
}