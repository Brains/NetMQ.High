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
    public class LoadBalancerTests
    {
        class Handler : IHandler
        {
            public object HandleRequest(ulong messageId, uint connectionId, string service, object body)
            {             
                ConnectionId = connectionId;
                return "Welcome";
            }

            public void HandleOneWay(ulong messageId, uint connectionId, string service, object body)
            {                
            }

            public uint ConnectionId { get; private set; }
        }

        [Test]
        public void RequestResponse()
        {
            using (var lb = new LoadBalancer("tcp://*:5557", "tcp://*:5558"))
            using (var client = new Client())
            using (var worker = new Worker(new Handler(), "tcp://localhost:5558"))
            {
                client.Init("tcp://localhost:5557");
                worker.Register("Hello");

                // Wait for the lb to process the register
                Thread.Sleep(100);

                var message = Encoding.ASCII.GetBytes("World");
                var reply = client.SendRequestAsync("Hello", message).Result;
                Assert.That(Encoding.ASCII.GetString(reply) == "Welcome");
            }
        }
    }
}
