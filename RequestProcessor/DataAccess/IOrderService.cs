using RequestProcessor.DataAccess.Models;

namespace RequestProcessor.DataAccess
{
    public interface IOrderService
    {
        Task<int> InsertOrderAsync(string clientId, string departmentAddress, decimal amount, string currency, string clientIp = null, int status = 0);
        Task<List<Order>> GetOrdersByClientIdAsync(string clientId, string departmentAddress);
        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}
