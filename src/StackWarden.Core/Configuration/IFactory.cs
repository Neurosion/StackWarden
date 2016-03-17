using System.Collections.Generic;

namespace StackWarden.Core.Configuration
{
    public interface IFactory<out T>
    {
        IEnumerable<string> SupportedValues { get; }
        T Build(string name, bool useExistingInstance = false);
        IEnumerable<T> BuildAll();
    }
}