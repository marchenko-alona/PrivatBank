namespace Utils.QueueServices.Models
{
    public class OrderDto
    {
        public string ClientId { get; set; }
        public string DepartmentAddress { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ClientIp { get; set; }

        public OrderDto(string clientId, string departmentAddress, decimal amount, string currency, string clientIp)
        {
            ClientId = clientId;
            DepartmentAddress = departmentAddress;
            Amount = amount;
            Currency = currency;
            ClientIp = clientIp;
        }
    }
}
