using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class AsyncServerSafe : AsyncServer
    {
        public AsyncServerSafe(IAsyncHandler asyncHandler) : base(asyncHandler) { }

        public new Task Init()
        {
            var engine = new AsyncServerEngineSafe(serializer, asyncHandler);
            m_actor = NetMQActor.Create(engine);
            ServerEngine = engine;
            return engine.Source.Task;
        }
    }
}