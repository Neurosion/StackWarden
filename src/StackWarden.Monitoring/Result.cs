using System;
using System.Collections.Generic;
using StackWarden.Core;

namespace StackWarden.Monitoring
{
    public class Result
    {
        public DateTime SentOn { get; } = DateTime.Now;
        public TimeSpan TimeToLive { get; set; }
        public DateTime ExpiresOn { get { return SentOn.Add(TimeToLive); } }
        public bool IsExpired { get { return ExpiresOn < DateTime.Now; } }
        public ResultSource Source { get; set; }
        public ResultTarget Target { get; set; }
        public string Message { get; set; }
        public string[] Tags { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        public Result[] Chain { get; set; }
    }

    public class ResultSource
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }

    public class ResultTarget
    {
        public string Name { get; set; }
        public SeverityState State { get; set; }
    }
}