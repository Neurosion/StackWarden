using System;
using log4net;
using StackWarden.Core.Extensions;
using System.Net.Mail;
using System.Collections.Generic;

namespace StackWarden.Monitoring.ResultHandling
{
    public class EmailResultHandler : IMonitorResultHandler
    {
        private readonly ILog _log;
        private readonly SmtpClient _smtpClient;
        private readonly string _sender;
        private readonly string _recipients;

        public Func<MonitorResult, string> SubjectFormatter { get; set; } = DefaultSubjectFormatter;
        public Func<MonitorResult, string> BodyFormatter { get; set; } = DefaultBodyFormatter;

        public EmailResultHandler(ILog log, SmtpClient smtpClient, string sender, params string[] recipients)
        {
            _log = log.ThrowIfNull(nameof(log));
            _smtpClient = smtpClient.ThrowIfNull(nameof(smtpClient));
            _sender = sender.ThrowIfNullOrWhiteSpace(nameof(sender));
            _recipients = string.Join(";", recipients.ThrowIfNullOrEmpty(nameof(recipients)));
        }

        private static string DefaultSubjectFormatter(MonitorResult result)
        {
            return $"{result.SourceType}: {result.TargetName}";
        }

        private static string DefaultBodyFormatter(MonitorResult result)
        {
            var components = new List<string>
            {
                $"Monitor: {result.SourceType}",
                $"Target: {result.TargetName}",
                $"State: {result.TargetState}"
            };

            if (!string.IsNullOrWhiteSpace(result.FriendlyMessage))
                components.Add($"Details: {result.FriendlyMessage}");

            var body = string.Join(Environment.NewLine, components);

            return body;
        }

        public void Handle(MonitorResult result)
        {
            try
            {
                var subject = SubjectFormatter.ThrowIfNull(nameof(SubjectFormatter)).Invoke(result);
                var body = BodyFormatter.ThrowIfNull(nameof(BodyFormatter)).Invoke(result);
                var message = new MailMessage(_sender, _recipients, subject, body);

                _smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to send email.", ex);
            }
        }
    }
}