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
                parameters.Add(Constants.RequestId, 0, DbType.Int32, ParameterDirection.Output);
                parameters.Add(Constants.ClientId, createOrderDto.ClientId);
                parameters.Add(Constants.DepartmentAddress, createOrderDto.DepartmentAddress);
                parameters.Add(Constants.Amount, createOrderDto.Amount);
                parameters.Add(Constants.Currency, createOrderDto.Currency);
                parameters.Add(Constants.ClientIp, createOrderDto.ClientIp);
                parameters.Add(Constants.Status, OrderStatus.Created);

                var result = await connection.ExecuteAsync(
                            Constants.InsertOrderProcedure,
                            parameters,
                            commandType: CommandType.StoredProcedure
                        );

                var requestId = parameters.Get<int>(Constants.RequestId);
                return requestId;
            }
        }

        public async virtual Task<Order?> GetOrderByIdAsync(int orderId)
        {
            using (IDbConnection connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add(Constants.ClientId, dbType: DbType.String, direction: ParameterDirection.Output, size: Constants.ClientIdSize);
                parameters.Add(Constants.DepartmentAddress, dbType: DbType.String, direction: ParameterDirection.Output, size: Constants.DepartmentAddressSize);
                parameters.Add(Constants.Amount, dbType: DbType.Decimal, direction: ParameterDirection.Output);
                parameters.Add(Constants.Currency, dbType: DbType.String, direction: ParameterDirection.Output, size: Constants.CurrencySize);
                parameters.Add(Constants.Status, dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add(Constants.ClientIp, dbType: DbType.String, direction: ParameterDirection.Output, size: Constants.ClientIpSize);
                parameters.Add(Constants.OrderId, orderId);

                Order order = null;

                await connection.ExecuteAsync(
                    Constants.GetOrderByIdProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var value = parameters.Get<object>(Constants.ClientId);

                if (value == null)
                {
                    return null;
                }

                order = new Order
                {
                    ClientId = parameters.Get<string>(Constants.ClientId),
                    DepartmentAddress = parameters.Get<string>(Constants.DepartmentAddress),
                    Amount = parameters.Get<decimal>(Constants.Amount),
                    Currency = parameters.Get<string>(Constants.Currency),
                    Status = (OrderStatus)parameters.Get<int>(Constants.Status),
                    ClientIp = parameters.Get<string>(Constants.ClientIp),
                    Id = orderId
                };

                return order;
            }
        }

        public abstract Task<List<Order>> GetOrdersByClientIdAsync(string clientId, string departmentAddress);
    }
}
