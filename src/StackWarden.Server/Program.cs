using System;
using System.Linq;
using System.Configuration;
using System.ServiceProcess;
using log4net;
using StructureMap;
using StackWarden.Monitoring.Configuration;
using StackWarden.Monitoring.ResultHandling;

namespace StackWarden.Server
{
    internal static class Program
    {
        private static void Main()
        {
            var container = new Container(x =>
            {
                x.Scan(s =>
                {
                    s.Assembly("StackWarden.Core");
                    s.Assembly("StackWarden.Monitoring");
                    s.LookForRegistries();
                });
            });
            var monitorFactory = container.GetInstance<CompositeMonitorFactory>();
            var monitors = monitorFactory.BuildAll().ToArray();
#if DEBUG

            Console.WriteLine($"Configured {monitors.Count()} monitors.");
            var consoleResultHandler = new ConsoleResultHandler();

            foreach (var currentMonitor in monitors)
            {
                currentMonitor.Updated += consoleResultHandler.Handle;
                currentMonitor.Start();
            }

            Console.ReadKey();

            foreach (var currentMonitor in monitors)
                currentMonitor.Stop();
#else
            var services = new ServiceBase[]
            {
                new ServerService(LogManager.GetLogger(typeof(ServerService)), monitors.ToArray())
            };

            ServiceBase.Run(services);
#endif
        }
    }
}