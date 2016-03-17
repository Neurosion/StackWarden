using System.Collections.Generic;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.ResultHandling
{
    public class SlackResultHandlerFactory : ResultHandlerConfigurationDrivenFactory<SlackResultHandlerFactory.Configuration, SlackResultHandler>
    {
        public class Configuration : IConfiguration
        {
            public string Type { get; set; }
            public string HookAddress { get; set; }
            public string Username { get; set; }
            public string Channel { get; set; }
            public string Icon { get; set; }
            public SeverityState? NotificationThreshold { get; set; }
        }

        public override IEnumerable<string> SupportedValues => new[] { "Slack" };

        public SlackResultHandlerFactory(string configPath, IConfigurationReader configurationReader) 
            :base(configPath, configurationReader)
        { }

        protected override SlackResultHandler BuildFromConfig(Configuration config)
        {
            var instance = new SlackResultHandler(LogManager.GetLogger(typeof(SlackResultHandler)), config.HookAddress);

            if (!string.IsNullOrWhiteSpace(config.Username))
                instance.Username = config.Username;

            if (!string.IsNullOrWhiteSpace(config.Channel))
                instance.Channel = config.Channel;

            if (!string.IsNullOrWhiteSpace(config.Icon))
                instance.Icon = config.Icon;

            if (config.NotificationThreshold.HasValue)
                instance.NotificationThreshold = config.NotificationThreshold.Value;

            return instance;
        }
    }
}