using System;
using System.Collections.Generic;
using System.Linq;
using StackWarden.Core.Extensions;

namespace StackWarden.Tools
{
    public class CompositeTool : Tool
    {
        private List<ITool> _tools;

        public CompositeTool(string name, string description, params ITool[] tools)
            :base(name, description)
        {
            tools.ThrowIfNull(nameof(tools))
                 .ThrowIf<ArgumentException, ITool[]>(!tools.Any(), "At least one tool must be provided.");
            _tools = new List<ITool>(tools);
        }

        protected override ToolResult ExecuteBody()
        {
            var aggregateResult = new ToolResult
            {
                DidSucceed = true
            };

            for (var i = 0; aggregateResult.DidSucceed && i < _tools.Count; i++)
            {
                var result = _tools[i].Execute().Result;

                aggregateResult.DidSucceed = aggregateResult.DidSucceed && result.DidSucceed;

                foreach (var currentPair in result.Metadata)
                {
                    if (!aggregateResult.Metadata.ContainsKey(currentPair.Key))
                        aggregateResult.Metadata.Add(currentPair.Key, currentPair.Value);
                    else
                        aggregateResult.Metadata[currentPair.Key] += Environment.NewLine + currentPair.Value;
                }
            }

            return aggregateResult;
        }
    }
}