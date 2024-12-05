
namespace QueueUtils.QueueServices.Configs;

public class DatabaseSettings
{
    public DatabaseType DatabaseType { get; set; }
    public string ConnectionString { get; set; }
}

public enum DatabaseType
{
    MsSql,
    PostgreSql
}
