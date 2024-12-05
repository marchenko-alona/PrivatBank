using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Npgsql;
using QueueUtils.QueueServices.Configs;
using System.ComponentModel;
using System.Data;

namespace RequestProcessor.DataAccess
{
    public class DbConnectionFactory(IOptions<DatabaseSettings> databaseSettings) : IDbConnectionFactory
    {
        public IDbConnection GetConnection()
        {
            return databaseSettings.Value.DatabaseType switch
            {
                DatabaseType.PostgreSql => new NpgsqlConnection(databaseSettings.Value.ConnectionString),
                DatabaseType.MsSql => new SqlConnection(databaseSettings.Value.ConnectionString),
                _ => throw new InvalidEnumArgumentException($"Invalid DatabaseType: {databaseSettings.Value.DatabaseType}")
            };
        }
    }
}

