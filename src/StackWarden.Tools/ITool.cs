using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackWarden.Tools
{
    public interface ITool
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<string> Tags { get; } 
        int Timeout { get; set; }

        Task<ToolResult> Execute();
    }
}