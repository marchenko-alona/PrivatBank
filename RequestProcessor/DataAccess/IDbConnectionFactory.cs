using System.Data;

namespace RequestProcessor.DataAccess
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
    }
}
