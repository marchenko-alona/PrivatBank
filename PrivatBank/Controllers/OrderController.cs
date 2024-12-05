using Microsoft.AspNetCore.Mvc;
using PrivatBank.Models;
using PrivatBank.QueueServices.Models;

namespace PrivatBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitRequest([FromBody] Order request)
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (request.Amount < 100 || request.Amount > 100000)
            {
                _logger.LogWarning("Incorrect amount: {Amount} for ClientID {ClientId}", request.Amount, request.ClientId);
                return BadRequest("Incorrect amount. Amount must be between 100 and 100 000");
            }

            var message = new OrderDto(
                ClientId: request.ClientId,
                DepartmentAddress: request.DepartmentAddress,
                Amount: request.Amount,
                Currency: request.Currency,
                ClientIp: clientIp);

            return new JsonResult(message);
        }

        //[HttpGet("request-status/{requestId}")]
        //public IActionResult GetStatusByRequestId(string requestId)
        //{
        //    // Запит на отримання статусу заявки №1
        //    Log.Information("Getting status for requestId: {RequestId}", requestId);
        //    return Ok($"Status of request {requestId}");
        //}

        //[HttpGet("request-status")]
        //public IActionResult GetStatusByClientAndAddress([FromQuery] string clientId, [FromQuery] string departmentAddress)
        //{
        //    // Запит на отримання статусу заявки №2
        //    Log.Information("Getting status for clientId: {ClientId}, departmentAddress: {DepartmentAddress}", clientId, departmentAddress);
        //    return Ok($"Status for client {clientId} at {departmentAddress}");
        //}
    }
}