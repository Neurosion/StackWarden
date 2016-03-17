using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Linq;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;

namespace StackWarden.Tools.Service
{
    public class StartTool : Tool
    {
        private static ServiceControllerStatus[] ErrorStatuses =
        {
             ServiceControllerStatus.Paused,
             ServiceControllerStatus.PausePending,
             ServiceControllerStatus.Stopped,
             ServiceControllerStatus.StopPending
        };

        private static ServiceControllerStatus[] WarningStatuses =
        {
            ServiceControllerStatus.ContinuePending,
            ServiceControllerStatus.StartPending
        };

        private readonly string _machineName;
        private readonly string _serviceName;

        public StartTool(string machineName, string serviceName)
            :base("Start", $"Starts the {serviceName} service on {machineName}.")
        {
            _machineName = machineName.ThrowIfNullOrWhiteSpace(nameof(machineName));
            _serviceName = serviceName.ThrowIfNullOrWhiteSpace(nameof(serviceName));
            Tags = new[] { Constants.Categories.Service };
        }
        
        protected override ToolResult ExecuteBody()
        {
            using (var service = new ServiceController(_serviceName, _machineName))
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(Timeout));

                var result = new ToolResult
                {
                    DidSucceed = true,
                    State = ErrorStatuses.Contains(service.Status)
                                ? Core.SeverityState.Error
                                : WarningStatuses.Contains(service.Status)
                                    ? Core.SeverityState.Warning
                                    : Core.SeverityState.Normal
                };
                

                return result;
            };
        }
    }
}