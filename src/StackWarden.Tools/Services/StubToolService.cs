using System.Collections.Generic;
using System.IO;
using StackWarden.Core.Extensions;

namespace StackWarden.Tools.Services
{
    public class StubToolService : IToolService
    {
        private const string TypeFileFilter = "*.dll";

        private string _toolPath;
        private List<ITool> _tools = new List<ITool>();
        private bool _hasLoaded = false;

        public StubToolService(string toolPath)
        {
            _toolPath = toolPath.ThrowIfNullOrWhiteSpace(nameof(toolPath))
                                .ThrowIf<DirectoryNotFoundException, string>(!Directory.Exists(toolPath),
                                                                             $"The path '{toolPath}' does not exist.");
        }

        public IEnumerable<ITool> Get()
        {
            LoadConfigurations();

            return _tools;
        }

        private void LoadConfigurations()
        {
            if (_hasLoaded)
                return;

            _tools.Clear();

            //var loadedConfigurations = ToolConfiguration.FromDirectory(_toolPath, false);
            //_tools.AddRange(loadedConfigurations);

            _hasLoaded = true;
        }
    }
}