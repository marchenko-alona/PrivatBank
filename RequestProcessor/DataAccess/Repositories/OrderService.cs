using Dapper;
using QueueUtils.QueueServices.Models.DTOs;
using RequestProcessor.DataAccess.Models;
using System.Data;

namespace RequestProcessor.DataAccess.Repositories
{
    public class OrderService(IDbConnectionFactory dbConnectionFactory) : IOrderService
    {
        public async Task<int> InsertOrderAsync(CreateOrderDto createOrderDto)
        {
            using (IDbConnection connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("request_id", 0, DbType.Int32, ParameterDirection.Output);
                parameters.Add("client_id", createOrderDto.ClientId);
                parameters.Add("department_address", createOrderDto.DepartmentAddress);
                parameters.Add("amount", createOrderDto.Amount);
                parameters.Add("currency", createOrderDto.Currency);
                parameters.Add("client_ip", createOrderDto.ClientIp);
                parameters.Add("status", OrderStatus.Created);

                var result = await connection.ExecuteAsync(
                            "insert_order",
                            parameters,
                            commandType: CommandType.StoredProcedure
                        );

                var requestId = parameters.Get<int>("request_id");
                return requestId;
            }
        }

        public async Task<List<Order>> GetOrdersByClientIdAsync(string clientId, string departmentAddress)
        {
            using (IDbConnection connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();

                var parameters = new
                {
                    client_id_param = clientId,
                    department_address_param = departmentAddress
                };

                await connection.ExecuteAsync(
                    "CALL get_orders(@client_id_param, @department_address_param)",
                    commandType: CommandType.Text);

                var orders = await connection.QueryAsync<Order>(
                    "FETCH ALL IN result_cursor",
                    commandType: CommandType.Text);

                return orders.ToList();
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            using (IDbConnection connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("client_id", dbType: DbType.String, direction: ParameterDirection.Output);
                parameters.Add("department_address", dbType: DbType.String, direction: ParameterDirection.Output);
                parameters.Add("amount", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                parameters.Add("currency", dbType: DbType.String, direction: ParameterDirection.Output);
                parameters.Add("status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("client_ip", dbType: DbType.String, direction: ParameterDirection.Output);
                parameters.Add("order_id", orderId);

                await connection.ExecuteAsync(
                    "get_order_by_id",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var order = new Order
                {
                    ClientId = parameters.Get<string>("client_id"),
                    DepartmentAddress = parameters.Get<string>("department_address"),
                    Amount = parameters.Get<decimal>("amount"),
                    Currency = parameters.Get<string>("currency"),
                    Status = (OrderStatus)parameters.Get<int>("status"),
                    ClientIp = parameters.Get<string>("client_ip"),
                    Id = orderId
                };

                return order;
            }
        }
    }
}

