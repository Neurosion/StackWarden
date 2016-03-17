using log4net;

namespace StackWarden.Monitoring.ResultHandling
{
    public class DashboardResultHandler : WebHookResultHandler
    {
        public DashboardResultHandler(ILog log, string hookAddress) 
            :base(log, hookAddress)
        {
            Headers.Add("Content-Type", "application/json");
        }

        public object JsonConvert { get; private set; }

        protected override string FormatResult(MonitorResult result)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(result);
        }
    }
}