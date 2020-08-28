using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class AsyncServerSafe : AsyncServer
    {
        public AsyncServerSafe(IAsyncHandler asyncHandler) : base(asyncHandler) { }

        public new Task Init()
        {
            ServerEngine = new AsyncServerEngine(serializer, asyncHandler);
            m_actor = NetMQActor.Create(ServerEngine);
            return ServerEngine.Source.Task;
        }
    }
}