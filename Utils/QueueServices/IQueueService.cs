using System.Threading;
using System.Threading.Tasks;

namespace Utils.QueueServices
{
    public interface IQueueService
    {
        Task InitQueues(QueueSettings queueSettings, CancellationToken cancellationToken);
    }
}
