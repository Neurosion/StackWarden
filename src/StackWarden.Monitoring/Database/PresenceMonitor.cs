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

        public string DatabaseName => _connection.Database;
        public abstract string DataSourceName { get; }

        protected PresenceMonitor(ILog log, T connection)
            :base(log, nameof(PresenceMonitor<T>).ToExpandedString())
        {
            _connection = connection.ThrowIfNull(nameof(connection));
            Name += $"Database presence monitor for {DatabaseName} on {DataSourceName}.";
        }

        protected abstract bool DoesDatabaseExist(T connection);

        protected override void Update(Result result)
        {
            result.Target.Name = $"{DatabaseName} on {DataSourceName}";
            result.Metadata.Add("Database", DatabaseName);
            result.Metadata.Add("DataSource", DataSourceName);

            try
            {
                _connection.Open();

                if (!DoesDatabaseExist(_connection))
                {
                    result.Target.State = SeverityState.Error;
                    result.Message = "Database was not found.";
                }
            }
            catch (Exception ex)
            {
                result.Target.State = SeverityState.Error;
                result.Message = ex.ToDetailString();
            }
            finally
            {
                if (_connection?.State.HasFlag(ConnectionState.Open) ?? false)
                    _connection.Close();
            }
        }
    }
}