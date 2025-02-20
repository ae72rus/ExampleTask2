using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TranCons.EventEmitter.TCP.Tests.TestImplementations;

namespace TranCons.EventEmitter.TCP.Tests
{
    [TestClass]
    public class TcpServerTests
    {
        private const int Port = 5734;
        private readonly TcpServerConfiguration _serverConfig = new()
        {
            ServerPort = Port
        };

        private TcpServer GetTcpServerInstance()
        {
            return new TcpServer(
                new TestLogger(),
                _serverConfig
            );
        }

        [TestMethod]
        public async Task TestInstantiated()
        {
            await using var server = GetTcpServerInstance();
            Assert.IsNotNull(server);
        }

        [TestMethod]
        public async Task TestDisposed()
        {
            var server = GetTcpServerInstance();
            await server.DisposeAsync();

            await Assert.ThrowsExceptionAsync<ObjectDisposedException>(
                () => server.SendAsync(new byte[] { 1, 2, 3 })
            );
        }

        [TestMethod]
        public async Task TestListening()
        {
            await using var server = GetTcpServerInstance();
            Assert.AreEqual(0, server.GetClientsCount());
            await using var client = new TestClient(Port, bytes =>
            {
                var message = Encoding.Default.GetString(bytes);
                Trace.WriteLine(message);
            });

            await Task.Delay(100);

            Assert.AreEqual(1, server.GetClientsCount());
        }

        [TestMethod]
        public async Task TestSending()
        {
            await using var server = GetTcpServerInstance();
            var readyToCheckBytes = false;
            var testMessage = Encoding.Default.GetBytes("This is tcp server test message. 1234567890!@#$%^&*()");
            var checkFunction = () => readyToCheckBytes;

            await using var client = new TestClient(Port, bytes =>
            {
                if (!checkFunction())//to ignore server welcome message
                    return;

                Assert.AreEqual(testMessage.Length, bytes.Length);

                for (var i = 0; i < testMessage.Length; i++)
                    Assert.AreEqual(testMessage[i], bytes[i]);
            });

            await Task.Delay(100);
            readyToCheckBytes = true;
            await server.SendAsync(testMessage);
        }

        [TestMethod]
        public async Task TestSendingToDisconnected()
        {
            await using var server = GetTcpServerInstance();
            Assert.AreEqual(0, server.GetClientsCount());

            await using (new TestClient(Port, bytes =>
                   {
                       var message = Encoding.Default.GetString(bytes);
                       Trace.WriteLine(message);
                   }))
            {
                await Task.Delay(100);
                await server.SendAsync(new byte[] { 1, 2, 3, 4, 5 });
            }

            await Task.Delay(2000);
            await server.SendAsync(new byte[] { 5, 4, 3, 2, 1 });
        }
    }
}
