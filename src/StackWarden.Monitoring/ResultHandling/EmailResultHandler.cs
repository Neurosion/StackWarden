using System;
using log4net;
using StackWarden.Core.Extensions;
using System.Net.Mail;
using System.Collections.Generic;

namespace StackWarden.Monitoring.ResultHandling
{
    public class EmailResultHandler : IResultHandler
    {
        private readonly ILog _log;
        private readonly SmtpClient _smtpClient;
        private readonly string _sender;
        private readonly string _recipients;

        private Func<Result, string> _subjectFormatter;
        private Func<Result, string> _bodyFormatter;

        public string Name { get; set; }

        public Func<Result, string> SubjectFormatter
        {
            get { return _subjectFormatter ?? DefaultSubjectFormatter; }
            set { _subjectFormatter = value; }
        }
        public Func<Result, string> BodyFormatter
        {
            get { return _bodyFormatter ?? DefaultBodyFormatter; }
            set { _bodyFormatter = value; }
        }
        
        public EmailResultHandler(ILog log, SmtpClient smtpClient, string sender, params string[] recipients)
        {
            _log = log.ThrowIfNull(nameof(log));
            _smtpClient = smtpClient.ThrowIfNull(nameof(smtpClient));
            _sender = sender.ThrowIfNullOrWhiteSpace(nameof(sender));
            _recipients = string.Join(";", recipients.ThrowIfNullOrEmpty(nameof(recipients)));
        }

        private static string DefaultSubjectFormatter(Result result)
        {
            return $"{result.Source.Type}: {result.Target.Name}";
        }

        private static string DefaultBodyFormatter(Result result)
        {
            var components = new List<string>
            {
                $"Monitor: {result.Source.Type}",
                $"Target: {result.Target.Name}",
                $"State: {result.Target.State}"
            };

            if (!string.IsNullOrWhiteSpace(result.Message))
                components.Add($"Details: {result.Message}");

            var body = string.Join(Environment.NewLine, components);

            return body;
        }

        public bool Handle(Result result)
        {
            try
            {
                var subject = SubjectFormatter.Invoke(result);
                var body = BodyFormatter.Invoke(result);
                var message = new MailMessage(_sender, _recipients, subject, body);

                _smtpClient.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to send email.", ex);
            }

            return false;
        }
    }
}