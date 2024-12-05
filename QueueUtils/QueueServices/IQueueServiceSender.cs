using QueueUtils.QueueServices.Models;
using QueueUtils.QueueServices.Models.DTOs;

namespace QueueUtils.QueueServices
{
    public interface IQueueServiceSender
    {
        Task InitQueues(QueueSettings queueSettings, CancellationToken cancellationToken);
        Task<string> SendMessage(QueueItem message, QueueSettings queueSettings, CancellationToken cancellationToken);
    }
}
