using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameLogic.Core.Network
{
    public abstract class Streamable
    {
        public abstract long MessageID { get; }
    }

    public interface IMessageReceiver
    {
        void MessageReceived(long timestamp, Streamable message);
    }

    public abstract class Connection
    {
        public event EventHandler<ExceptionCaughtEventArgs> ExceptionCaught;
        
        public abstract void SendMessage(Streamable message);
        public abstract void AddMessageReceiver(long messageID, IMessageReceiver receiver);
        public abstract void UpdateReceiver();
    }
    
    public sealed class ExceptionCaughtEventArgs : EventArgs
    {

    }
}
