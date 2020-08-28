using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class ClientSafe : Client
    {
        public TaskCompletionSource<object> Source { get; }
        public Task Task => Source.Task;
        public ClientSafeEngine ServerEngine { get; set; }

        public ClientSafe(string address) : base(address)
        {
            Source = new TaskCompletionSource<object>();
            ServerEngine = new ClientSafeEngine(Serializer, m_outgoingQueue, Address, Source);
        }

        public new Task Init()
        {
            var engine = new ClientSafeEngine(Serializer, m_outgoingQueue, Address);
            m_actor = NetMQActor.Create(engine);
            return engine.Source.Task;
        }
    }
}