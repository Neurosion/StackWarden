using System;
using System.IO;
using System.Collections.Generic;
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
        private DateTime _lastUpdatedOn = DateTime.MinValue;

        public Regex FileNamePattern { get; set; }
        public Regex LogLineTimestampPattern { get; set; }
        public Dictionary<Regex, SeverityState> PatternSeverities { get; } = new Dictionary<Regex, SeverityState>();

        public PatternMonitor(ILog log, string logDirectoryPath)
            :base(log, $"Log pattern monitor for '{logDirectoryPath}'")
        {
            _logDirectoryPath = logDirectoryPath.ThrowIfNullOrWhiteSpace(nameof(logDirectoryPath));
        }
        
        protected override void Update(MonitorResult result)
        {
            var filesChanged = 0;

            result.Details.Add("Root Path", _logDirectoryPath);

            try
            {
                foreach (var currentFile in GetFilesSinceLastUpdate())
                {
                    try
                    {
                        filesChanged++;

                        var fullFileName = currentFile.FullName;

                        if (!_lastProcessedLine.ContainsKey(fullFileName))
                            _lastProcessedLine.Add(fullFileName, 1);

                        var lineCount = 0;
                        var lineCountLock = new object();

                        Parallel.ForEach(File.ReadLines(fullFileName),
                            (currentLine, state, index) =>
                            {
                                lock (lineCountLock)
                                    lineCount++;

                                if (!IsNewlyAddedLine(currentLine, index, fullFileName))
                                    return;

                                foreach (var currentPair in PatternSeverities.Where(x => x.Key.IsMatch(currentLine)))
                                {
                                    result.Details.Add($"Line {index}", currentLine);
                                    UpdateState(result, currentPair.Value);
                                }
                            });

                        _lastProcessedLine[currentFile.FullName] = lineCount;
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

        private IEnumerable<FileInfo> GetFilesSinceLastUpdate()
        {
            return Directory.GetFiles(_logDirectoryPath)
                            .Where(x => FileNamePattern?.IsMatch(x) ?? false)
                            .Select(x => new FileInfo(x))
                            .Where(x => x.LastWriteTime >= _lastUpdatedOn);
        }

        private static void UpdateState(MonitorResult result, SeverityState lineSeverityState)
        {
            switch (result.TargetState)
            {
                case SeverityState.Warning:
                    if (lineSeverityState == SeverityState.Error)
                        result.TargetState = lineSeverityState;
                    break;
                case SeverityState.Normal:
                    if (lineSeverityState == SeverityState.Error ||
                        lineSeverityState == SeverityState.Warning)
                        result.TargetState = lineSeverityState;
                    break;
                default:
                    return;
            }
        }

        private bool IsNewlyAddedLine(string line, long index, string fileName)
        {
            if (index < _lastProcessedLine[fileName])
                return false;

            if (LogLineTimestampPattern == null)
                return true;

            var match = LogLineTimestampPattern.Match(line);

            if (!match.Success)
                return false;

            DateTime parsedDate;

            if (!DateTime.TryParse(match.Value, out parsedDate))
                return false;

            return parsedDate > _lastUpdatedOn;
        }
    }
}