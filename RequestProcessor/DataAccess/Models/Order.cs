using System.Text.Json.Serialization;
using System.Text.Json;

namespace RequestProcessor.DataAccess.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string DepartmentAddress { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ClientIp { get; set; }
        public OrderStatus Status { get; set; }
    }
}
