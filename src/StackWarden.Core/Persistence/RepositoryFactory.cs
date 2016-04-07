using System;
using System.Collections.Generic;
using StackWarden.Core.Configuration;

namespace StackWarden.Core.Persistence
{
    public class RepositoryFactory : ConfigurationDrivenFactory<RepositoryFactory.Configuration, IRepository>
    {
        public class Configuration : IConfiguration
        {
            public string Type { get; set; }
        }

        public override IEnumerable<string> SupportedValues => new [] { "SQL", "Memory" };

        protected override string ConfigExtension => "repositoryconfig";

        protected RepositoryFactory(string configPath, IConfigurationReader configurationReader) 
            :base(configPath, configurationReader)
        { }

        protected override IEnumerable<IRepository> BuildFromConfig(Configuration config)
        {
            throw new NotImplementedException();
            // todo: turn this into a composite factory, like monitor factory
        }
    }
}