using System.Text.Json;

namespace QueueUtils.QueueServices.Models.DTOs
{
    public class GetOrdersDTO : BaseRequestDTO
    {
        public string ClientId { get; set; }
        public string DepartmentAddress { get; set; }


        public override string Serialize()
        {
            return JsonSerializer.Serialize(this, DefaultSerializerOptions);
        }
    }
}
