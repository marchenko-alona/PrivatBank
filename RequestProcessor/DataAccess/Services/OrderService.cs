using QueueUtils.QueueServices.Models.DTOs;
using RequestProcessor.DataAccess.Models;

namespace RequestProcessor.DataAccess.Services
{
    public class OrderService(OrderRepositoryFactory repositoryFactory) :IOrderService
    {
        public async Task<int> InsertOrderAsync(CreateOrderDto createOrderDto)
        {
            var repository = repositoryFactory.Create();
            return await repository.InsertOrderAsync(createOrderDto);
        }

        public async Task<List<Order>> GetOrdersByClientIdAsync(string clientId, string departmentAddress)
        {
            var repository = repositoryFactory.Create();
            return await repository.GetOrdersByClientIdAsync(clientId, departmentAddress);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var repository = repositoryFactory.Create();
            return await repository.GetOrderByIdAsync(orderId);
        }
    }
}
