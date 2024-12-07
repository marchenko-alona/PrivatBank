using QueueUtils.QueueServices.Models.DTOs;
using RequestProcessor.DataAccess.Models;

namespace RequestProcessor.DataAccess.Services
{
    public interface IOrderService
    {
        Task<int> InsertOrderAsync(CreateOrderDto createOrderDto);
        Task<List<Order>> GetOrdersByClientIdAsync(string clientId, string departmentAddress);
        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}
