using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NetMQ.High.Tests
{
    [TestFixture]
    class AsyncServerTests
    {
        class Handler : IAsyncHandler
        {
            public void HandleOneWay(ulong messageId, uint connectionId, string service, byte[] body) { }
            public Task<byte[]> HandleRequestAsync(ulong messageId, uint connectionId, string service, byte[] body) =>
                Task.FromResult(new byte[] { });
        }

        [Test]
        [Ignore("No way to catch. Also, Null is almost impossible for address")]
        public void Init_NullAddress_NotFails()
        {
            using (var server = new AsyncServerSafe(new Handler()))
                Assert.DoesNotThrow(() => { server.Init(); server.Bind(null); });
        }

        [Test]
        public void Init_EmptyAddress_NotFails()
        {
            using (var server = new AsyncServerSafe(new Handler()))
                Assert.DoesNotThrow(() => { server.Init(); server.Bind(""); });
        }

        [Test]
        public void Init_InvalidAddress_NotFails()
        {
            using (var server = new AsyncServerSafe(new Handler()))
                Assert.DoesNotThrow(() => { server.Init(); server.Bind("abc"); });
        }

        [Test]
        [Ignore("No way to catch. Also, Null is almost impossible for address")]
        public void AwaitClientTask_NullAddress_ThrowsNullReferenceException()
        {
            using (var server = new AsyncServerSafe(new Handler()))
                Assert.Throws<ArgumentNullException>(
                    async () => { server.Init(); server.Bind(null); await server.Task; });
        }

        [Test]
        public void AwaitClientTask_EmptyAddress_ThrowsArgumentOutOfRangeException()
        {
            using (var server = new AsyncServerSafe(new Handler()))
                Assert.Throws<ArgumentOutOfRangeException>(
                    async () => { server.Init(); server.Bind(""); await server.Task; });
        }

        [Test]
        public void AwaitClientTask_InvalidAddress_ThrowsArgumentOutOfRangeException()
        {
            using (var server = new AsyncServerSafe(new Handler()))
                Assert.Throws<ArgumentOutOfRangeException>(
                    async () => { server.Init(); server.Bind("abc"); await server.Task; });
        }
    }
}