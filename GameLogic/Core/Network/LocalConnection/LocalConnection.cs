using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameLogic.Core.Network.LocalConnection
{
    public class LocalConnection : Connection
    {
        private MemoryStream _memoryStream;

        public LocalConnection(bool server)
        {
            _memoryStream = new MemoryStream();
            
        }

        public override void AddMessageReceiver(int messageType, IMessageReceiver receiver)
        {
            throw new NotImplementedException();
        }
        
        public override void SendMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public override void UpdateReceiver()
        {
            throw new NotImplementedException();
        }
    }
}
