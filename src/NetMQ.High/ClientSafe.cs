using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class ClientSafe : Client
    {
        public override void Connect(string address)
        {
            var task = new TaskCompletionSource<object>();
            var engine = new ClientSafeEngine(serializer, m_outgoingQueue, task, address);
            m_actor = NetMQActor.Create(engine);
        }
    }
}