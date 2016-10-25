using System;
using System.Collections.Generic;
using System.Linq;
using StackWarden.Monitoring;

namespace StackWarden.UI.Models
{
    public class Monitor
    {
        public string Name { get; set; }
        public string TargetName { get; set; }
        public string State { get; set; }
        public string Message { get; set; }
        public string Icon { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, object> Details { get; set; }
        public DateTime StaleAfter { get; set; }
        public List<Tool> Tools { get; set; }

        public static implicit operator Monitor(Result result)
        {
            var lenientTimeToLive = result.TimeToLive + Constants.Monitor.TimeToLiveLeniency;
            var projectedResultLife = result.SentOn.AddMilliseconds(lenientTimeToLive.TotalMilliseconds);

            var model = new Monitor
            {
                Name = result.Source.Name,
                TargetName = result.Target.Name,
                State = result.Target.State.ToString(),
                Message = result.Message,
                StaleAfter = projectedResultLife,
                Icon = GetIcon(result.Source.Type),
                Tags = result.Tags.ToList()
            };

            if (result.Metadata != null)
                model.Details = new Dictionary<string, object>(result.Metadata);
            
            return model;
        }

        private static string GetIcon(Type monitorType)
        {
            var namespaceName = monitorType.Namespace.Split('.').LastOrDefault();
            var foundIcon = Constants.Icons.Map.ContainsKey(namespaceName)
                                ? Constants.Icons.Map[namespaceName]
                                : null;

            return foundIcon;
        }
    }
}