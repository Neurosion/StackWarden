using StructureMap;
using log4net;
using StackWarden.Core.Persistence;

namespace StackWarden.Core.Configuration
{
    public class CoreRegistry : Registry
    {
        public CoreRegistry()
        {
            For<ILog>().Use(context => LogManager.GetLogger(context.ParentType));
            For<IRepository>().Use<MemoryRepository>()
                              .Singleton()
                              .SetProperty(x => x.MaximumSize = 100);
            For<IConfigurationReader>().Use<JsonConfigurationReader>()
                                       .Singleton();
        }
    }
}