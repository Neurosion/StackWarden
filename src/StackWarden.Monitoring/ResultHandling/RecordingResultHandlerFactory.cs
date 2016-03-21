using System.Collections.Generic;
using log4net;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;
using StackWarden.Core.Persistence;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.ResultHandling
{
    public class RecordingResultHandlerFactory : ResultHandlerConfigurationDrivenFactory<RecordingResultHandlerFactory.Configuration, RecordingResultHandler>
    {
        private readonly IRepository _repository;

        public class Configuration : IConfiguration
        {
            public string Type { get; set; }
        }

        public RecordingResultHandlerFactory(string configPath, IConfigurationReader configurationReader, IRepository repository)
            : base(configPath, configurationReader)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        public override IEnumerable<string> SupportedValues => new [] { "Record" };

        protected override RecordingResultHandler BuildFromConfig(Configuration config)
        {
            var log = LogManager.GetLogger(typeof (RecordingResultHandler));
            var handler = new RecordingResultHandler(log, _repository);

            return handler;
        }
    }
}