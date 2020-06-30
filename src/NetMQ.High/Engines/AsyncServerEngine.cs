using System;
using System.Threading;
using System.Threading.Tasks;
using NetMQ.High.Serializers;
using NetMQ.High.Utils;
using NetMQ.Sockets;

namespace NetMQ.High.Engines
{
    public class AsyncServerEngine : BaseEngine
    {
        public const string BindCommand = "BIND";

        private readonly ISerializer m_serializer;
        private readonly IAsyncHandler m_asyncHandler;
        public RouterSocket m_serverSocket;        
        
        public AsyncServerEngine(ISerializer serializer, IAsyncHandler asyncHandler)
        {
            m_serializer = serializer;
            m_asyncHandler = asyncHandler;
        }
               
        protected override void Initialize()
        {
            m_serverSocket = new RouterSocket();
            m_serverSocket.ReceiveReady += OnSocketReady;
            m_serverSocket.Options.Backlog = 1000;
            //m_serverSocket.Options.
            Poller.Add(m_serverSocket);            
        }
       
        protected override void Cleanup()
        {            
            m_serverSocket.Dispose();
        }       

        protected override void OnShimCommand(string command)
        {                   
            switch (command)
            {                
                case BindCommand:
                    string addresss = Shim.ReceiveFrameString();
                    m_serverSocket.Bind(addresss);
                    break;                    
            }
        }

        private void OnSocketReady(object sender, NetMQSocketEventArgs e)
        {
            Codec.Receive(m_serverSocket);

            bool oneway = Codec.Message.OneWay == 1;        
            //object message = m_serializer.Deserialize(Codec.Message.Subject, Codec.Message.Body, 0, Codec.Message.Body.Length);
            byte[] message = Codec.Message.Body;
            ulong messageId = Codec.Message.MessageId;
            string service = Codec.Message.Service;            
            byte[] routingId = Codec.RoutingId;

            //Console.WriteLine("start messageId: " + messageId);
            if (oneway)
            {
                // TODO: this should run on user provided task scheduler
                ThreadPool.QueueUserWorkItem(s => HandleOneWay(routingId, messageId, service, message));
            }
            else
            {
                // TODO: this should run on user provided task scheduler
                ThreadPool.QueueUserWorkItem(s =>
                {
                    // we set the task scheduler so we now run on the actor thread to complete the request async
                    HandleRequestAsync(routingId, messageId, service, message).
                        ContinueWith(t => CompleteRequestAsync(t, service, messageId, routingId), Poller);
                });
            }
        }

        private void CompleteRequestAsync(Task<byte[]> t, string subject, ulong messageId, byte[] routingId)
        {
            //Console.WriteLine("Completed messageId: " + messageId);
            if (t.IsFaulted)
            {
                // Exception, let just send an error
                Codec.Id = Codec.MessageId.Message;
                Codec.Error.RelatedMessageId = messageId;

                Codec.RoutingId = routingId;
                Codec.Send(m_serverSocket);
            }
            else
            {
                byte[] reply = t.Result;

                //string subject = m_serializer.GetObjectSubject(reply);

                // TODO: Zproto should support ArraySegment to improve performance            
                //var bodySegment = m_serializer.Serialize(reply);
                //byte[] body = new byte[bodySegment.Count];
                //Buffer.BlockCopy(bodySegment.Array, bodySegment.Offset, body, 0, bodySegment.Count);

                Codec.Id = Codec.MessageId.Message;
                Codec.Message.Subject = subject;
                Codec.Message.Body = reply;
                Codec.Message.RelatedMessageId = messageId;

                Codec.RoutingId = routingId;
                Codec.Send(m_serverSocket);
            }
        }

        private void HandleOneWay(byte[] routingId, ulong messageId, string service, byte[] message)
        {
            m_asyncHandler.HandleOneWay(messageId, RouterUtility.ConvertRoutingIdToConnectionId(routingId), service, message);
        }

        private Task<byte[]> HandleRequestAsync(byte[] routingId, ulong messageId, string service, byte[] message)
        {
            return m_asyncHandler.HandleRequestAsync(messageId, RouterUtility.ConvertRoutingIdToConnectionId(routingId), service, message);
        }       
    }
}