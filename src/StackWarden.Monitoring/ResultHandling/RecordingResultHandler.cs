using System;
using log4net;
using StackWarden.Core.Extensions;
using StackWarden.Core.Persistence;

namespace StackWarden.Monitoring.ResultHandling
{
    public class RecordingResultHandler : IResultHandler
    {
        private readonly ILog _log;
        private readonly IRepository _repository;

        public string Name { get; set; }

        public RecordingResultHandler(ILog log, IRepository repository)
        {
            _log = log.ThrowIfNull(nameof(log));
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        public bool Handle(Result result)
        {
            try
            {
                _repository.Save(result);

                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to record result.", ex);
            }

            return false;
        }
    }
}