using System;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NetMQ.High.Tests
{
    [TestFixture]
    class ClientSafeTests
    {
        [Test]
        public void Init_WithNullAddress_NotFails()
        {
            using (var client = new ClientSafe(null))
            Assert.DoesNotThrow(() => client.Init());
        }

        [Test]
        public void Init_WithEmptyAddress_NotFails()
        {
            using (var client = new ClientSafe(""))
            Assert.DoesNotThrow(() => client.Init());
        }

        [Test]
        public void AwaitClientTask_WithNullAddress_ThrowsNullReferenceException()
        {
            using (var client = new ClientSafe(null))
                Assert.Throws<NullReferenceException>(async () =>
                {
                    client.Init();
                    await client.Task;
                });
        }

        [Test]
        public void AwaitClientTask_WithEmptyAddress_ThrowsArgumentOutOfRangeException()
        {
            using (var client = new ClientSafe(""))
                Assert.Throws<ArgumentOutOfRangeException>(async () =>
                {
                    client.Init();
                    await client.Task;
                });
        }
    }
}