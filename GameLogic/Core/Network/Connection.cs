using GameLogic.Core.DataSystem;
using GameLogic.Core.Network.ClientMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameLogic.Core.Network
{
    public abstract class Message : IStreamable
    {
        #region Message Creator
        public static Message New(int messageType)
        {
            switch (messageType)
            {
                case IdentifiedMessage.MESSAGE_TYPE:
                    return new IdentifiedMessage();
                case StorySceneObjectActionMessage.MESSAGE_TYPE:
                    return new StorySceneObjectActionMessage();
                case TextSelectedMessage.MESSAGE_TYPE:
                    return new TextSelectedMessage();
                // ...
                default:
                    return null;
            }
        }
        #endregion

        public abstract void WriteTo(IDataOutputStream stream);
        public abstract void ReadFrom(IDataInputStream stream);

        public abstract int MessageType { get; }
    }

    public sealed class IdentifiedMessage : Message
    {
        public Message innerMessage;
        public Guid guid;
        public bool resp;

        public const int MESSAGE_TYPE = 0;
        public override int MessageType => MESSAGE_TYPE;
        public int InnerMsgType => innerMessage.MessageType;

        public IdentifiedMessage() { }

        public IdentifiedMessage(Message message)
        {
            this.innerMessage = message;
            this.guid = Guid.NewGuid();
            this.resp = false;
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int innerType = stream.ReadInt32();
            innerMessage = New(innerType);
            innerMessage.ReadFrom(stream);
            guid = InputStreamHelper.ReadGuid(stream);
            resp = stream.ReadBoolean();
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(this.InnerMsgType);
            innerMessage.WriteTo(stream);
            OutputStreamHelper.WriteGuid(stream, guid);
            stream.WriteBoolean(resp);
        }
    }

    public interface IMessageReceiver
    {
        void MessageReceived(Message message);
    }

    public interface IRequestHandler
    {
        Message MakeResponse(Message request);
    }

    public abstract class Connection : IMessageReceiver
    {
        private readonly Dictionary<Guid, Action<Message>> _callbackDict = new Dictionary<Guid, Action<Message>>();
        private readonly Dictionary<int, IRequestHandler> _reqHandlerDict = new Dictionary<int, IRequestHandler>();
        
        public void MessageReceived(Message message)
        {
            var identifiedMessage = (IdentifiedMessage)message;
            if (identifiedMessage.resp)
            {
                if (_callbackDict.TryGetValue(identifiedMessage.guid, out Action<Message> callback))
                {
                    callback(identifiedMessage.innerMessage);
                    _callbackDict.Remove(identifiedMessage.guid);
                }
            }
            else
            {
                if (_reqHandlerDict.TryGetValue(identifiedMessage.InnerMsgType, out IRequestHandler handler))
                {
                    var resp = handler.MakeResponse(identifiedMessage.innerMessage);
                    var respWrapper = new IdentifiedMessage() { innerMessage = resp, guid = identifiedMessage.guid, resp = true };
                    SendMessage(respWrapper);
                }
            }
        }

        public void Request(Message message, Action<Message> callback)
        {
            var identifiedMsg = new IdentifiedMessage(message);
            while (_callbackDict.ContainsKey(identifiedMsg.guid)) identifiedMsg.guid = Guid.NewGuid();
            _callbackDict.Add(identifiedMsg.guid, callback);
            SendMessage(identifiedMsg);
        }

        public void SetRequestHandler(int messageType, IRequestHandler handler)
        {
            _reqHandlerDict[messageType] = handler;
        }

        public Connection()
        {
            AddMessageReceiver(IdentifiedMessage.MESSAGE_TYPE, this);
        }

        public event EventHandler<ExceptionCaughtEventArgs> ExceptionCaught;
        
        public abstract void SendMessage(Message message);
        public abstract void AddMessageReceiver(int messageType, IMessageReceiver receiver);
        public abstract void UpdateReceiver();
    }

    public sealed class ExceptionCaughtEventArgs : EventArgs
    {
        public ulong timestamp;
        public string message;
        // ...
    }

    public sealed class NetworkfMessage : Networkf.Message
    {
        private readonly Message _innerMessage;

        public Message InnerMessage => _innerMessage;

        public static Networkf.Message ParseMessage(byte[] buf, ref int i)
        {
            int type = ReadMessageType(buf, ref i);
            var stream = new BitDataInputStream(buf, i);
            var message = Message.New(type);
            message.ReadFrom(stream);
            i = stream.i;
            return new NetworkfMessage(message);
        }

        public NetworkfMessage(Message message) :
            base(message.MessageType)
        {
            _innerMessage = message;
        }

        public override void WriteTo(byte[] buf, ref int i)
        {
            var stream = new BitDataOutputStream(buf, i);
            _innerMessage.WriteTo(stream);
            i = stream.i;
        }
    }

    public sealed class NetworkfConnection : Connection
    {
        static NetworkfConnection()
        {
            Networkf.NetworkService.ParseMessage = NetworkfMessage.ParseMessage;
        }

        private readonly Networkf.NetworkService _service;
        private readonly List<Message> _messageList = new List<Message>();
        private readonly Dictionary<int, List<IMessageReceiver>> _messageReceiverDict = new Dictionary<int, List<IMessageReceiver>>();
        
        public NetworkfConnection(Networkf.NetworkService service)
        {
            _service = service;
            service.OnMessageReceived += OnMessageReceived;
            service.OnServiceTeardown += OnServiceTeardown;
        }

        public override void AddMessageReceiver(int messageType, IMessageReceiver receiver)
        {
            if (_messageReceiverDict.ContainsKey(messageType))
            {
                _messageReceiverDict[messageType].Add(receiver);
            } else
            {
                _messageReceiverDict.Add(messageType, new List<IMessageReceiver>() { receiver });
            }
        }

        public override void SendMessage(Message message)
        {
            _service.SendMessage(new NetworkfMessage(message));
        }

        public override void UpdateReceiver()
        {
            lock (_messageList)
            {
                foreach (var message in _messageList)
                {
                    if (_messageReceiverDict.ContainsKey(message.MessageType))
                    {
                        foreach (var receiver in _messageReceiverDict[message.MessageType])
                        {
                            receiver.MessageReceived(message);
                        }
                    }
                }
                _messageList.Clear();
            }
        }
        
        private void OnMessageReceived(int id, Networkf.Message message)
        {
            lock (_messageList)
            {
                _messageList.Add(((NetworkfMessage)message).InnerMessage);
            }
        }

        private void OnServiceTeardown()
        {
            _service.OnMessageReceived -= OnMessageReceived;
            _service.OnServiceTeardown -= OnServiceTeardown;
        }
    }
}
