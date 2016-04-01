using System.Configuration;
using StructureMap;

namespace StackWarden.Monitoring.Configuration
{
    public class MonitoringRegistry : Registry
    {
        public MonitoringRegistry()
        {
            RegisterResultHandlerFactories();
            RegisterMonitorFactories();

            For<IMonitorService>().Use<MonitorService>()
                                  .Singleton();
        }

        private void RegisterMonitorFactories()
        {
            var configPath = ConfigurationManager.AppSettings["Monitor.Config.Path"];

            RegisterMonitorFactory<Database.SQLPresenceMonitorFactory>(configPath);
            RegisterMonitorFactory<Http.AvailabilityMonitorFactory>(configPath);
            RegisterMonitorFactory<Machine.AvailabilityMonitorFactory>(configPath);
            RegisterMonitorFactory<Machine.PerformanceMonitorFactory>(configPath);
            RegisterMonitorFactory<MessageQueue.QueueSizeMonitorFactory>(configPath);
            RegisterMonitorFactory<Service.StateMonitorFactory>(configPath);
            RegisterMonitorFactory<Log.PatternMonitorFactory>(configPath);

            ForConcreteType<CompositeMonitorFactory>().Configure
                                                      .Singleton()
                                                      .Ctor<string>("configPath").Is(configPath);
        }

        private void RegisterMonitorFactory<T>(string configPath)
            where T: class, IMonitorFactory
        {
            For<IMonitorFactory>().Add<T>()
                                  .Ctor<string>("configPath").Is(configPath)
                                  .Ctor<IMonitorResultHandlerFactory>().Is(c => c.GetInstance<CompositeResultHandlerFactory>("DefaultResultHandlerFactory"));
        }

        private void RegisterResultHandlerFactories()
        {
            var configPath = ConfigurationManager.AppSettings["MonitorResultHandler.Config.Path"];
            
            RegisterResultHandlerFactory<ResultHandling.EmailResultHandlerFactory>(configPath);
            RegisterResultHandlerFactory<ResultHandling.DashboardResultHandlerFactory>(configPath);
            RegisterResultHandlerFactory<ResultHandling.SlackResultHandlerFactory>(configPath);

            ForConcreteType<CompositeResultHandlerFactory>().Configure
                                                            .Singleton()
                                                            .Named("DefaultResultHandlerFactory")
                                                            .Ctor<string>("configPath").Is(configPath);
        }

        private void RegisterResultHandlerFactory<T>(string configPath)
            where T : class, IMonitorResultHandlerFactory
        {
            For<IMonitorResultHandlerFactory>().Add<T>().Ctor<string>("configPath").Is(configPath);
        }
    }
}