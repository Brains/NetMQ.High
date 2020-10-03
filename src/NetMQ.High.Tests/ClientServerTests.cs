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
    public class ClientServerTests
    {
        class Handler : IAsyncHandler
        {
            public Handler()
            {
            }

            public Task<byte[]> HandleRequestAsync(ulong messageId, uint connectionId, string service, byte[] body)
            {
                ConnectionId = connectionId;
                Console.WriteLine(Encoding.ASCII.GetString(body));
                return Task.FromResult(Encoding.ASCII.GetBytes("Welcome"));
            }

            public void HandleOneWay(ulong messageId, uint connectionId, string service, byte[] body)
            {

            }

            public uint ConnectionId { get; private set; }
        }

        [Test]
        public void RequestResponse()
        {
            var serverHandler = new Handler();
            using (AsyncServer server = new AsyncServer(serverHandler))
            {
                server.Init();
                server.Bind("tcp://*:6666");
                using (Client client = new Client("tcp://localhost:6666"))
                {
                    // client to server
                    client.Init();
                    var message = Encoding.ASCII.GetBytes("World");
                    var res = client.SendRequestAsync("Hello", message).Result;
                    var reply = Encoding.ASCII.GetString(res);
                    Console.WriteLine("Reply: " + reply);
                    Assert.That(reply == "Welcome");
                }
            }
        }
    }
}
