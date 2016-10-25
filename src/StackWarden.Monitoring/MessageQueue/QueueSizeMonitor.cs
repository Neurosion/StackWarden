using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Messaging;
using log4net;
using StackWarden.Core;

namespace StackWarden.Monitoring.MessageQueue
{
    using System;
    using MessageQueue = System.Messaging.MessageQueue;

    public class QueueSizeMonitor : QueueMonitor
    {
        public int WarningThreshold { get; set; } = 700;
        public int ErrorThreshold { get; set; } = 1000;

        public QueueSizeMonitor(ILog log, string queuePath)
            :base(log, queuePath, $"MSMQ queue size monitor for {queuePath}")
        { }

        protected override void Update(MessageQueue queue, Result result)
        {
            var messageCounts = new Dictionary<string, int>();
            queue.MessageReadPropertyFilter.DestinationQueue = true;

            using (var messageEnumerator = queue.GetMessageEnumerator2())
            {
                while (messageEnumerator.MoveNext())
                {
                    var currentMessageName = GetMessageName(messageEnumerator.Current);

                    if (!messageCounts.ContainsKey(currentMessageName))
                        messageCounts.Add(currentMessageName, 0);

                    messageCounts[currentMessageName]++;
                }
            }

            var totalMessageCount = messageCounts.Sum(x => x.Value);
            result.Message = $"The queue contains {totalMessageCount} messages.";
            result.Metadata.Add("All Messages", totalMessageCount.ToString());

            foreach (var currentPair in messageCounts.OrderByDescending(x => x.Value))
                result.Metadata.Add(currentPair.Key, currentPair.Value.ToString());

            if (totalMessageCount >= ErrorThreshold)
                result.Target.State = SeverityState.Error;
            else if (totalMessageCount >= WarningThreshold)
                result.Target.State = SeverityState.Warning;
        }

        private string GetMessageName(Message message)
        {
            try
            {
                string rawBody = null;

                using (var reader = new StreamReader(message.BodyStream))
                {
                    rawBody = reader.ReadToEnd();
                }

                var xmlMessage = XDocument.Parse(rawBody);

                var root = xmlMessage.Root;
                var name = !string.Equals(root?.Name?.LocalName, "Messages", System.StringComparison.InvariantCultureIgnoreCase)
                                ? root?.Name?.LocalName
                                : root?.Descendants().Select(x => x.Name.LocalName).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(name))
                    return name;
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to extract message name from message (id: {message?.Id}) in queue '{message?.DestinationQueue?.FormatName}'", ex);
            }

            return message.Id;
        }
    }
}