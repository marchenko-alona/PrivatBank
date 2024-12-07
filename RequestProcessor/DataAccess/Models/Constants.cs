namespace RequestProcessor.DataAccess.Models
{
    public class Constants
    {
        public const string RequestIdParameter = "request_id";
        public const string ClientIdParameter = "client_id";
        public const string DepartmentAddressParameter = "department_address";
        public const string AmountParameter = "amount";
        public const string CurrencyParameter = "currency";
        public const string ClientIpParameter = "client_ip";
        public const string StatusParameter = "status";
        public const string OrderIdParameter = "order_id";

        // Stored procedure names
        public const string InsertOrderProcedure = "insert_order";
        public const string GetOrderByIdProcedure = "get_order_by_id";
        public const string GetOrdersProcedure = "get_orders";

        // Default size for string parameters
        public const int ClientIdSize = 50;
        public const int DepartmentAddressSize = 255;
        public const int CurrencySize = 10;
        public const int ClientIpSize = 50;
    }
}
