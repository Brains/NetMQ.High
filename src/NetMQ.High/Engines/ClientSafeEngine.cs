using NetMQ.High.Serializers;

namespace NetMQ.High.Engines
{
    public class ClientSafeEngine : ClientEngine
    {
        public ClientSafeEngine(ISerializer serializer, NetMQQueue<OutgoingMessage> outgoingQueue, string address) : 
            base(serializer, outgoingQueue, address) { }
    }
}