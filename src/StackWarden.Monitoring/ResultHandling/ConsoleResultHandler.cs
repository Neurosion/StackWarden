using System;
using StackWarden.Core;

namespace StackWarden.Monitoring.ResultHandling
{
    public class ConsoleResultHandler : IMonitorResultHandler
    {
        private readonly object _handleLock = new object();

        public void Handle(MonitorResult result)
        {
            lock (_handleLock)
            {
                switch (result.TargetState)
                {
                    case SeverityState.Normal:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case SeverityState.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case SeverityState.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                }
                Console.WriteLine($"Monitor: {result.SourceType}, Target: {result.TargetName}, State: {result.TargetState}");

                if (!string.IsNullOrWhiteSpace(result.FriendlyMessage))
                    Console.WriteLine($"\tDetails: {result.FriendlyMessage}");
            }
        }
    }
}