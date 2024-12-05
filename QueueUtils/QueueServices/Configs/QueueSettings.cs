public class QueueSettings
{
    public string Host { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public List<string> Queues { get; set; }
    public string SendMessageQueueName { get; set; }
}