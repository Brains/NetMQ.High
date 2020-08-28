using System;
using System.Threading;
using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class ClientTimeout : ClientSafe
    {
        readonly int timeout;

        public ClientTimeout(int timeout) => this.timeout = timeout;

        // Answer by Lawrence Johnston at https://stackoverflow.com/questions/4238345/asynchronously-wait-for-taskt-to-complete-with-timeout
        static async Task<TResult> TimeoutAfter<TResult>(Task<TResult> task, TimeSpan timeout)
        {
            using (var cancellation = new CancellationTokenSource())
            {
                var delay = Task.Delay(timeout, cancellation.Token);
                var completed = await Task.WhenAny(task, delay);

                if (completed != task) throw new TimeoutException("The operation has timed out.");

                cancellation.Cancel();
                return await task;  // Very important in order to propagate exceptions
            }
        }

        public override Task<byte[]> SendRequestAsync(string service, byte[] message) => 
            TimeoutAfter(
                base.SendRequestAsync(service, message), 
                TimeSpan.FromMilliseconds(timeout));
    }
}