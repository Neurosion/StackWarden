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

        protected override IEnumerable<RecordingResultHandler> BuildFromConfig(Configuration config)
        {
            yield return new RecordingResultHandler(LogManager.GetLogger(typeof(RecordingResultHandler)), _repository);
        }
    }
}