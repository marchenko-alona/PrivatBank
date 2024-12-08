using Dapper;
using RequestProcessor.DataAccess.Models;
using System.Data;

namespace RequestProcessor.DataAccess.Repositories
{
    public class PostgresOrderRepository : BaseOrderRepository
    {
        public PostgresOrderRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async override Task<List<Order>> GetOrdersByClientIdAsync(string clientId, string departmentAddress)
        {
            using (IDbConnection connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();

                var query = @"SELECT * FROM get_orders(@ClientId, @DepartmentAddress)";
                var parameters = new { ClientId = clientId, DepartmentAddress = departmentAddress };
                var result = await connection.QueryAsync<Order>(query, parameters);

                var orders = result.ToList();

                if (!orders.Any())
                {
                    return new List<Order>();
                }

                return orders;
            }
        }

        public async override Task<Order?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                return await base.GetOrderByIdAsync(orderId);
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "P0001")
            {
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
