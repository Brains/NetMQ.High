using System;
using System.Threading.Tasks;

namespace NetMQ.High
{
    public interface IAsyncHandler
    {
        /// <summary>
        /// Handle request from a client // object
        /// </summary>                
        Task<byte[]> HandleRequestAsync(ulong messageId, uint connectionId, string service, byte[] body);

        /// <summary>
        /// Handle oneway request from client
        /// </summary>        
        void HandleOneWay(ulong messageId, uint connectionId, string service, byte[] body);
    }
}