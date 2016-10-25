using System.Collections.Generic;

namespace StackWarden.Monitoring.ResultHandling
{
    public interface ICompositeResultHandlerFactory
    {
        IEnumerable<string> SupportedTypeValues { get; }
    }
}