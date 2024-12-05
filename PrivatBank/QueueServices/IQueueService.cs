namespace PrivatBank.QueueServices
{
    public interface IQueueService
    {
        Task InitQueues(QueueSettings queueSettings, CancellationToken cancellationToken);
    }
}
