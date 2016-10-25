using StackWarden.Core.Configuration;
using StackWarden.Monitoring.ResultHandling;

namespace StackWarden.Monitoring.Configuration
{
    public interface IResultHandlerFactory : IFactory<IResultHandler>
    {
    }
}