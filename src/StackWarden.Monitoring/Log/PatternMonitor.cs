using System;
using System.Collections.Concurrent;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;

namespace StackWarden.Monitoring.Log
{
    public class PatternMonitor : Monitor
    {
        private readonly string _logDirectoryPath;
        private readonly Dictionary<string, int> _lastProcessedLine = new Dictionary<string, int>();
        private DateTime _lastUpdatedOn = DateTime.Now;

        public string FileNamePattern { get; set; }
        public Dictionary<Regex, SeverityState> PatternSeverities { get; } = new Dictionary<Regex, SeverityState>();

        public PatternMonitor(ILog log, string logDirectoryPath)
            :base(log, $"Log pattern monitor for '{logDirectoryPath}'")
        {
            _logDirectoryPath = logDirectoryPath.ThrowIfNullOrWhiteSpace(nameof(logDirectoryPath));
            Tags.Add(Constants.Categories.Log);
        }

        protected override void Update(MonitorResult result)
        {
            var filesChanged = 0;

            result.Details.Add("Root Path", _logDirectoryPath);

            try
            {
                foreach (var currentFile in Directory.GetFiles(_logDirectoryPath)
                                                     .Where(x => string.IsNullOrWhiteSpace(FileNamePattern) || Regex.IsMatch(x, FileNamePattern))
                                                     .Select(x => new FileInfo(x))
                                                     .Where(x => x.LastWriteTime >= _lastUpdatedOn))
                {
                    try
                    {
                        filesChanged++;

                        var fullFileName = currentFile.FullName;

                        if (!_lastProcessedLine.ContainsKey(fullFileName))
                            _lastProcessedLine.Add(fullFileName, 1);

                        var totalTime = new Stopwatch();
                        totalTime.Start();

                        var lineCount = 0;
                        var lineCountLock = new object();

                        Parallel.ForEach(File.ReadLines(fullFileName),
                            (currentLine, state, index) =>
                            {
                                lock (lineCountLock)
                                    lineCount++;

                                if (index < _lastProcessedLine[fullFileName])
                                    return;

                                foreach (var currentPair in PatternSeverities.Where(x => x.Key.IsMatch(currentLine)))
                                {
                                    result.Details.Add($"Line {index}", currentLine);

                                    switch (result.TargetState)
                                    {
                                        case SeverityState.Warning:
                                            if (currentPair.Value == SeverityState.Error)
                                                result.TargetState = currentPair.Value;
                                            break;
                                        case SeverityState.Normal:
                                            if (currentPair.Value == SeverityState.Error ||
                                                currentPair.Value == SeverityState.Warning)
                                                result.TargetState = currentPair.Value;
                                            break;
                                        default:
                                            continue;
                                    }
                                }
                            });

                        _lastProcessedLine[currentFile.FullName] = lineCount;

                        totalTime.Stop();
                        Debug.WriteLine($"{lineCount} in {totalTime.ElapsedMilliseconds}");
                    }
                    catch (Exception ex)
                    {
                        result.TargetState = SeverityState.Error;
                        result.FriendlyMessage = $"Failed to process file '{currentFile.FullName}'";
                        result.Details.Add("Exception", ex.ToDetailString());

                        Log.Warn(result.FriendlyMessage, ex);
                    }
                }

                _lastUpdatedOn = DateTime.Now;
            }
            catch (Exception ex)
            {
                result.TargetState = SeverityState.Error;
                result.FriendlyMessage = $"Failed to process files in  '{_logDirectoryPath}'";
                result.Details.Add("Exception", ex.ToDetailString());

                Log.Error(result.FriendlyMessage, ex);
            }

            result.Details.Add("Files Changed", filesChanged.ToString());
        }
    }
}