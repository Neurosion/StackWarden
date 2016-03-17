using System;
using System.IO;
using Newtonsoft.Json;
using StackWarden.Core.Extensions;

namespace StackWarden.Core.Configuration
{
    public class JsonConfigurationReader : IConfigurationReader
    {
        public T Read<T>(string path)
        {
            var definition = Activator.CreateInstance<T>();
            var config = Read(path, definition);

            return config;
        }

        public T Read<T>(string path, T definition)
        {
            path.ThrowIfNullOrWhiteSpace(nameof(path))
                .ThrowIf<FileNotFoundException, string>(!File.Exists(path), $"No file found at '{path}'");

            var rawConfiguration = File.ReadAllText(path);
            var config = JsonConvert.DeserializeObject<T>(rawConfiguration);

            return config;
        }
    }
}