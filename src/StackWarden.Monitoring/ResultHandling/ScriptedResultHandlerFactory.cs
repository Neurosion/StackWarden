using System.Collections.Generic;
using log4net;
using StackWarden.Core.Configuration;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring.ResultHandling
{
    public class ScriptedResultHandlerFactory : ResultHandlerConfigurationDrivenFactory<ScriptedResultHandlerFactory.Configuration, ScriptedResultHandler>
    {
        public class Configuration : IConfiguration
        {
            public string Type { get; set; }
            public string Script { get; set; }
        }

        public override IEnumerable<string> SupportedTypeValues => new[] { "Scripted" };

        public ScriptedResultHandlerFactory(string configPath, IConfigurationReader configurationReader)
            : base(configPath, configurationReader)
        { }

        protected override IEnumerable<ScriptedResultHandler> BuildFromConfig(Configuration config)
        {
            yield return new ScriptedResultHandler(LogManager.GetLogger(typeof(ScriptedResultHandler)), config.Script);
        }
    }
}