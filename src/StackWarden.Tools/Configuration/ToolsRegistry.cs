using System.Configuration;
using StackWarden.Tools.Services;
using StructureMap;

namespace StackWarden.Tools.Configuration
{
    public class ToolsRegistry: Registry
    {
        public ToolsRegistry()
        {
            For<IToolService>().Use<StubToolService>()
                               .Singleton()
                               .Ctor<string>("toolPath").Is(ConfigurationManager.AppSettings["Tools.Path"]);
        }
    }
}