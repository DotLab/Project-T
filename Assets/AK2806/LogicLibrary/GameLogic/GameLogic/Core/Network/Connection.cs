using GameLogic.Core.Network.ClientMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameLogic.Core.Network
{
    public abstract class Message : Streamable
    {
        public static Message New(long messageType)
        {
            switch (messageType)
            {
                case 0L:
                    return new StorySceneObjectActionMessage();
                case 1L:
                    return new TextSelectedMessage();
                // ...
                default:
                    return null;
            }
        }

        public abstract long MessageType { get; }
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

}
