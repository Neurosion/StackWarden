using System;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackWarden.Core.Extensions;

namespace StackWarden.Tools
{
    public class PowerShellCommand : Tool
    {
        private readonly string _command;
        private readonly string[] _arguments;

        public bool IsScript { get; private set; }

        public PowerShellCommand(string name, string description, string command, string[] arguments, bool isScript = false)
            :base(name, description)
        {
            _command = command.ThrowIfNullOrWhiteSpace(nameof(command));
            _arguments = arguments;
            IsScript = isScript;
        }

        protected override ToolResult ExecuteBody()
        {
            var powershell = PowerShell.Create();
            var pipeline = powershell.Runspace.CreatePipeline();

            if (IsScript)
                pipeline.Commands.AddScript(_command);
            else
                pipeline.Commands.Add(_command);

            var shellResult = pipeline.Invoke();
            var builder = new StringBuilder();
            var result = new ToolResult();

            if (pipeline.HasErrors())
            {
                result.DidSucceed = false;

                var errors = pipeline.GetErrors()?
                                     .Select(x => x.ErrorDetails)
                                     .Where(x => x != null);

                if (errors != null)
                    result.Metadata.Add("Errors", JsonConvert.SerializeObject(errors));
            }

            result.Metadata.Add("Details", JsonConvert.SerializeObject(shellResult));

            return result;
        }
    }
}