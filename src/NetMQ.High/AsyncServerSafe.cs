using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class AsyncServerSafe : AsyncServer
    {
        public TaskCompletionSource<object> Source { get; }
        public Task Task => Source.Task;

        public AsyncServerSafe(IAsyncHandler handler) : base(handler)
        {
            Source = new TaskCompletionSource<object>();
            Engine = new AsyncServerEngineSafe(serializer, handler, Source);
        }
    }
}