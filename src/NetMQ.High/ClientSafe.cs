using System;
using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class ClientSafe : Client
    {
        public ClientSafe(string address) : base(address) { }

        public new Task Init()
        {
            var engine = new ClientSafeEngine(serializer, m_outgoingQueue, address);
            m_actor = NetMQActor.Create(engine);
            return engine.Source.Task;
        }
    }
}