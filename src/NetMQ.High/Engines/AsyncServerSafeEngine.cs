using System;
using System.Threading.Tasks;
using NetMQ.High.Serializers;

namespace NetMQ.High.Engines
{
    public class AsyncServerEngineSafe : AsyncServerEngine
    {
        readonly TaskCompletionSource<object> source;

        public AsyncServerEngineSafe(ISerializer serializer, IAsyncHandler asyncHandler, TaskCompletionSource<object> source)
            : base(serializer, asyncHandler) => 
                this.source = source;

        protected override void OnShimCommand(string command)
        {
            try
            {
                base.OnShimCommand(command);
                source.SetResult(null);  //Null means success (only generic TaskCompletionSource exists)
            }
            catch (Exception e)
            {
                source.SetException(e);
            }
        }

    }
}