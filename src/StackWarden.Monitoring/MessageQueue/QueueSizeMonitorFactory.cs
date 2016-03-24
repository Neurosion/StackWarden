using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using log4net;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.MessageQueue
{
    using MessageQueue = System.Messaging.MessageQueue;

    public class QueueSizeMonitorFactory : MonitorConfigurationDrivenFactory<QueueSizeMonitorFactory.Configuration, QueueSizeMonitor>
    {
        public class Configuration : MonitorConfiguration
        {
            public string Path { get; set; }
            public IEnumerable<string> MachineNames { get; set; }
            public string NamePattern { get; set; }
            public int? WarningThreshold { get; set; }
            public int? ErrorThreshold { get; set; }
        }

        public override IEnumerable<string> SupportedValues => new[] { "MSMQ.QueueSize" };

        public QueueSizeMonitorFactory(string configPath, IConfigurationReader configurationReader, IMonitorResultHandlerFactory resultHandlerFactory)
            :base(configPath, configurationReader, resultHandlerFactory)
        { }
        
        protected override IEnumerable<QueueSizeMonitor> BuildFromConfig(Configuration config)
        {
            if (string.IsNullOrWhiteSpace(config.NamePattern))
            {
                var instance = new QueueSizeMonitor(LogManager.GetLogger(typeof(QueueSizeMonitor)), config.Path);
                ConfigureInstance(config, instance);

                yield return instance;
            }

            config.MachineNames.ThrowIfNullOrEmpty(nameof(config.MachineNames), 
                                                   "At least one machine name must be provided when a name pattern is used.");

            var queueFormatNames = config.MachineNames
                                         .SelectMany(x => MessageQueue.GetPrivateQueuesByMachine(x).Concat(MessageQueue.GetPublicQueuesByMachine(x)))
                                         .Where(x => Regex.IsMatch(x.QueueName, config.NamePattern))
                                         .Select(x => x.FormatName);

            queueFormatNames.ThrowIfNullOrEmpty(nameof(queueFormatNames),
                                                $"No message queues found on '{string.Join(",", config.MachineNames)}' with name matching pattern '{config.NamePattern}'.");

            foreach (var currentFormatName in config.MachineNames)
            {
                var instance = new QueueSizeMonitor(LogManager.GetLogger(typeof(QueueSizeMonitor)), currentFormatName);
                ConfigureInstance(config, instance);

                yield return instance;
            }
        }

        private void ConfigureInstance(Configuration config, QueueSizeMonitor instance)
        {
            if (config.ErrorThreshold.HasValue)
                instance.ErrorThreshold = config.ErrorThreshold.Value;

            if (config.WarningThreshold.HasValue)
                instance.WarningThreshold = config.WarningThreshold.Value;
        }
    }
}