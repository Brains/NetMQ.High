using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class AsyncServerSafe : AsyncServer
    {
        private readonly TaskCompletionSource<object> source;
        public Task Task => source.Task;

        public AsyncServerSafe(IAsyncHandler handler) : base(handler)
        {
            source = new TaskCompletionSource<object>();
            Engine = new AsyncServerEngineSafe(serializer, handler, source);
        }
    }
}