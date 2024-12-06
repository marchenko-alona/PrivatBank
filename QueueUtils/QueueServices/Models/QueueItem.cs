using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueueUtils.QueueServices.Models
{
    public class QueueItem
    {
        [JsonPropertyName("eventType")]
        public QueueMessageType EventType { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        public QueueItem(QueueMessageType eventType)
        {
            EventType = eventType;
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
