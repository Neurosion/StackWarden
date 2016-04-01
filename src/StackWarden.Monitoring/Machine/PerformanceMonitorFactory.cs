using System.Collections.Generic;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;
using StackWarden.Core.Extensions;

namespace StackWarden.Monitoring.Machine
{
    public class PerformanceMonitorFactory : MonitorConfigurationDrivenFactory<PerformanceMonitorFactory.Configuration, IMonitor>
    {
        public class Configuration : MonitorConfiguration
        {
            public IEnumerable<string> MachineNames { get; set; }
            public Dictionary<string, int> CpuUsageSeverity { get; set; }
            public Dictionary<string, int> MemoryAvailableSeverity { get; set; }
            public Dictionary<string, int> DiskSpaceAvailableSeverity { get; set; }
            public Dictionary<string, int> MSMQStorageUsageSeverity { get; set; }
        }

        public override IEnumerable<string> SupportedValues => new[] { "Machine.Performance" };

        public PerformanceMonitorFactory(string configPath, IConfigurationReader configurationReader, IMonitorResultHandlerFactory resultHandlerFactory)
            : base(configPath, configurationReader, resultHandlerFactory)
        { }

        protected override IEnumerable<IMonitor> BuildFromConfig(Configuration config)
        {
            config.ThrowIfNull(nameof(config.MachineNames));

            foreach (var currentName in config.MachineNames)
            {
                var instance = new PerformanceMonitor(LogManager.GetLogger(typeof(PerformanceMonitor)), currentName);

                MapConfiguration(config.CpuUsageSeverity, instance.CpuUsageSeverity);
                MapConfiguration(config.DiskSpaceAvailableSeverity, instance.DiskSpaceAvailableSeverity);
                MapConfiguration(config.MemoryAvailableSeverity, instance.MemoryAvailableSeverity);
                MapConfiguration(config.MSMQStorageUsageSeverity, instance.MSMQStorageUsageSeverity);

                yield return instance;
            }
        }

        private void MapConfiguration(Dictionary<string, int> source, Dictionary<SeverityState, int> destination)
        {
            if (source == null)
                return;

            foreach (var currentPair in source)
            {
                var severity = currentPair.Key.ToEnum<SeverityState>();

                if (!destination.ContainsKey(severity))
                    destination.Add(severity, 0);

                destination[severity] = currentPair.Value;
            }
        }
    }
}