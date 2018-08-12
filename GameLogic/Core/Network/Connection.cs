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
