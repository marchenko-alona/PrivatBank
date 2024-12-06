using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueueUtils.QueueServices.Models.DTOs
{
    public class GetOrderDTO : BaseRequestDTO
    {
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }


        public override string Serialize()
        {
            return JsonSerializer.Serialize(this, DefaultSerializerOptions);
        }
    }
}
