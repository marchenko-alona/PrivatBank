using Dapper;
using RequestProcessor.DataAccess.Models;

namespace RequestProcessor.DataAccess.Repositories
{
    public class SqlOrderRepository : BaseOrderRepository
    {
        public SqlOrderRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public override async Task<List<Order>> GetOrdersByClientIdAsync(string clientId, string departmentAddress)
        {
            using (var connection = dbConnectionFactory.GetConnection())
            {
                connection.Open();
                var query = @"EXEC get_orders @ClientId, @DepartmentAddress";
                var parameters = new { ClientId = clientId, DepartmentAddress = departmentAddress };
                return (await connection.QueryAsync<Order>(query, parameters)).ToList();
            }
        }
    }
}
