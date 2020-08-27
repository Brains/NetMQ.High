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
                    var text = Encoding.ASCII.GetString(reply);
                    Assert.That(text == "Welcome");
                }
            }
        }

        class DelayedHandler : IAsyncHandler
        {
            readonly int delay;
            public DelayedHandler(int delay) => this.delay = delay;
            public void HandleOneWay(ulong messageId, uint connectionId, string service, byte[] body) { }
            public async Task<byte[]> HandleRequestAsync(ulong messageId, uint connectionId, string service, byte[] body)
            {
                await Task.Delay(delay);
                var text = $"Delayed for {delay} milliseconds";
                return Encoding.ASCII.GetBytes(text);
            }
        }

        [Test]
        public void RequestResponseBelowTimeout()
        {
            var handler = new DelayedHandler(1000);
            using (var server = new AsyncServer(handler))
            {
                server.Bind("tcp://*:6666");
                using (var client = new TimeoutClient("tcp://localhost:6666"))
                {
                    var message = Encoding.ASCII.GetBytes("World");
                    var reply = client.SendRequestAsyncWithTimeout("Hello", message, 2000).Result;
                    var text = Encoding.ASCII.GetString(reply);
                    Assert.That(text == "Delayed for 1000 milliseconds");
                }
            }
        }

        [Test]
        public void RequestResponseAboveTimeout()
        {
            var handler = new DelayedHandler(2000);
            using (var server = new AsyncServer(handler))
            {
                server.Bind("tcp://*:6666");
                using (var client = new TimeoutClient("tcp://localhost:6666"))
                {
                    var message = Encoding.ASCII.GetBytes("World");
                    Assert.Throws<TimeoutException>(
                        async () => await client.SendRequestAsyncWithTimeout("Hello", message, 1000));
                }
            }
        }
    }
}
