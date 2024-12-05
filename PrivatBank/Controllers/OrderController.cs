using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PrivatBank.Models;
using QueueUtils.QueueServices;
using QueueUtils.QueueServices.Models;
using QueueUtils.QueueServices.Models.DTOs;

namespace PrivatBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(ILogger<OrderController> logger, IQueueServiceSender queueService, IOptions<QueueSettings> queueSettings) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> SubmitRequest([FromBody] Order request, CancellationToken cancellationToken)
        {
            var clientIp = GetClientIp();

            if (request.Amount < 100 || request.Amount > 100000)
            {
                logger.LogWarning("Incorrect amount: {Amount} for ClientID {ClientId}", request.Amount, request.ClientId);
                return BadRequest("Incorrect amount. Amount must be between 100 and 100 000");
            }

            var model = new CreateOrderDto(
                clientId: request.ClientId,
                departmentAddress: request.DepartmentAddress,
                amount: request.Amount,
                currency: request.Currency,
                clientIp: clientIp);

            var queueItem = new QueueItem(QueueMessageType.CreateRequest);
            queueItem.Message = model.Serialize();

            var response = await queueService.SendMessage(queueItem, queueSettings.Value, cancellationToken);

            return Ok(response);
        }

        [HttpGet("request-status/{requestId}")]
        public async Task<IActionResult> GetStatusByRequestIdAsync(int requestId, CancellationToken cancellationToken)
        {
            var clientIp = GetClientIp();

            var model = new GetOrderDTO()
            {
                ClientIp = clientIp,
                OrderId = requestId
            };

            var queueItem = new QueueItem(QueueMessageType.GetOrder);
            queueItem.Message = model.Serialize();

            var response = await queueService.SendMessage(queueItem, queueSettings.Value, cancellationToken);

            return Ok(response);
        }

        [HttpGet("request-status")]
        public async Task<IActionResult> GetStatusByClientAndAddressAsync([FromQuery] string clientId, [FromQuery] string departmentAddress, CancellationToken cancellationToken)
        {
            var clientIp = GetClientIp();

            var model = new GetOrdersDTO()
            {
                ClientIp = clientIp,
                ClientId = clientId,
                DepartmentAddress = departmentAddress
            };

            var queueItem = new QueueItem(QueueMessageType.GetOrders);
            queueItem.Message = model.Serialize();

            var response = await queueService.SendMessage(queueItem, queueSettings.Value, cancellationToken);

            return Ok(response);
        }

        private string? GetClientIp()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}