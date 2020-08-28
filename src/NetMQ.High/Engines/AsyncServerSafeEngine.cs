using System;
using System.Threading.Tasks;
using NetMQ.High.Serializers;

namespace NetMQ.High.Engines
{
    public class AsyncServerSafeEngine : AsyncServerEngine
    {
        readonly TaskCompletionSource<object> source;

        public AsyncServerSafeEngine(ISerializer serializer, IAsyncHandler asyncHandler, TaskCompletionSource<object> source)
            : base(serializer, asyncHandler) => 
                this.source = source;

        protected override void OnShimCommand(string command)
        {
            try
            {
                base.OnShimCommand(command);
                source.SetResult(null);
            }
            catch (Exception e)
            {
                source.SetException(e);
            }
        }

    }
}