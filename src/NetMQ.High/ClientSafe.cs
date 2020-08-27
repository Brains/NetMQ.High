using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class ClientSafe : Client
    {
        public override void Connect(string address)
        {
            var engine = new ClientSafeEngine(serializer, m_outgoingQueue, address);
            m_actor = NetMQActor.Create(engine);
        }
    }
}