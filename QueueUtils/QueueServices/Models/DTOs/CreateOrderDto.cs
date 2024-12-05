using System.Text.Json;

namespace QueueUtils.QueueServices.Models.DTOs
{
    public class CreateOrderDto : BaseRequestDTO
    {
        public string ClientId { get; set; }
        public string DepartmentAddress { get; set; }
        public decimal Amount { get; set; }
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
