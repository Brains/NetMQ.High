using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetMQ.High.Serializers;
using NetMQ.Sockets;

namespace NetMQ.High.Engines
{
    class ClientEngine : BaseEngine
    {

        public struct OutgoingMessage
        {
            public OutgoingMessage(TaskCompletionSource<byte[]> taskCompletionSource, string service, byte[] message, bool oneway)
            {
                Service = service;
                Message = message;
                Oneway = oneway;
                TaskCompletionSource = taskCompletionSource;
            }

            public TaskCompletionSource<byte[]> TaskCompletionSource { get; }
            public string Service { get; private set; }
            public byte[] Message { get; private set; }
            public bool Oneway { get; private set; }
        }

        struct PendingMessage
        {
            public PendingMessage(ulong messageId, TaskCompletionSource<byte[]> taslCompletionSource)
            {
                MessageId = messageId;
                TaslCompletionSource = taslCompletionSource;
            }

            public ulong MessageId { get; private set; }
            public TaskCompletionSource<byte[]> TaslCompletionSource { get; private set; }
        }

        private readonly ISerializer m_serializer;
        public readonly NetMQQueue<OutgoingMessage> m_outgoingQueue;        
        private readonly string m_address;        
        
        private Dictionary<UInt64, PendingMessage> m_pendingRequests;
        private UInt64 m_nextMessageId;
        private DealerSocket m_clientSocket;

        public ClientEngine(ISerializer serializer, NetMQQueue<OutgoingMessage> outgoingQueue, string address)            
        {
            m_serializer = serializer;
            m_outgoingQueue = outgoingQueue;
            m_address = address;

            m_pendingRequests = new Dictionary<ulong, PendingMessage>();
            GC.KeepAlive(m_pendingRequests);
            m_nextMessageId = 0;
        }

        protected override void Initialize()
        {
            m_clientSocket = new DealerSocket();
            m_clientSocket.Connect(m_address);
            m_clientSocket.ReceiveReady += OnSocketReady;
            Poller.Add(m_clientSocket);

            m_outgoingQueue.ReceiveReady += OnOutgoingQueueReady;
            Poller.Add(m_outgoingQueue);
        }

        private void OnSocketReady(object sender, NetMQSocketEventArgs e)
        {
            Codec.Receive(m_clientSocket);

            UInt64 relatedMessageId = Codec.Id == Codec.MessageId.Message ? Codec.Message.RelatedMessageId : Codec.Error.RelatedMessageId;

            PendingMessage pendingMessage;           

            bool chk = m_pendingRequests.TryGetValue(relatedMessageId, out pendingMessage);
            Console.WriteLine("OnSocketReady IDs/Count: " + Codec.Message.MessageId + " ; " + Codec.Message.RelatedMessageId + "; " + m_pendingRequests.Count + "; " + chk);
            if (chk)
            {                
                if (Codec.Id == Codec.MessageId.Message)
                {
                    //var body = m_serializer.Deserialize(Codec.Message.Subject, Codec.Message.Body, 0,
                    //    Codec.Message.Body.Length);
                    //
                    if (!pendingMessage.TaslCompletionSource.Task.IsCompleted)
                    {
                        pendingMessage.TaslCompletionSource.SetResult(Codec.Message.Body);
                        m_pendingRequests.Remove(relatedMessageId);
                    }
                }
                else
                {
                    // TODO: we should pass more meaningful exceptions
                    pendingMessage.TaslCompletionSource.SetException(new Exception());
                }
            }
            else
            {
                // TOOD: how to handle messages that don't exist or probably expired
                Console.WriteLine("pendingMessage = false");
                //foreach (KeyValuePair<UInt64,PendingMessage> msg in m_pendingRequests)
                //{
                //    Console.WriteLine("Key: " + msg.Key);
                //}
            }
        }

        protected override void Cleanup()
        {
            m_clientSocket.Dispose();
        }

        protected override void OnShimCommand(string command)
        {
            throw new NotSupportedException();
        }



        private void OnOutgoingQueueReady(object sender, NetMQQueueEventArgs<OutgoingMessage> e)
        {
            var outgoingMessage = m_outgoingQueue.Dequeue();

            //var bodySegment = m_serializer.Serialize(outgoingMessage.Message);
            //byte[] body = new byte[bodySegment.Count];
            //Buffer.BlockCopy(bodySegment.Array, bodySegment.Offset, body, 0, bodySegment.Count);

            UInt64 messageId = ++m_nextMessageId;
            m_nextMessageId = messageId;

            //string subject = m_serializer.GetObjectSubject(outgoingMessage.Message);
            //Console.WriteLine("Codec.Message.RelatedMessageId: " + messageId + " ; " + Codec.Error.RelatedMessageId);


            Codec.Id = Codec.MessageId.Message;
            Codec.Message.MessageId = messageId;
            Codec.Message.Service = outgoingMessage.Service;
            Codec.Message.Subject = "";
            Codec.Message.Body = outgoingMessage.Message;
            Codec.Message.RelatedMessageId = 0;

            // one way message
            if (outgoingMessage.Oneway)
            {
                Codec.Message.OneWay = 1;
            }
            else
            {
                Codec.Message.OneWay = 0;
                // add to pending requests dictionary
                // TODO: we might want to create a pending message structure that will not hold reference to the message (can lead to GC second generation)
                var msg = new PendingMessage(messageId, outgoingMessage.TaskCompletionSource);
                m_pendingRequests.Add(messageId, msg);
                GC.KeepAlive(msg);
                GC.KeepAlive(outgoingMessage);
            }

            Codec.Send(m_clientSocket);
        }      
    }
}