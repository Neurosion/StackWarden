using System;
using System.Data;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Configuration;
using StackWarden.Core.Extensions;

namespace StackWarden.Monitoring.Database
{
    public abstract class PresenceMonitor<T> : Monitor
        where T: class, IDbConnection
    {
        private readonly T _connection;

        protected string DatabaseName => _connection.Database;
        protected abstract string DataSourceName { get; }

        protected PresenceMonitor(ILog log, T connection)
            :base(log, nameof(PresenceMonitor<T>).ToExpandedString())
        {
            _connection = connection.ThrowIfNull(nameof(connection));
            Name += $"Database presence monitor for {DatabaseName} on {DataSourceName}.";
            Tags.Add(Constants.Categories.Database);
        }

        protected abstract bool DoesDatabaseExist(T connection);

        protected override void Update(MonitorResult result)
        {
            result.TargetName = $"{DatabaseName} on {DataSourceName}";
            result.Details.Add("Database", DatabaseName);
            result.Details.Add("Data Source", DataSourceName);

            try
            {
                _connection.Open();

                if (!DoesDatabaseExist(_connection))
                {
                    result.TargetState = SeverityState.Error;
                    result.FriendlyMessage = "Database was not found.";
                }
            }
            catch (Exception ex)
            {
                result.TargetState = SeverityState.Error;
                result.FriendlyMessage = ex.ToDetailString();
            }
            finally
            {
                if (_connection?.State.HasFlag(ConnectionState.Open) ?? false)
                    _connection.Close();
            }
        }
    }
}