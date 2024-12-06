using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueueUtils.QueueServices.Models.DTOs
{
    public class GetOrdersDTO : BaseRequestDTO
    {
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("departmentAddress")]
        public string DepartmentAddress { get; set; }


        public override string Serialize()
        {
            return JsonSerializer.Serialize(this, DefaultSerializerOptions);
        }
    }
}
