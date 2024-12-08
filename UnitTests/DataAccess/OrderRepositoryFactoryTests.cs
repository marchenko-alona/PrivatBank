using Microsoft.Extensions.Options;
using Moq;
using QueueUtils.QueueServices.Configs;
using RequestProcessor.DataAccess.Repositories;
using System.ComponentModel;

namespace RequestProcessor.DataAccess.Tests
{
    [TestClass()]
    public class OrderRepositoryFactoryTests
    {
        private Mock<IServiceProvider> _mockServiceProvider;
        private Mock<IOptions<DatabaseSettings>> _mockOptions;


        [TestInitialize]
        public void Setup()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockOptions = new Mock<IOptions<DatabaseSettings>>();
        }

        [TestMethod]
        public void Create_PostgreSql_Success()
        {
            // Arrange
            var settings = new DatabaseSettings { DatabaseType = DatabaseType.PostgreSql };
            _mockOptions.Setup(o => o.Value).Returns(settings);

            var factoryConnection = new Mock<IDbConnectionFactory>();
            var repository = new PostgresOrderRepository(factoryConnection.Object);

            _mockServiceProvider
                .Setup(sp => sp.GetService(typeof(PostgresOrderRepository)))
                .Returns(repository);

            var factory = new OrderRepositoryFactory(_mockOptions.Object, _mockServiceProvider.Object);

            // Act
            var result = factory.Create();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(repository, result);
        }

        [TestMethod]
        public void Create_MsSql_Success()
        {
            // Arrange
            var settings = new DatabaseSettings { DatabaseType = DatabaseType.MsSql };
            _mockOptions.Setup(o => o.Value).Returns(settings);

            var factoryConnection = new Mock<IDbConnectionFactory>();
            var repository = new SqlOrderRepository(factoryConnection.Object);

            _mockServiceProvider
                .Setup(sp => sp.GetService(typeof(SqlOrderRepository)))
                .Returns(repository);

            var factory = new OrderRepositoryFactory(_mockOptions.Object, _mockServiceProvider.Object);

            // Act
            var result = factory.Create();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(repository, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void Create_Invalid_Failed()
        {
            // Arrange
            var settings = new DatabaseSettings { DatabaseType = (DatabaseType)999 };
            _mockOptions.Setup(o => o.Value).Returns(settings);

            var factory = new OrderRepositoryFactory(_mockOptions.Object, _mockServiceProvider.Object);

            // Act
            factory.Create();
        }
    }
}