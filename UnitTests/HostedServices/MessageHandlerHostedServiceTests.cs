using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using QueueUtils.QueueServices;
using QueueUtils.QueueServices.Models;
using QueueUtils.QueueServices.Models.DTOs;
using RequestProcessor.DataAccess.Models;
using RequestProcessor.DataAccess.Services;
using RequestProcessor.HostedServices;
using System.Text.Json;

namespace UnitTests.HostedServices
{
    [TestClass()]
    public class MessageHandlerHostedServiceTests
    {
        private readonly Mock<IQueueServiceConsumer> _mockQueueService;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<ILogger<MessageHandlerHostedService>> _mockLogger;
        private readonly IOptions<QueueSettings> _queueSettings;

        public MessageHandlerHostedServiceTests()
        {
            _mockQueueService = new Mock<IQueueServiceConsumer>();
            _mockOrderService = new Mock<IOrderService>();
            _mockLogger = new Mock<ILogger<MessageHandlerHostedService>>();
            _queueSettings = Options.Create(new QueueSettings { SendMessageQueueName = "TestQueue" });
        }

        [TestMethod]
        public async Task CreateOrder_Success()
        {
            // Arrange
            var createOrderDto = new CreateOrderDto { ClientId = "TestClient", Amount = 1, ClientIp = "test", Currency = "usd" };

            var queueItem = new QueueItem(QueueMessageType.CreateRequest)
            {
                Message = JsonSerializer.Serialize(createOrderDto)
            };

            _mockOrderService
                .Setup(s => s.InsertOrderAsync(It.IsAny<CreateOrderDto>()))
                .ReturnsAsync(1);

            Func<QueueItem, Task<string>> capturedHandler = null;

            _mockQueueService
                .Setup(s => s.StartListening(
                    It.IsAny<QueueSettings>(),
                    It.IsAny<Func<QueueItem, Task<string>>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<QueueSettings, Func<QueueItem, Task<string>>, CancellationToken>((_, handler, _) =>
                {
                    capturedHandler = handler;
                })
                .Returns(Task.CompletedTask);

            var service = new MessageHandlerHostedService(_queueSettings, _mockQueueService.Object, _mockOrderService.Object, _mockLogger.Object);

            // Act
            await service.StartAsync(CancellationToken.None);

            var result = await capturedHandler(queueItem);

            // Assert
            Assert.IsNotNull(result);
            _mockOrderService.Verify(s => s.InsertOrderAsync(It.IsAny<CreateOrderDto>()), Times.Once);

            var id = JsonSerializer.Deserialize<int>(result);
            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public async Task GetOrder_NotFound()
        {
            // Arrange
            var orderId = 1;
            var getOrderDto = new GetOrderDTO { OrderId = orderId };

            var queueItem = new QueueItem(QueueMessageType.GetOrder)
            {
                Message = JsonSerializer.Serialize(getOrderDto)
            };

            _mockOrderService
                .Setup(s => s.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            Func<QueueItem, Task<string>> capturedHandler = null;

            _mockQueueService
                .Setup(s => s.StartListening(
                    It.IsAny<QueueSettings>(),
                    It.IsAny<Func<QueueItem, Task<string>>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<QueueSettings, Func<QueueItem, Task<string>>, CancellationToken>((_, handler, _) =>
                {
                    capturedHandler = handler;
                })
                .Returns(Task.CompletedTask);

            var service = new MessageHandlerHostedService(_queueSettings, _mockQueueService.Object, _mockOrderService.Object, _mockLogger.Object);

            // Act
            await service.StartAsync(CancellationToken.None);

            var result = await capturedHandler(queueItem);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Not found", result);
            _mockOrderService.Verify(s => s.GetOrderByIdAsync(orderId), Times.Once);
        }
    }
}