using System;
using log4net;
using StackWarden.Core.Extensions;
using StackWarden.Core.Persistence;

namespace StackWarden.Monitoring.ResultHandling
{
    public class RecordingResultHandler : IMonitorResultHandler
    {
        private readonly ILog _log;
        private readonly IRepository _repository;

        public RecordingResultHandler(ILog log, IRepository repository)
        {
            _log = log.ThrowIfNull(nameof(log));
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        public void Handle(MonitorResult result)
        {
            try
            {
                _repository.Save(result);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to record result.", ex);
            }
        }
    }
}