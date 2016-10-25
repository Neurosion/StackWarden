using System;
using StackWarden.Core;

namespace StackWarden.Monitoring.ResultHandling
{
    public class ConsoleResultHandler : IResultHandler
    {
        private readonly object _handleLock = new object();

        public string Name { get; set; }

        public bool Handle(Result result)
        {
            lock (_handleLock)
            {
                switch (result.Target.State)
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
                Console.WriteLine($"Monitor: {result.Source.Type}, Target: {result.Target.Name}, State: {result.Target.State}");

                if (!string.IsNullOrWhiteSpace(result.Message))
                    Console.WriteLine($"\tDetails: {result.Message}");
            }

            return true;
        }
    }
}