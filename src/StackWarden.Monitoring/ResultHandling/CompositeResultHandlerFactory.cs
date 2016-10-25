using System.Linq;
using System.Collections.Generic;
using log4net;
using StackWarden.Core.Extensions;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.ResultHandling
{
    public class CompositeResultHandlerFactory : ResultHandlerConfigurationDrivenFactory<CompositeResultHandlerFactory.Configuration, CompositeResultHandler>, ICompositeResultHandlerFactory
    {
        public class Configuration : IConfiguration
        {
            public string Type { get; set; }
            public bool ShouldStopProcessingOnSuccess { get; set; }
            public string[] ResultHandlers { get; set; }
        }

        private readonly IResultHandlerFactory _resultHandlerFactory;

        public override IEnumerable<string> SupportedTypeValues => new[] { "Composite" };

        public CompositeResultHandlerFactory(string configPath, IConfigurationReader configurationReader, IResultHandlerFactory resultHandlerFactory)
            :base (configPath, configurationReader)
        {
            _resultHandlerFactory = resultHandlerFactory.ThrowIfNull(nameof(resultHandlerFactory));
        }

        protected override IEnumerable<CompositeResultHandler> BuildFromConfig(Configuration config)
        {
            var children = config.ResultHandlers
                                 .ThrowIfNullOrEmpty(nameof(config.ResultHandlers))
                                 .SelectMany(_resultHandlerFactory.Build)
                                 .ToArray();

            yield return new CompositeResultHandler(LogManager.GetLogger(typeof(CompositeResultHandler)), children)
            {
                ShouldStopProcessingOnSuccess = config.ShouldStopProcessingOnSuccess
            };
        }
    }
}