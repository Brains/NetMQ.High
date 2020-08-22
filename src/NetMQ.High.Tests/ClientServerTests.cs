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
            public void HandleOneWay(ulong messageId, uint connectionId, string service, byte[] body) { }
            public async Task<byte[]> HandleRequestAsync(ulong messageId, uint connectionId, string service, byte[] body) => 
                Encoding.ASCII.GetBytes("Welcome");
        }     

        [Test]
        public void RequestResponse()
        {
            var serverHandler = new Handler();
            using (var server = new AsyncServer(serverHandler))
            {
                server.Bind("tcp://*:6666");
                using (var client = new Client("tcp://localhost:6666"))
                {
                    // client to server
                    var message = Encoding.ASCII.GetBytes("World");
                    var reply = client.SendRequestAsync("Hello", message).Result;
                    Assert.That(Encoding.ASCII.GetString(reply) == "Welcome");                    
                }
            }    
        }

        class SlowHandler : IAsyncHandler
        {
            public void HandleOneWay(ulong messageId, uint connectionId, string service, byte[] body) { }
            public async Task<byte[]> HandleRequestAsync(ulong messageId, uint connectionId, string service, byte[] body)
            {
                await Task.Delay(4000);
                return Encoding.ASCII.GetBytes("Delayed");
            }
        }

        [Test]
        public void RequestResponseWithTimeout()
        {
            var handler = new SlowHandler();
            using (var server = new AsyncServer(handler))
            {
                server.Bind("tcp://*:6666");
                using (var client = new Client("tcp://localhost:6666"))
                {
                    var message = Encoding.ASCII.GetBytes("World");
                    Assert.Throws<TimeoutException>(async () => await client.SendRequestAsync("Hello", message));                    
                }
            }    
        }
    }
}
