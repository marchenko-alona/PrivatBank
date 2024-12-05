namespace PrivatBank.QueueServices.Models
{
    public record OrderDto(string ClientId, string DepartmentAddress, decimal Amount, string Currency, string ClientIp);
}
