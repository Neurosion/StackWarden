using StackWarden.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackWarden.Tools
{
    public abstract class Tool : ITool
    {
        public string Name { get; }
        public string Description { get; }
        public IEnumerable<string> Tags { get; protected set; }
        public int Timeout { get; set; } = Configuration.Constants.DefaultTimeout;

        protected Tool(string name, string description)
        {
            Name = name.ThrowIfNullOrWhiteSpace(nameof(name)); ;
            Description = description;
        }

        public virtual Task<ToolResult> Execute()
        {
            return Task.Run(() => 
            {
                var result = new ToolResult();

                try
                {
                    result = ExecuteBody();
                }
                catch (Exception ex)
                {
                    result.DidSucceed = false;
                    result.State = Core.SeverityState.Error;
                    result.Metadata.Add("Exception", ex.ToDetailString());
                }

                return result;
            });
        }

        protected abstract ToolResult ExecuteBody();
    }
}