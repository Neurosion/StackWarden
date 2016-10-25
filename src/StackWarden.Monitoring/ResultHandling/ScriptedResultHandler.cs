using System;
using log4net;
using StackWarden.Core.Extensions;
using System.Management.Automation;

namespace StackWarden.Monitoring.ResultHandling
{
    public class ScriptedResultHandler : ResultHandler
    {
        private readonly string _script;

        public ScriptedResultHandler(ILog log, string script)
            :base(log)
        {
            _script = script.ThrowIfNullOrWhiteSpace(nameof(script));
        }

        protected override Result HandleAndGetResult(Result result)
        {
            var powershell = PowerShell.Create();
            var pipeline = powershell.Runspace.CreatePipeline();
            pipeline.Commands.AddScript(_script, true);
            
            pipeline.Input.Write(result);
            var scriptResult = pipeline.Invoke();

            if (pipeline.HasErrors())
            {
                // build exception, throw
            }


            throw new NotImplementedException();
        }
    }
}