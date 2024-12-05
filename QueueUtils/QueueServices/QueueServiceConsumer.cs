using QueueUtils.QueueServices.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace QueueUtils.QueueServices
{
    public class QueueServiceConsumer : IQueueServiceConsumer
    {
        public async Task StartListening(QueueSettings queueSettings, Func<QueueItem, string> handler, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = queueSettings.Host,
                UserName = queueSettings.UserName,
                Password = queueSettings.Password
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueSettings.SendMessageQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                string response = string.Empty;
                var props = ea.BasicProperties;
                var replyProps = new BasicProperties
                {
                    CorrelationId = props.CorrelationId
                };

                try
                {
                    var request = QueueItem.Deserialize(message);
                    response = handler(request);
                }
                catch (Exception e)
                {
                    response = string.Empty;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: props.ReplyTo!,
                        mandatory: true, basicProperties: replyProps, body: responseBytes);
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            await channel.BasicConsumeAsync(queue: queueSettings.SendMessageQueueName, autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, cancellationToken);

        }

        //public async Task<string> SendMessage(OrderDto message, QueueSettings queueSettings, CancellationToken cancellationToken)
        //{
        //    var factory = new ConnectionFactory
        //    {
        //        HostName = queueSettings.Host,
        //        UserName = queueSettings.UserName,
        //        Password = queueSettings.Password
        //    };

        //    using var connection = await factory.CreateConnectionAsync();
        //    using var channel = await connection.CreateChannelAsync();

        //    QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
        //    var replyQueueName = queueDeclareResult.QueueName;

        //    var consumer = new AsyncEventingBasicConsumer(channel);

        //    consumer.ReceivedAsync += async (model, ea) =>
        //    {
        //        string? correlationId = ea.BasicProperties.CorrelationId;

        //        if (!string.IsNullOrEmpty(correlationId))
        //        {
        //            if (_callbackMapper.TryRemove(correlationId, out var tcs))
        //            {
        //                var body = ea.Body.ToArray();
        //                var response = Encoding.UTF8.GetString(body);
        //                tcs.TrySetResult(response);
        //            }
        //        }
        //    };

        //    await channel.BasicConsumeAsync(replyQueueName, true, consumer);

        //    string correlationId = Guid.NewGuid().ToString();
        //    var props = new BasicProperties
        //    {
        //        CorrelationId = correlationId,
        //        ReplyTo = replyQueueName
        //    };

        //    var tcs = new TaskCompletionSource<string>(
        //            TaskCreationOptions.RunContinuationsAsynchronously);
        //    _callbackMapper.TryAdd(correlationId, tcs);

        //    var jsonMessage = JsonSerializer.Serialize(message);
        //    var messageBytes = Encoding.UTF8.GetBytes(jsonMessage);

        //    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueSettings.SendMessageQueueName,
        //        mandatory: true, basicProperties: props, body: messageBytes);

        //    using CancellationTokenRegistration ctr =
        //        cancellationToken.Register(() =>
        //        {
        //            _callbackMapper.TryRemove(correlationId, out _);
        //            tcs.SetCanceled();
        //        });

        //    return await tcs.Task;
        //}
    }
}
