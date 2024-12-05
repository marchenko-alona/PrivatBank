namespace PrivatBank.Models
{
    public class Order
    {
        public string ClientId { get; set; }
        public string DepartmentAddress { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
