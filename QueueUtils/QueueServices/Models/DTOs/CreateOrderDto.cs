using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueueUtils.QueueServices.Models.DTOs
{
    public class CreateOrderDto : BaseRequestDTO
    {
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }


        [JsonPropertyName("departmentAddress")]
        public string DepartmentAddress { get; set; }


        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }


        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        public CreateOrderDto() { }

        public CreateOrderDto(string clientId, string departmentAddress, decimal amount, string currency, string clientIp)
        {
            ClientId = clientId;
            DepartmentAddress = departmentAddress;
            Amount = amount;
            Currency = currency;
            ClientIp = clientIp;
        }

        public override string Serialize()
        {
            return JsonSerializer.Serialize(this, DefaultSerializerOptions);
        }
    }
}
