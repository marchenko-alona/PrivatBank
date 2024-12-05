using Microsoft.Extensions.Options;
using PrivatBank.QueueServices;

namespace PrivatBank.HostedServices
{
    public class QueueInitializer : IHostedService
    {
        private readonly IQueueService _queueService;
        private readonly QueueSettings _configuration;

        public QueueInitializer(IQueueService queueService, IOptions<QueueSettings> configuration)
        {
            _queueService = queueService;
            _configuration = configuration.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_configuration.Queues == null || !_configuration.Queues.Any())
            {
                return;
            }

            await _queueService.InitQueues(_configuration, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
