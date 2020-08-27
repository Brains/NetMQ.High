using System;
using System.Threading.Tasks;
using NetMQ.High.Serializers;

namespace NetMQ.High.Engines
{
    public class ClientSafeEngine : ClientEngine
    {
        public TaskCompletionSource<object> Task { get; }

        public ClientSafeEngine(
            ISerializer serializer, 
            NetMQQueue<OutgoingMessage> outgoingQueue, 
            TaskCompletionSource<object> task,
            string address) 
            : base(serializer, outgoingQueue, address) => 
                Task = task;

        protected override void Initialize()
        {
            try
            {
                base.Initialize();
                Task.SetResult(null);
            }
            catch (Exception e)
            {
                Task.SetException(e);
            }
        }
    }
}