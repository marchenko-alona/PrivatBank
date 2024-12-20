﻿using QueueUtils.QueueServices.Models;

namespace QueueUtils.QueueServices
{
    public interface IQueueServiceConsumer
    {
        Task StartListening(QueueSettings queueSettings, Func<QueueItem, Task<string>> handler, CancellationToken cancellationToken);
    }
}
