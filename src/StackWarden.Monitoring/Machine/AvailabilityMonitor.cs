using System;
using System.Net.NetworkInformation;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Extensions;
using StackWarden.Core.Configuration;

namespace StackWarden.Monitoring.Machine
{
    public class AvailabilityMonitor : Monitor
    {
        public string TargetAddress { get; }
        public int ErrorThreshold { get; set; } = 500;
        public int WarningThreshold { get; set; } = 50;
        
        public AvailabilityMonitor(ILog log, string targetAddress)
            :base(log, $"Availability monitor for {targetAddress}.")
        {
            TargetAddress = targetAddress.ThrowIfNullOrWhiteSpace(nameof(targetAddress));
        }

        protected override void Update(MonitorResult result)
        {
            result.TargetName = TargetAddress;

            try
            {
                var reply = new Ping().Send(TargetAddress);
                result.Details.Add("IP Status", reply.Status.ToExpandedString());

                if (reply.Status == IPStatus.Success)
                {
                    result.FriendlyMessage = $"Ping: {reply.RoundtripTime}ms";
                    result.Details.Add(nameof(reply.RoundtripTime).ToExpandedString(), $"{reply.RoundtripTime}ms");

                    if (reply.RoundtripTime >= ErrorThreshold)
                        result.TargetState = SeverityState.Error;
                    else if (reply.RoundtripTime >= WarningThreshold)
                        result.TargetState = SeverityState.Warning;
                }
                else
                {
                    result.TargetState = SeverityState.Error;
                    result.FriendlyMessage = "Ping failed.";
                }
            }
            catch (Exception ex)
            {
                result.TargetState = SeverityState.Error;
                result.FriendlyMessage = ex.ToDetailString();
            }
        }
    }
}