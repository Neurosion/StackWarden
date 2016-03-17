using System.Collections.Generic;

namespace StackWarden.Monitoring
{
    public interface IMonitorService
    {
        IEnumerable<IMonitor> GetAllMonitors();
        IEnumerable<MonitorResult> GetLatestResults();
        void Save(MonitorResult result);
    }
}