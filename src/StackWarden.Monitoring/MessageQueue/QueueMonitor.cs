using System;
using System.Messaging;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Extensions;
using StackWarden.Core.Configuration;

namespace StackWarden.Monitoring.MessageQueue
{
    using MessageQueue = System.Messaging.MessageQueue;

    public abstract class QueueMonitor : Monitor
    {
        private readonly string _queuePath;

        protected QueueMonitor(ILog log, string queuePath, string name)
            :base(log, name)
        {
            _queuePath = queuePath.ThrowIfNullOrWhiteSpace(nameof(queuePath));
        }

        protected override void Update(Result result)
        {
            result.Target.Name = _queuePath;

            try
            {
                using (var queue = new MessageQueue(_queuePath, QueueAccessMode.Peek))
                    Update(queue, result);
            }
            catch (Exception ex)
            {
                Log.Error("Update failed.", ex);
                result.Target.State = SeverityState.Error;
                result.Message = ex.ToDetailString();
            }
        }

        protected abstract void Update(MessageQueue queue, Result result);
    }
}