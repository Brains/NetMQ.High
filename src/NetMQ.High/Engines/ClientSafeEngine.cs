using System;
using System.Threading.Tasks;
using NetMQ.High.Serializers;

namespace NetMQ.High.Engines
{
    public class ClientSafeEngine : ClientEngine
    {
        readonly TaskCompletionSource<object> source;

        public ClientSafeEngine(ISerializer serializer, NetMQQueue<OutgoingMessage> outgoingQueue, string address, TaskCompletionSource<object> source)
            : base(serializer, outgoingQueue, address) =>
                this.source = source;

        protected override void Initialize()
        {
            try
            {
                base.Initialize();
                source.SetResult(null);
            }
            catch (Exception e)
            {
                source.SetException(e);
            }
        }
    }
}