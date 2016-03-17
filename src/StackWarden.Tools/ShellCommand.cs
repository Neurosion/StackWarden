using System.Diagnostics;
using StackWarden.Core;
using StackWarden.Core.Extensions;

namespace StackWarden.Tools
{
    public class ShellCommand : Tool
    {
        private readonly string _command;
        private readonly string[] _arguments;

        public ShellCommand(string name, string description, string command, params string[] arguments)
            :base(name, description)
        {
            _command = command.ThrowIfNullOrWhiteSpace(nameof(command));
            _arguments = arguments;
        }

        protected override ToolResult ExecuteBody()
        {
            var commandArguments = string.Join(" ", _arguments ?? new string[0]);
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C {_command} {commandArguments}"
                }
            };

            if (process.Start())
                process.WaitForExit(Timeout);

            var output = process.StandardOutput.ReadToEnd();
            var errorOutput = process.StandardError.ReadToEnd();
            var didSucceed = string.IsNullOrWhiteSpace(errorOutput);
            var result = new ToolResult
            {
                DidSucceed = didSucceed,
                State = didSucceed ? SeverityState.Normal : SeverityState.Error,
                Metadata =
                    {
                        { "Exit Code", process.ExitCode.ToString() },
                        { "Output", output },
                        { "Errors", errorOutput }
                    }
            };

            return result;
        }
    }
}