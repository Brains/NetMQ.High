using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class AsyncServerSafe : AsyncServer
    {
        private readonly TaskCompletionSource<object> source; // Only generic TaskCompletionSource exists
        public Task Task => source.Task;  // To await until AsyncServerEngineSafe sets a Result or Exception

        public AsyncServerSafe(IAsyncHandler handler) : base(handler)
        {
            source = new TaskCompletionSource<object>();
            Engine = new AsyncServerEngineSafe(serializer, handler, source);
        }
    }
}