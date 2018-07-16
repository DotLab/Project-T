using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public interface IClientEventHandler
    {
        void BindHander(GameClient client);
    }

    public sealed class GameClient
    {
        private bool _isDM;
        
        public void AddClientEventHandler(IClientEventHandler handler)
        {
            handler.BindHander(this);
        }
    }
}
