using Microsoft.Extensions.Options;
using QueueUtils.QueueServices.Configs;
using RequestProcessor.DataAccess.Repositories;
using System.ComponentModel;

namespace RequestProcessor.DataAccess
{
    public class OrderRepositoryFactory(IOptions<DatabaseSettings> databaseSettings, IServiceProvider services)
    {
        private readonly DatabaseSettings _settings = databaseSettings.Value;

        public IOrderRepository Create()
        {
            return _settings.DatabaseType switch
            {
                DatabaseType.PostgreSql =>  services.GetRequiredService<PostgresOrderRepository>(),
                DatabaseType.MsSql => services.GetRequiredService <SqlOrderRepository>(),
                _ => throw new InvalidEnumArgumentException($"Invalid DatabaseType: {_settings.DatabaseType}")
            };
        }
    }
}
