using System.Collections.Generic;

namespace StackWarden.Tools.Services
{
    public interface IToolService
    {
        IEnumerable<ITool> Get();
    }
}