using Dapper;
using QueueUtils.QueueServices.Models.DTOs;
using RequestProcessor.DataAccess.Models;
using System.Data;

namespace RequestProcessor.DataAccess.Repositories
{
    public abstract class BaseOrderRepository : IOrderRepository
    {
        protected readonly IDbConnectionFactory dbConnectionFactory;

        public BaseOrderRepository(IDbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }

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

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            using (IDbConnection connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("client_id", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                parameters.Add("department_address", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);
                parameters.Add("amount", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                parameters.Add("currency", dbType: DbType.String, direction: ParameterDirection.Output, size: 10);
                parameters.Add("status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("client_ip", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                parameters.Add("order_id", orderId);

                await connection.ExecuteAsync(
                        "get_order_by_id",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );


                var value = parameters.Get<object>("client_id");
                if (value == DBNull.Value)
                {
                    return null;
                }

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

        public abstract Task<List<Order>> GetOrdersByClientIdAsync(string clientId, string departmentAddress);
    }
}

