using System;
using System.Collections.Generic;
using log4net;
using Newtonsoft.Json;
using StackWarden.Core;

namespace StackWarden.Monitoring.ResultHandling
{
    public class SlackResultHandler : WebHookResultHandler
    {
        private static class MessageConstants
        {
            public const string Text = "text";
            public const string Username = "username";
            public const string Icon = "icon_emoji";
            public const string Channel = "channel";
            public const string MarkdownToggle = "mrkdwn"; // true, false
        }

        private static class AttachmentContstants
        {
            public const string Container = "attachments";
            public const string Title = "title";
            public const string PreText = "pretext";
            public const string Text = "text";
            public const string MarkdownInFields = "mrkdwn_in"; // ["pretext", "text", "field"]
        }

        public string Username { get; set; }
        public string Channel { get; set; }
        public string Icon { get; set; }
        public SeverityState NotificationThreshold { get; set; } = SeverityState.Warning;

        public SlackResultHandler(ILog log, string hookAddress)
            : base(log, hookAddress)
        {
            Headers.Add("Content-Type", "application/json");
        }

        protected override bool ShouldHandle(Result result)
        {
            switch (NotificationThreshold)
            {
                case SeverityState.Normal:
                    return true;
                case SeverityState.Warning:
                    return result.Target.State == SeverityState.Warning ||
                           result.Target.State == SeverityState.Error;
                case SeverityState.Error:
                    return result.Target.State == SeverityState.Error;
                default:
                    return false;
            }
        }

        protected override string FormatResult(Result result)
        {
            var slackData = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(Username))
                slackData.Add(MessageConstants.Username, Username);

            if (!string.IsNullOrWhiteSpace(Channel))
                slackData.Add(MessageConstants.Channel, Channel);

            if (!string.IsNullOrWhiteSpace(Icon))
                slackData.Add(MessageConstants.Icon, Icon);

            slackData.Add(MessageConstants.Text, string.Join(Environment.NewLine, new[]
            {
                $"Monitor: {result.Source.Name}", $"Target: {result.Target.Name}", $"State: {result.Target.State}", result.Message
            }));

            var serializedMessage = JsonConvert.SerializeObject(slackData);

            return serializedMessage;
        }
    }
}