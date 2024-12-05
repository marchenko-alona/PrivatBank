using System.Text.Json;

namespace QueueUtils.QueueServices.Models.DTOs
{
    public class GetOrderDTO : BaseRequestDTO
    {
        public int OrderId { get; set; }


        public override string Serialize()
        {
            return JsonSerializer.Serialize(this, DefaultSerializerOptions);
        }
    }
}
