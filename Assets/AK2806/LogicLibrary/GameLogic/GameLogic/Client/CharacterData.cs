using GameLogic.Core;
using GameLogic.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public class CharacterData : ClientComponent
    {
        public CharacterData(Connection connection, User owner) :
            base(connection, owner)
        {

        }

        public override void MessageReceived(ulong timestamp, Message message)
        {
            
        }
    }
}
