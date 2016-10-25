using System;
using System.Linq;
using System.ServiceProcess;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;
using System.Collections.Generic;

namespace StackWarden.Monitoring.Service
{
    public class StateMonitor : Monitor
    {
        private static readonly Dictionary<ServiceControllerStatus, string> StateMessageSuffixMap = new Dictionary<ServiceControllerStatus, string>
        {
            { ServiceControllerStatus.Paused, "paused" },
            { ServiceControllerStatus.PausePending, "pausing" },
            { ServiceControllerStatus.Stopped, "stopped" },
            { ServiceControllerStatus.StopPending, "stopping" },
            { ServiceControllerStatus.ContinuePending, "continuing" },
            { ServiceControllerStatus.StartPending, "starting" },
            { ServiceControllerStatus.Running, "running" }
        };

        private static readonly ServiceControllerStatus[] ErrorStatuses =
        {
            ServiceControllerStatus.Paused,
            ServiceControllerStatus.PausePending,
            ServiceControllerStatus.Stopped,
            ServiceControllerStatus.StopPending,
        };
        private static readonly ServiceControllerStatus[] WarningStatuses =
        {
            ServiceControllerStatus.ContinuePending,
            ServiceControllerStatus.StartPending
        };

        private readonly string _machineName;
        private readonly string _serviceName;

        
        public StateMonitor(ILog log, string machineName, string serviceName)
            :base(log, $"Service state monitor for {serviceName} on {machineName}.")
        {
            _machineName = machineName.ThrowIfNullOrWhiteSpace(nameof(machineName));
            _serviceName = serviceName.ThrowIfNullOrWhiteSpace(nameof(serviceName));
        }

        protected override void Update(Result result)
        {
            result.Target.Name = $"{_serviceName} on {_machineName}";
            result.Metadata.Add("Service", _serviceName);
            result.Metadata.Add("Machine", _machineName);

            try
            {
                var serviceController = new ServiceController(_serviceName, _machineName);
                var serviceStatus = serviceController.Status;

                result.Metadata.Add("Status", serviceStatus.ToExpandedString());
             
                if (ErrorStatuses.Contains(serviceStatus))
                    result.Target.State = SeverityState.Error;
                else if (WarningStatuses.Contains(serviceStatus))
                    result.Target.State = SeverityState.Warning;

                 result.Message = $"The service is {StateMessageSuffixMap[serviceStatus]}.";
            }
            catch (Exception ex)
            {
                Log.Error("Failed to update.", ex);

                result.Target.State = SeverityState.Error;
                result.Message = ex.ToDetailString();
            }
        }
    }
}