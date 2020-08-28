using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class AsyncServerSafe : AsyncServer
    {
        public AsyncServerSafe(IAsyncHandler asyncHandler) : base(asyncHandler) { }

        public override void Init()
        {
            var engine = new AsyncServerEngineSafe(serializer, asyncHandler);
            m_actor = NetMQActor.Create(engine);
            ServerEngine = engine;
        }

        public new Task Bind(string address)
        {
            base.Bind(address);
            return (ServerEngine as AsyncServerEngineSafe)?.Source.Task;
        }
    }
}