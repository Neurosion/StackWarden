namespace StackWarden.Monitoring.ResultHandling
{
    public interface IMonitorResultHandler
    {
        void Handle(MonitorResult result);
    }
}