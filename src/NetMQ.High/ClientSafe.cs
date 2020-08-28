using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class ClientSafe : Client
    {
        public ClientSafe(string address) : base(address) { }

        public new Task Init()
        {
            var engine = new ClientSafeEngine(Serializer, m_outgoingQueue, Address);
            m_actor = NetMQActor.Create(engine);
            return engine.Source.Task;
        }
    }
}