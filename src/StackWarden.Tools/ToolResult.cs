using System.Collections.Generic;
using StackWarden.Core;

namespace StackWarden.Tools
{
    public class ToolResult
    {
        public bool DidSucceed { get; set; }
        public SeverityState State { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}