
using Microsoft.Extensions.Options;
using QueueUtils.QueueServices;
using QueueUtils.QueueServices.Models;

namespace RequestProcessor.HostedServices
{
    public class MessageHandlerHostedService : BackgroundService
    {
        private readonly QueueSettings _queueSettings;
        private readonly IQueueServiceConsumer _queueService;

        public MessageHandlerHostedService(IOptions<QueueSettings> queueSettings, IQueueServiceConsumer queueService)
        {
            _queueSettings = queueSettings.Value;
            _queueService = queueService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Func<QueueItem, string> handler = (queueItem) =>
            {
                return $"Processed item with Id: {queueItem.EventType}, Name: {queueItem.Message}";
            };

            Console.WriteLine("start listening");
            await _queueService.StartListening(_queueSettings, handler, CancellationToken.None);
            Console.WriteLine("done listening");
        }
    }
}
