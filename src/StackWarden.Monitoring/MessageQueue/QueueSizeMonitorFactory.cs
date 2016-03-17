using System.Collections.Generic;
using log4net;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.MessageQueue
{
    public class QueueSizeMonitorFactory : MonitorConfigurationDrivenFactory<QueueSizeMonitorFactory.Configuration, QueueSizeMonitor>
    {
        public class Configuration : MonitorConfiguration
        {
            public string Path { get; set; }
            public int? WarningThreshold { get; set; }
            public int? ErrorThreshold { get; set; }
        }

        public override IEnumerable<string> SupportedValues => new[] { "MSMQ.QueueSize" };

        public QueueSizeMonitorFactory(string configPath, IConfigurationReader configurationReader, IMonitorResultHandlerFactory resultHandlerFactory)
            :base(configPath, configurationReader, resultHandlerFactory)
        { }
        
        protected override QueueSizeMonitor BuildFromConfig(Configuration config)
        {
            var instance = new QueueSizeMonitor(LogManager.GetLogger(typeof(QueueSizeMonitor)), config.Path);

            if (config.ErrorThreshold.HasValue)
                instance.ErrorThreshold = config.ErrorThreshold.Value;

            if (config.WarningThreshold.HasValue)
                instance.WarningThreshold = config.WarningThreshold.Value;

            return instance;
        }
    }
}