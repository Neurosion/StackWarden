using System.Data.SqlClient;
using log4net;
using StackWarden.Core.Extensions;

namespace StackWarden.Monitoring.Database
{
    public class SQLPresenceMonitor : PresenceMonitor<SqlConnection>
    {
        public override string DataSourceName { get; }
        public SQLPresenceMonitor(ILog log, string connectionString)
            :base(log, new SqlConnection(connectionString.ThrowIfNullOrWhiteSpace(nameof(connectionString))))
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            DataSourceName = builder.DataSource;
        }

        protected override bool DoesDatabaseExist(SqlConnection connection)
        {
            var command = new SqlCommand($"SELECT DB_ID([{DatabaseName}])", connection);
            var commandResult = command.ExecuteScalar();
            var databaseId = 0;
            var doesExist = !int.TryParse(commandResult as string, out databaseId);

            return doesExist;
        }
    }
}