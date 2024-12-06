using System.Text.Json.Serialization;
using System.Text.Json;

namespace QueueUtils.QueueServices.Models.DTOs
{
    public abstract class BaseRequestDTO
    {
        [JsonPropertyName("clientIp")]
        public string ClientIp { get; set; }

        internal static JsonSerializerOptions DefaultSerializerOptions { get; set; } = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
        {
            new JsonStringEnumConverter()
        },
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        public abstract string Serialize();
    }
}
