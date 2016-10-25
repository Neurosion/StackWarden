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
            var monitors = monitorFactory.Build().ToArray();
#if DEBUG

            Console.WriteLine($"Configured {monitors.Length} monitors.");
            Console.WriteLine(string.Join(Environment.NewLine, monitors.Select(x => x.Name)));

            var consoleResultHandler = new ConsoleResultHandler();

            foreach (var currentMonitor in monitors)
            {
                currentMonitor.Updated += r => consoleResultHandler.Handle(r);
                currentMonitor.Start();
            }

            Console.ReadKey();

            foreach (var currentMonitor in monitors)
                currentMonitor.Stop();
#else
            var services = new ServiceBase[]
            {
                new StackWardenServerService(LogManager.GetLogger(typeof(StackWardenServerService)), monitors.ToArray())
            };

            ServiceBase.Run(services);
#endif
        }
    }
}