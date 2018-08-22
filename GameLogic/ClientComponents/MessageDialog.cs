using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.Network;

namespace GameLogic.ClientComponents
{
    public class MessageDialog : ClientComponent
    {
        public override void MessageReceived(Message message) { }

        public MessageDialog(Connection connection, User owner) :
            base(connection, owner)
        {

        }

        public void Show(string text)
        {
            
        }
        
    }
}
