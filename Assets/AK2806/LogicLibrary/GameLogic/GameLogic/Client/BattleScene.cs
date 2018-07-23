using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public class BattleScene
    {
        protected BattleScene()
        {

        }


        public void Reset()
        {

        }
    }

    public sealed class DMBattleground : BattleScene
    {
        private readonly DMClient _gameClient;

        public DMClient GameClient => _gameClient;

        public DMBattleground(DMClient parent)
        {
            _gameClient = parent;
        }

    }

    public sealed class PlayerBattleground : BattleScene
    {
        private readonly PlayerClient _gameClient;

        public PlayerClient GameClient => _gameClient;

        public PlayerBattleground(PlayerClient parent)
        {
            _gameClient = parent;
        }

    }

}
