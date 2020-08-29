﻿using System;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NetMQ.High.Tests
{
    public class ClientSafeTests2
    {
        class Handler : IAsyncHandler
        {
            public void HandleOneWay(ulong messageId, uint connectionId, string service, byte[] body) { }
            public Task<byte[]> HandleRequestAsync(ulong messageId, uint connectionId, string service, byte[] body) =>
                Task.FromResult(new byte[] { });
        }

        [Test]
        [Ignore]
        public void Template()
        {
            //using (var server = new AsyncServer(new Handler()))
            {
                //server.Bind("inproc://test");
                var client = new ClientSafe("inproc://test");
                //client.Init();
                //client.Task.Wait();
                client.Dispose();
                Assert.Throws<ArgumentNullException>(() => client.Dispose());
            }
        }

        [Test]
        public void Dispose_Initialized_NotThrows()
        {
            var client = new ClientSafe("inproc://test");
            client.Init();
            Assert.DoesNotThrow(
                () => client.Dispose());
        }

        [Test]
        public void Dispose_NotInitialized_ThrowsArgumentNullException()
        {
            var client = new ClientSafe("inproc://test");
            Assert.Throws<ArgumentNullException>(
                () => client.Dispose());
        }

        [Test]
        public void Dispose_Initialized_Twice_ThrowsFaultException()
        {
            var client = new ClientSafe("inproc://test");
            client.Init();
            client.Dispose();
            Assert.Throws<FaultException>(
                () => client.Dispose(), 
                "Cannot close an uninitialised Msg.");
        }

        [Test]
        public void Dispose2_Twice_ThrowsFaultException()
        {
            //using (var server = new AsyncServer(new Handler()))
            {
                //server.Bind("inproc://test");
                var client = new ClientSafe("inproc://test");
                client.Init();
                //client.Task.Wait();
                client.Dispose();
                Assert.Throws<FaultException>(() => client.Dispose(), "Cannot close an uninitialised Msg.");
            }
        }

        [Test]
        public void SendRequestAsync_DisposedClient_()
        {
            var client = new ClientSafe("inproc://test");
            client.Init();
            // client.Task.Wait();
            client.Dispose();
            Task.Delay(2000).Wait();
            var message = Encoding.ASCII.GetBytes("World");
            //client.SendRequestAsync("serice", message);
            client.Dispose();
        }
    }
}