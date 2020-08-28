using System;
using NUnit.Framework;

namespace NetMQ.High.Tests
{
    [TestFixture]
    class AsyncServerTests
    {
        [Test]
        public void Init_NullAddress_NotFails()
        {
            using (var client = new ClientSafe(null))
                Assert.DoesNotThrow(() => client.Init());
        }

        [Test]
        public void Init_EmptyAddress_NotFails()
        {
            using (var client = new ClientSafe(""))
                Assert.DoesNotThrow(() => client.Init());
        }

        [Test]
        public void Init_InvalidAddress_NotFails()
        {
            using (var client = new ClientSafe("abc"))
                Assert.DoesNotThrow(() => client.Init());
        }

        [Test]
        public void Init_MissingEndpoint_NotFails()
        {
            using (var client = new ClientSafe("inproc://test"))
                Assert.DoesNotThrow(() => client.Init());
        }

        [Test]
        public void AwaitClientTask_NullAddress_ThrowsNullReferenceException()
        {
            using (var client = new ClientSafe(null))
                Assert.Throws<NullReferenceException>(
                    async () => { client.Init(); await client.Task; });
        }

        [Test]
        public void AwaitClientTask_EmptyAddress_ThrowsArgumentOutOfRangeException()
        {
            using (var client = new ClientSafe(""))
                Assert.Throws<ArgumentOutOfRangeException>(
                    async () => { client.Init(); await client.Task; });
        }

        [Test]
        public void AwaitClientTask_InvalidAddress_ThrowsArgumentOutOfRangeException()
        {
            using (var client = new ClientSafe("abc"))
                Assert.Throws<ArgumentOutOfRangeException>(
                    async () => { client.Init(); await client.Task; });
        }

        [Test]
        public void AwaitClientTask_MissingEndpoint_ThrowsEndpointNotFoundException()
        {
            using (var client = new ClientSafe("inproc://test"))
                Assert.Throws<EndpointNotFoundException>(
                    async () => { client.Init(); await client.Task; });
        }
    }
}