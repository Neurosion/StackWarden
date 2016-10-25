using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;

namespace StackWarden.Monitoring.Machine
{
    public class PerformanceMonitor : Monitor
    {
        private readonly string _machineName;

        public Dictionary<SeverityState, int> CpuUsageSeverity => new Dictionary<SeverityState, int>
        {
            { SeverityState.Warning, 80 },
            { SeverityState.Error, 90 }
        };

        public Dictionary<SeverityState, int> MemoryAvailableSeverity => new Dictionary<SeverityState, int>
        {
            { SeverityState.Warning, 1500 },
            { SeverityState.Error, 500 }
        };

        public Dictionary<SeverityState, int> DiskSpaceAvailableSeverity = new Dictionary<SeverityState, int>
        {
            { SeverityState.Warning, 20 },
            { SeverityState.Error, 5 }
        };

        public Dictionary<SeverityState, int> MSMQStorageUsageSeverity = new Dictionary<SeverityState, int>
        {
            { SeverityState.Warning, 1400 },
            { SeverityState.Error, 1600 }
        };

        public PerformanceMonitor(ILog log, string machineName)
            :base(log, $"Performance monitor for {machineName}.")
        {
            _machineName = machineName.ThrowIfNullOrWhiteSpace(nameof(machineName));
        }

        protected override void Update(Result result)
        {
            CheckCpu(result);
            CheckMemory(result);
            CheckDiskSpace(result);
            CheckMSMQStorage(result);
        }

        private SeverityState GetThresholdState<T>(Dictionary<SeverityState, T> criteriaSource, Func<T, bool> compare)
        {
            foreach (var currentState in Enum.GetValues(typeof(SeverityState)).Cast<SeverityState>())
                if (criteriaSource.ContainsKey(currentState) && compare(criteriaSource[currentState]))
                    return currentState;

            return SeverityState.Normal;
        }

        private void AddDetail<T>(Result result,
                                  Func<PerformanceCounter> buildCounter,
                                  string name,
                                  string valueFormatString,
                                  Dictionary<SeverityState, T> thresholdCriteria,
                                  Func<T, float, bool> compareThreshold)
        {
            AddDetail(result, buildCounter, name, valueFormatString, x => x, thresholdCriteria, compareThreshold);
        }

        private void AddDetail<T>(Result result,
                                  Func<PerformanceCounter> buildCounter,
                                  string name,
                                  string valueFormatString,
                                  Func<float, float> transformValue,
                                  Dictionary<SeverityState, T> thresholdCriteria,
                                  Func<T, float, bool> compareThreshold)
        {
            float? counterValue = null;

            try
            {
                var counter = buildCounter();
                counterValue = counter.NextValue();

                if (transformValue != null && counterValue.HasValue)
                    counterValue = transformValue(counterValue.Value);
            }
            catch (Exception ex)
            {
                result.Message = ex.ToDetailString();
                result.Target.State = SeverityState.Error;
            }

            var formattedValue = counterValue.HasValue
                                    ? string.Format(valueFormatString, counterValue)
                                    : string.Format(valueFormatString, "N/A");
            result.Metadata.Add(name, formattedValue);

            if (!counterValue.HasValue)
                return;

            if (result.Target.State == SeverityState.Error)
                return;

            var counterState = GetThresholdState(thresholdCriteria, threshold => compareThreshold(threshold, counterValue.Value));

            if ((result.Target.State == SeverityState.Warning && counterState == SeverityState.Error) ||
                (result.Target.State == SeverityState.Normal && counterState == SeverityState.Warning))
            {
                result.Target.State = counterState;
                result.Message = $"{name} is {formattedValue}";
            }
        }

        private void CheckCpu(Result result)
        {
            AddDetail(result,
                      () => new PerformanceCounter
                      {
                          MachineName = _machineName,
                          CategoryName = "Processor",
                          CounterName = "% Processor Time",
                          InstanceName = "_Total"
                      },
                      "CPU Usage",
                      "{0:0}%",
                      CpuUsageSeverity,
                      (threshold, cpuUsage) => cpuUsage >= threshold);
        }

        private void CheckMemory(Result result)
        {
            AddDetail(result,
                      () => new PerformanceCounter
                      {
                          MachineName = _machineName,
                          CategoryName = "Memory",
                          CounterName = "Available MBytes"
                      },
                      "Memory Available",
                      "{0}MB",
                      MemoryAvailableSeverity,
                      (threshold, memoryAvailable) => memoryAvailable <= threshold);
        }

        private void CheckDiskSpace(Result result)
        {
            AddDetail(result,
                      () => new PerformanceCounter
                      {
                          MachineName = _machineName,
                          CategoryName = "LogicalDisk",
                          CounterName = "% Free Space",
                          InstanceName = "_Total"
                      },
                      "Disk Space Available",
                      "{0:0}%",
                      DiskSpaceAvailableSeverity,
                      (threshold, diskAvailable) => diskAvailable <= threshold);
        }
        
        private void CheckMSMQStorage(Result result)
        {
            AddDetail(result,
                      () => new PerformanceCounter
                      {
                          MachineName = _machineName,
                          CategoryName = "MSMQ Service",
                          CounterName = "Total bytes in all queues"
                      },
                      "MSMQ Storage Usage",
                      "{0:#}MB",
                      value => value / Constants.Units.Kibi / Constants.Units.Kibi,
                      MSMQStorageUsageSeverity,
                      (threshold, storageUsage) => threshold >= storageUsage);
        }
    }
}