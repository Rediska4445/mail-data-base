using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System;
using Moq;
using Microsoft.Data.SqlClient;

namespace mailRu.Tests
{
    [TestClass]
    public class SqlConnectorTests
    {
        private SqlConnector connector;

        [TestInitialize]
        public void Setup()
        {
            connector = new SqlConnector();
        }

        public interface ISqlCommandWrapper
        {
            void ExecuteNonQuery();
            void Configure(Action<SqlCommand> configure);
        }

        public class SqlCommandWrapper : ISqlCommandWrapper
        {
            private readonly SqlCommand _command;
            public SqlCommandWrapper(SqlCommand command) => _command = command;
            public void ExecuteNonQuery() => _command.ExecuteNonQuery();
            public void Configure(Action<SqlCommand> configure) => configure?.Invoke(_command);
        }

        [TestMethod]
        public void Push_ShouldCallConfigureCommandAndTriggerOnPush()
        {
            var connector = new SqlConnector();

            bool onPushCalled = false;
            bool configureCalled = false;

            connector.onPush = conn => onPushCalled = true;

            var mockCommand = new Mock<ISqlCommandWrapper>();
            mockCommand.Setup(cmd => cmd.Configure(It.IsAny<Action<SqlCommand>>()))
                .Callback<Action<SqlCommand>>(cfg => configureCalled = true);
            mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Verifiable();

            mockCommand.Object.Configure(null);
            mockCommand.Object.ExecuteNonQuery();
            connector.onPush?.Invoke(connector.GetConnection());

            Assert.IsTrue(configureCalled, "configureCommand должен быть вызван");
            Assert.IsTrue(onPushCalled, "событие onPush должно сработать");
            mockCommand.Verify(cmd => cmd.ExecuteNonQuery(), Times.Once);
        }

        [TestMethod]
        public void Read_ShouldInvokeProcessRowForEveryRowAndTriggerOnRead()
        {
            string sql = "SELECT 1 AS DummyColumn"; 

            int rowsProcessed = 0;
            bool onReadCalled = false;

            connector.onRead = conn => onReadCalled = true;

            connector.Read(sql, reader =>
            {
                if (reader["DummyColumn"] != null)
                {
                    rowsProcessed++;
                }
            });

            Assert.IsGreaterThan(0, rowsProcessed, "processRow должен вызываться для каждой строки");
            Assert.IsTrue(onReadCalled, "событие onRead должно сработать");
        }

        [TestMethod]
        public void Open_ShouldTriggerOnOpen()
        {
            bool eventFired = false;
            connector.onOpen = conn => eventFired = true;

            connector.Open();

            Assert.IsTrue(eventFired, "Событие onOpen должно сработать.");
            Assert.AreEqual(ConnectionState.Open, connector.GetConnection().State);
            connector.Close();
        }

        [TestMethod]
        public void Close_ShouldTriggerOnClose()
        {
            connector.Open();
            bool eventFired = false;
            connector.onClose = conn => eventFired = true;

            connector.Close();

            Assert.IsTrue(eventFired, "Событие onClose должно сработать.");
            Assert.AreEqual(ConnectionState.Closed, connector.GetConnection().State);
        }
    }
}
