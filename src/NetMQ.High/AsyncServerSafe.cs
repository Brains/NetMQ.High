using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class AsyncServerSafe : AsyncServer
    {
        public TaskCompletionSource<object> Source { get; set; }
        public Task Task => Source.Task;
        public AsyncServerSafe(IAsyncHandler handler) : base(handler)
        {
            Source = new TaskCompletionSource<object>();
            ServerEngine = new AsyncServerSafeEngine(serializer, handler, Source);
        }
    }
}