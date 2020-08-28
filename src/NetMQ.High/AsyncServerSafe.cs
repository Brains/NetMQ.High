using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class AsyncServerSafe : AsyncServer
    {
        public AsyncServerSafe(IAsyncHandler asyncHandler) : base(asyncHandler) => 
            ServerEngine = new AsyncServerEngineSafe(serializer, asyncHandler);

        public new Task Bind(string address)
        {
            base.Bind(address);
            return (ServerEngine as AsyncServerEngineSafe)?.Source.Task;
        }
    }
}