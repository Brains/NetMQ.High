using System;
using System.Threading.Tasks;
using NetMQ.High.Serializers;

namespace NetMQ.High.Engines
{
    public class AsyncServerEngineSafe : AsyncServerEngine
    {
        public AsyncServerEngineSafe(ISerializer serializer, IAsyncHandler asyncHandler)
            : base(serializer, asyncHandler) => 
                Source = new TaskCompletionSource<object>();

        public TaskCompletionSource<object> Source { get; set; }

        protected override void OnShimCommand(string command)
        {
            try
            {
                base.OnShimCommand(command);
                Source.SetResult(null);
            }
            catch (Exception e)
            {
                Source.SetException(e);
            }
        }

    }
}