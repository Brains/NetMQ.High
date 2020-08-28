using System;
using System.Threading.Tasks;
using NetMQ.High.Serializers;

namespace NetMQ.High.Engines
{
    public class AsyncServerEngineSafe : AsyncServerEngine
    {
        public AsyncServerEngineSafe(ISerializer serializer, IAsyncHandler asyncHandler) 
            : base(serializer, asyncHandler) { }

        public TaskCompletionSource<object> Source { get; set; }

        protected override void Initialize()
        {
            Source = new TaskCompletionSource<object>();

            try
            {
                base.Initialize();
                Source.SetResult(null);
            }
            catch (Exception e)
            {
                Source.SetException(e);
            }
        }

    }
}