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
    }
}