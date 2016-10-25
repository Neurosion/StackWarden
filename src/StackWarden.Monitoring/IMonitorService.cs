using System.Collections.Generic;

namespace StackWarden.Monitoring
{
    public interface IMonitorService
    {
        IEnumerable<IMonitor> GetAllMonitors();
        IEnumerable<Result> GetLatestResults();
        void Save(Result result);
    }
}