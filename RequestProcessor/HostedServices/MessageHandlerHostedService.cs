using Microsoft.Extensions.Options;
using QueueUtils.QueueServices;
using QueueUtils.QueueServices.Models;
using QueueUtils.QueueServices.Models.DTOs;
using RequestProcessor.DataAccess.Services;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RequestProcessor.HostedServices
{
    public class MessageHandlerHostedService : BackgroundService
    {
        private readonly QueueSettings _queueSettings;
        private readonly IQueueServiceConsumer _queueService;
        private readonly IOrderService _orderService;
        private readonly static JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
        {
            new JsonStringEnumConverter()
        },
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        public MessageHandlerHostedService(
            IOptions<QueueSettings> queueSettings,
            IQueueServiceConsumer queueService,
            IOrderService orderService)
        {
            _queueSettings = queueSettings.Value;
            _queueService = queueService;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Func<QueueItem, Task<string>> handler = async (queueItem) =>
            {
                //todo add logging 

                switch (queueItem.EventType)
                {
                    case QueueMessageType.CreateRequest:
                        var createOrderModel = QueueItem.Deserialize<CreateOrderDto>(queueItem.Message);
                        var resultCreation = await _orderService.InsertOrderAsync(createOrderModel);
                        return JsonSerializer.Serialize(resultCreation, _jsonSerializerOptions);

                    case QueueMessageType.GetOrders:
                        var getOrders = QueueItem.Deserialize<GetOrdersDTO>(queueItem.Message);
                        var resultOrders = await _orderService.GetOrdersByClientIdAsync(getOrders.ClientId, getOrders.DepartmentAddress);
                        return JsonSerializer.Serialize(resultOrders, _jsonSerializerOptions);

                    case QueueMessageType.GetOrder:
                        var getOrderModel = QueueItem.Deserialize<GetOrderDTO>(queueItem.Message);
                        var resultOrder = await _orderService.GetOrderByIdAsync(getOrderModel.OrderId);
                        return JsonSerializer.Serialize(resultOrder, _jsonSerializerOptions);

                    default:
                        throw new InvalidEnumArgumentException($"Invalid EventType: {queueItem.EventType}");
                }
            };

            Console.WriteLine("start listening");
            await _queueService.StartListening(_queueSettings, handler, CancellationToken.None);
            Console.WriteLine("done listening");
        }
    }
}
