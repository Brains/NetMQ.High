using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NetMQ.High.Tests
{
    [TestFixture]
    class ClientServerTests
    {
        class Handler : IAsyncHandler
        {
            public Handler()
            {
            }

            public async Task<byte[]> HandleRequestAsync(ulong messageId, uint connectionId, string service, byte[] body)
            {
                ConnectionId = connectionId;
                return Encoding.ASCII.GetBytes("Welcome");
            }

            public void HandleOneWay(ulong messageId, uint connectionId, string service, byte[] body)
            {

            }

            public uint ConnectionId { get; private set; }
        }

        [Test]
        public void RequestResponse()
        {
            int i = 0;

            var serverHandler = new Handler();
            using (AsyncServer server = new AsyncServer(serverHandler))
            {
                server.Bind("tcp://*:6666");
                using (Client client = new Client())
                {
                    // client to server
                    client.Init("tcp://localhost:6666");
                    var message = Encoding.ASCII.GetBytes("World");
                    var reply = client.SendRequestAsync("Hello", message).Result;
                    Assert.That(Encoding.ASCII.GetString(reply) == "Welcome");
                }
            }
        }
    }
}
