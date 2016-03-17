using System.Collections.Generic;

namespace StackWarden.Monitoring.Configuration
{
    public class MonitorConfiguration : IMonitorConfiguration
    {
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public string[] Tags { get; set; }
        public int? Interval { get; set; }
        public IEnumerable<string> Handlers { get; set; }
    }
}