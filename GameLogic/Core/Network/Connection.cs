using GameLogic.Core.DataSystem;
using GameLogic.Core.Network.ClientMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameLogic.Core.Network
{
    public abstract class Message : Networkf.Message, IStreamable
    {
        #region Message Creator
        public static Message New(long messageType)
        {
            switch (messageType)
            {
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

        public abstract long MessageType { get; }

        protected Message() : base(0) {
            type = (int)MessageType;
        }

        public override void WriteTo(byte[] buf, ref int i)
        {
            var stream = new BitDataOutputStream(buf, i);
            WriteTo(stream);
            i = stream.i;
        }
    }

    public interface IMessageReceiver
    {
        void MessageReceived(ulong timestamp, Message message);
    }

    public abstract class Connection
    {
        public event EventHandler<ExceptionCaughtEventArgs> ExceptionCaught;

        public abstract void SendMessage(Message message);
        public abstract void AddMessageReceiver(long messageType, IMessageReceiver receiver);
        public abstract void UpdateReceiver();
    }

    public sealed class ExceptionCaughtEventArgs : EventArgs
    {
        public ulong timestamp;
        public string message;
        // ...
    }

    public sealed class NetworkfConnection : Connection
    {
        public readonly Networkf.NetworkService service;
        public readonly List<Message> messageList = new List<Message>();
        public readonly Dictionary<long, List<IMessageReceiver>> messageReceiverDict = new Dictionary<long, List<IMessageReceiver>>();

        public NetworkfConnection(Networkf.NetworkService service)
        {
            this.service = service;
            service.OnMessageReceived += OnMessageReceived;
            service.OnServiceTeardown += OnServiceTeardown;
        }

        public override void AddMessageReceiver(long messageType, IMessageReceiver receiver)
        {
            if (messageReceiverDict.ContainsKey(messageType))
            {
                messageReceiverDict[messageType].Add(receiver);
            } else
            {
                messageReceiverDict.Add(messageType, new List<IMessageReceiver>() { receiver });
            }
        }

        public override void SendMessage(Message message)
        {
            service.SendMessage(message);
        }

        public override void UpdateReceiver()
        {
            lock (messageList)
            {
                foreach (var message in messageList)
                {
                    if (messageReceiverDict.ContainsKey(message.MessageType))
                    {
                        foreach (var receiver in messageReceiverDict[message.MessageType])
                        {
                            receiver.MessageReceived(0, message);
                        }
                    }
                }
                messageList.Clear();
            }
        }

        private void OnMessageReceived(int id, Networkf.Message message)
        {
            lock (messageList)
            {
                messageList.Add(message as Message);
            }
        }

        private void OnServiceTeardown()
        {
            service.OnMessageReceived -= OnMessageReceived;
            service.OnServiceTeardown -= OnServiceTeardown;
        }
    }
}
