using System;
using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class ClientSafe : ClientTimeout
    {
        public ClientSafe(int timeout) : base(timeout) { }

        public new Task Connect(string address)
        {
            var engine = new ClientSafeEngine(serializer, m_outgoingQueue, address);
            m_actor = NetMQActor.Create(engine);
            return engine.Source.Task;
        }
    }
}