using System.Collections.Generic;
using System.Net.Mail;
using log4net;
using StackWarden.Core.Extensions;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.ResultHandling
{
    public class EmailResultHandlerFactory : ResultHandlerConfigurationDrivenFactory<EmailResultHandlerFactory.Configuration, EmailResultHandler>
    {
        public class Configuration : IConfiguration
        {
            public string Type { get; set; }
            public string Sender { get; set; }
            public string[] Recipients { get; set; }
        }

        private readonly SmtpClient _smtpClient;

        public override IEnumerable<string> SupportedValues => new[] { "Email" };
        
        public EmailResultHandlerFactory(string configPath, IConfigurationReader configurationReader, SmtpClient smtpClient)
            :base(configPath, configurationReader)
        {
            _smtpClient = smtpClient.ThrowIfNull(nameof(smtpClient));
        }

        protected override EmailResultHandler BuildFromConfig(Configuration config)
        {
            var instance = new EmailResultHandler(LogManager.GetLogger(typeof(EmailResultHandler)), _smtpClient, config.Sender, config.Recipients);

            return instance;
        }
    }
}