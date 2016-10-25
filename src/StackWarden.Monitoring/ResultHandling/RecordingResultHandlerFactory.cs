using System.Collections.Generic;
using System.Linq;
using log4net;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;
using StackWarden.Core.Persistence;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.ResultHandling
{
    public class RecordingResultHandlerFactory : ResultHandlerConfigurationDrivenFactory<RecordingResultHandlerFactory.Configuration, RecordingResultHandler>
    {
        private readonly RepositoryFactory _repositoryFactory;

        public class Configuration : IConfiguration
        {
            public string Type { get; set; }
            public string[] Repositories { get; set; }
        }

        public override IEnumerable<string> SupportedTypeValues => new[] { "Record" };

        public RecordingResultHandlerFactory(string configPath, IConfigurationReader configurationReader, RepositoryFactory repositoryFactory)
            : base(configPath, configurationReader)
        {
            _repositoryFactory = repositoryFactory.ThrowIfNull(nameof(repositoryFactory));
        }
        
        protected override IEnumerable<RecordingResultHandler> BuildFromConfig(Configuration config)
        {
            config.Repositories.ThrowIfNullOrEmpty(nameof(config.Repositories));

            var repositories = config.Repositories.SelectMany(x => _repositoryFactory.Build(x));

            repositories.ThrowIfNullOrEmpty(nameof(repositories), $"No repository instances could be resolved from repositories: {string.Join(",", config.Repositories)}");

            foreach (var currentRepository in repositories)
                yield return new RecordingResultHandler(LogManager.GetLogger(typeof(RecordingResultHandler)), currentRepository);
        }
    }
}