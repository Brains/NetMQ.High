using System;
using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class ClientSafe : Client
    {
        public new Task Init(string address)
        {
            var engine = new ClientSafeEngine(serializer, m_outgoingQueue, address);
            m_actor = NetMQActor.Create(engine);
            return engine.Source.Task;
        }
    }
}