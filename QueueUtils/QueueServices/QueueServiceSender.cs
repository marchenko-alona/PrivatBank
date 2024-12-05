using QueueUtils.QueueServices.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace QueueUtils.QueueServices
{
    public class QueueServiceSender : IQueueServiceSender
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper
            = new();

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

        public async Task<string> SendMessage(QueueItem message, QueueSettings queueSettings, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = queueSettings.Host,
                UserName = queueSettings.UserName,
                Password = queueSettings.Password
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
            var replyQueueName = queueDeclareResult.QueueName;

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                string? correlationId = ea.BasicProperties.CorrelationId;

                if (!string.IsNullOrEmpty(correlationId))
                {
                    if (_callbackMapper.TryRemove(correlationId, out var tcs))
                    {
                        var body = ea.Body.ToArray();
                        var response = Encoding.UTF8.GetString(body);
                        tcs.TrySetResult(response);
                    }
                }
            };

            await channel.BasicConsumeAsync(replyQueueName, true, consumer);

            string correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = replyQueueName
            };

            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(correlationId, tcs);

            var jsonMessage = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(jsonMessage);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueSettings.SendMessageQueueName,
                mandatory: true,
                basicProperties: props,
                body: messageBytes
            );

            using (cancellationToken.Register(() =>
            {
                _callbackMapper.TryRemove(correlationId, out _);
                tcs.SetCanceled();
            }))
            {
                return await tcs.Task;
            }
        }
    }
}
