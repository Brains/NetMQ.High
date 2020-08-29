using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class ClientSafe : Client
    {
        private readonly TaskCompletionSource<object> source;
        public Task Task => source.Task;

        public ClientSafe(string address) : base(address)
        {
            source = new TaskCompletionSource<object>();
            Engine = new ClientSafeEngine(Serializer, m_outgoingQueue, Address, source);
        }
    }
}