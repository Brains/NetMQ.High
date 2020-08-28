using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NetMQ.High.Engines;
using NetMQ.High.Serializers;

namespace NetMQ.High
{
    public class AsyncServer : IDisposable
    {
        protected readonly ISerializer serializer;
        protected readonly IAsyncHandler asyncHandler;
        protected NetMQActor m_actor;
        public AsyncServerEngine ServerEngine;

        /// <summary>
        /// Create new server with default serializer
        /// </summary>
        /// <param name="asyncHandler">Handler to handle messages from client</param>
        public AsyncServer(IAsyncHandler asyncHandler) : this(Global.DefaultSerializer, asyncHandler)
        {

        }

        /// <summary>
        /// Create new server
        /// </summary>
        /// <param name="serializer">Serializer to use to serialize messages</param>
        /// <param name="asyncHandler">Handler to handle messages from client</param>
        public AsyncServer(ISerializer serializer, IAsyncHandler asyncHandler)
        {
            this.serializer = serializer;
            this.asyncHandler = asyncHandler;
        }

        public virtual void Init()
        {
            ServerEngine = new AsyncServerEngine(serializer, asyncHandler);
            m_actor = NetMQActor.Create(ServerEngine);
        }

        /// <summary>
        /// Bind the server to a address. Server can be binded to multiple addresses
        /// </summary>
        /// <param name="address"></param>
        public void Bind(string address)
        {
            lock (m_actor)
            {
                m_actor.SendMoreFrame(AsyncServerEngine.BindCommand).SendFrame(address);
            }
        }

        public void Dispose()
        {
            lock (m_actor)
            {
                m_actor.Dispose();
            }
        }
    }
}