using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class ClientSafe : Client
    {
        public TaskCompletionSource<object> Source { get; }
        public Task Task => Source.Task;

        public ClientSafe(string address) : base(address)
        {
            Source = new TaskCompletionSource<object>();
            Engine = new ClientSafeEngine(Serializer, m_outgoingQueue, Address, Source);
        }
    }
}