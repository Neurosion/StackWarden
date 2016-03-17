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

        public override IEnumerable<string> SupportedValues => new[] { "Database.SQLPresence" };

        public SQLPresenceMonitorFactory(string configPath, IConfigurationReader configurationReader, IMonitorResultHandlerFactory resultHandlerFactory)
            :base(configPath, configurationReader, resultHandlerFactory)
        { }
        
        protected override SQLPresenceMonitor BuildFromConfig(Configuration config)
        {
            var instance = new SQLPresenceMonitor(LogManager.GetLogger(typeof(SQLPresenceMonitor)), config.ConnectionString);

            return instance;
        }
    }
}