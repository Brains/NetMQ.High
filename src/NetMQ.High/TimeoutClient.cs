using System;
using System.Threading;
using System.Threading.Tasks;
using NetMQ.High.Engines;

namespace NetMQ.High
{
    public class TimeoutClient : Client
    {
        public TimeoutClient(string address) : base(address) { }

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

        public Task<byte[]> SendRequestAsyncWithTimeout(string service, byte[] message, int timeout)
        {
            var outgoing = new ClientEngine.OutgoingMessage(new TaskCompletionSource<byte[]>(), service, message, false);
            // NetMQQueue is thread safe, so no need to lock
            m_outgoingQueue.Enqueue(outgoing);
            return TimeoutAfter(
                outgoing.TaskCompletionSource.Task,
                TimeSpan.FromMilliseconds(timeout));
        }
    }
}