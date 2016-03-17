using System.Collections.Generic;
using StackWarden.Core.Configuration;

namespace StackWarden.Monitoring.Configuration
{
    public interface IMonitorConfiguration : IConfiguration
    {
        string DisplayName { get; set; }
        string[] Tags { get; set; }
        int? Interval { get; set; }
        IEnumerable<string> Handlers { get; set; }
    }
}