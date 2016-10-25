using System;
using System.Net.NetworkInformation;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Extensions;

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

        protected override void Update(Result result)
        {
            result.Target.Name = TargetAddress;

            try
            {
                var reply = new Ping().Send(TargetAddress);
                result.Metadata.Add("IP Status", reply.Status.ToExpandedString());

                if (reply.Status == IPStatus.Success)
                {
                    result.Message = $"Ping: {reply.RoundtripTime}ms";
                    result.Metadata.Add(nameof(reply.RoundtripTime).ToExpandedString(), $"{reply.RoundtripTime}ms");

                    if (reply.RoundtripTime >= ErrorThreshold)
                        result.Target.State = SeverityState.Error;
                    else if (reply.RoundtripTime >= WarningThreshold)
                        result.Target.State = SeverityState.Warning;
                }
                else
                {
                    result.Target.State = SeverityState.Error;
                    result.Message = "Ping failed.";
                }
            }
            catch (Exception ex)
            {
                result.Target.State = SeverityState.Error;
                result.Message = ex.ToDetailString();
            }
        }
    }
}