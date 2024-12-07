using QueueUtils.QueueServices.Models;

namespace QueueUtils.QueueServices
{
    public interface IQueueServiceSender
    {
        Task InitQueues(QueueSettings queueSettings, CancellationToken cancellationToken);
        Task<string> SendMessage(QueueItem message, QueueSettings queueSettings, CancellationToken cancellationToken);
    }
}
