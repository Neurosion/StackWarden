using System.Collections.Generic;
using log4net;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.Database
{
    public class SQLPresenceMonitorFactory : MonitorConfigurationDrivenFactory<SQLPresenceMonitorFactory.Configuration, SQLPresenceMonitor>
    {
        public class Configuration : MonitorConfiguration
        {
            public string ConnectionString { get; set; }
        }

        public override IEnumerable<string> SupportedTypeValues => new[] { "Database.SQLPresence" };

        public SQLPresenceMonitorFactory(string configPath, IConfigurationReader configurationReader, IResultHandlerFactory resultHandlerFactory)
            :base(configPath, configurationReader, resultHandlerFactory)
        { }
        
        protected override IEnumerable<SQLPresenceMonitor> BuildFromConfig(Configuration config)
        {
            yield return new SQLPresenceMonitor(LogManager.GetLogger(typeof(SQLPresenceMonitor)), config.ConnectionString);
        }
    }
}