namespace RequestProcessor.DataAccess.Models
{
    public class Constants
    {
        public const string RequestId = "request_id";
        public const string ClientId = "client_id";
        public const string DepartmentAddress = "department_address";
        public const string Amount = "amount";
        public const string Currency = "currency";
        public const string ClientIp = "client_ip";
        public const string Status = "status";
        public const string OrderId = "order_id";
        public const string DepartmentAddressParam = "department_address_param";
        public const string ClientIdParam = "client_id_param";

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
