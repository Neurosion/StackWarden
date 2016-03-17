using System;
using System.IO;
using log4net;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;

namespace StackWarden.Monitoring.Log
{
    public class PatternMonitor : Monitor
    {
        private readonly string _logFilePath;

        // todo: allow specification of patterns and severity levels per pattern

        public PatternMonitor(ILog log, string name, string logFilePath)
            :base(log, name)
        {
            _logFilePath = logFilePath.ThrowIfNullOrWhiteSpace(nameof(logFilePath))
                                      .ThrowIf<FileNotFoundException, string>(!File.Exists(logFilePath), $"No log file exists at '{logFilePath}'");
            Tags.Add(Constants.Categories.Log);
        }

        protected override void Update(MonitorResult result)
        {
            throw new NotImplementedException();
        }
    }
}