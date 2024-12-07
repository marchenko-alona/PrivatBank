using Dapper;
using RequestProcessor.DataAccess.Models;
using System.Data;

namespace RequestProcessor.DataAccess.Repositories
{
    public class SqlOrderRepository : BaseOrderRepository
    {
        public SqlOrderRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async override Task<List<Order>> GetOrdersByClientIdAsync(string clientId, string departmentAddress)
        {
            using (IDbConnection connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add(Constants.ClientIdParam, clientId);
                parameters.Add(Constants.DepartmentAddressParam, departmentAddress);

                var result = await connection.QueryAsync<Order>(
                    Constants.GetOrdersProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var orders = result.ToList();

                if (!orders.Any())
                {
                    return new List<Order>();
                }

                return orders;
            }
        }
    }
}