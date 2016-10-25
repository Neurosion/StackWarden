using System.Collections.Generic;
using System.Text.RegularExpressions;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.Log
{
    public class PatternMonitorFactory : MonitorConfigurationDrivenFactory<PatternMonitorFactory.Configuration, IMonitor>
    {
        public class Configuration : MonitorConfiguration
        {
            public string DirectoryPath { get; set; }
            public string FileNamePattern { get; set; }
            public string LogLineTimestampPattern { get; set; }
            public Dictionary<string, string> PatternSeverities { get; set; }
        }

        public PatternMonitorFactory(string configPath, IConfigurationReader configurationReader, IResultHandlerFactory resultHandlerFactory)
            :base(configPath, configurationReader, resultHandlerFactory)
        { }

        public override IEnumerable<string> SupportedTypeValues => new[] { "Log.Pattern" };

        protected override IEnumerable<IMonitor> BuildFromConfig(Configuration config)
        {
            var log = LogManager.GetLogger(typeof (PatternMonitor));
            var instance = new PatternMonitor(log, config.DirectoryPath);

            if (!string.IsNullOrWhiteSpace(config.FileNamePattern))
                instance.FileNamePattern = new Regex(config.FileNamePattern, RegexOptions.Compiled);

            if (!string.IsNullOrWhiteSpace(config.LogLineTimestampPattern))
                instance.LogLineTimestampPattern = new Regex(config.LogLineTimestampPattern, RegexOptions.Compiled);

            config.PatternSeverities.ThrowIfNullOrEmpty(nameof(config.PatternSeverities));

            foreach (var currentPair in config.PatternSeverities)
            {
                var parsedSeverity = currentPair.Value.ToEnum<SeverityState>();
                instance.PatternSeverities.Add(new Regex(currentPair.Key, RegexOptions.Compiled), parsedSeverity);
            }

            yield return instance;
        }
    }
}