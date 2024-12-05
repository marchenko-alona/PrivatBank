using Dapper;
using Npgsql;
using RequestProcessor.DataAccess.Models;
using System.Data;

namespace RequestProcessor.DataAccess.Repositories
{
    public class OrderService(IDbConnectionFactory dbConnectionFactory) : IOrderService
    {
        public async Task<int> InsertOrderAsync(string clientId, string departmentAddress, decimal amount, string currency, string clientIp = null, int status = 0)
        {
            using (IDbConnection connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();
                var parameters = new
                {
                    client_id = clientId,
                    department_address = departmentAddress,
                    amount = amount,
                    currency = currency,
                    client_ip = clientIp,
                    status = status
                };

                var result = await connection.ExecuteScalarAsync<int>(
                            "insert_order",
                            parameters,
                            commandType: CommandType.StoredProcedure
                        );

                return result;
            }
        }

        public async Task<List<Order>> GetOrdersByClientIdAsync(string clientId, string departmentAddress)
        {
            using (IDbConnection connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();

                var parameters = new
                {
                    client_id = clientId,
                    department_address = departmentAddress
                };

                var orders = await connection.QueryAsync<Order>(
                    "get_orders",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return orders.ToList();
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            using (IDbConnection connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();

                var parameters = new { order_id = orderId };

                var order = await connection.QueryFirstOrDefaultAsync<Order>(
                    "get_order_by_id",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return order;
            }
        }
    }
}

