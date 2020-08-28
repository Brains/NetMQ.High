using System;
using System.Threading.Tasks;
using NetMQ.High.Serializers;

namespace NetMQ.High.Engines
{
    public class ClientSafeEngine : ClientEngine
    {
        public TaskCompletionSource<object> Source { get; set; }

        public ClientSafeEngine(
            ISerializer serializer, 
            NetMQQueue<OutgoingMessage> outgoingQueue,
            string address) 
            : base(serializer, outgoingQueue, address) { }

        protected override void Initialize()
        {
            Source = new TaskCompletionSource<object>();

            try
            {
                base.Initialize();
                Source.SetResult(null);
            }
            catch (Exception e)
            {
                Source.SetException(e);
            }
        }
    }
}