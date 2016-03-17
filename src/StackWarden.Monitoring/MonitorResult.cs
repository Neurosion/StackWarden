using System;
using System.Collections.Generic;
using StackWarden.Core;

namespace StackWarden.Monitoring
{
    public class MonitorResult
    {
        public DateTime SentOn { get; } = DateTime.Now;
        public string SourceName { get; set; }
        public Type SourceType { get; set; }
        public string[] Tags { get; set; }
        public string TargetName { get; set; }
        public SeverityState TargetState { get; set; }
        public double TimeToLive { get; set; }
        public string FriendlyMessage { get; set; }
        public Dictionary<string, string> Details { get; } = new Dictionary<string, string>();
    }
}