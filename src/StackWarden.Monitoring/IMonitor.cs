using System;
using System.Collections.Generic;

namespace StackWarden.Monitoring
{
    public interface IMonitor
    {
        string Name { get; set; }
        List<string> Tags { get; }
        double Interval { get; set; }

        event Action<MonitorResult> Updated;

        void Start();
        void Stop();
    }
}