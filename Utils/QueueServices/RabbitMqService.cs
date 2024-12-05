using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Utils.QueueServices
{
    public class RabbitMqService : IQueueService
    {
        public async Task InitQueues(QueueSettings queueSettings, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = queueSettings.Host,
                UserName = queueSettings.UserName,
                Password = queueSettings.Password
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            foreach (var queueName in queueSettings.Queues)
            {
                await channel.QueueDeclareAsync
                    (
                        queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
            }
        }
    }
}
