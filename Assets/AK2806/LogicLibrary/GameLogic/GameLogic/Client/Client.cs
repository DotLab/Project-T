using GameLogic.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public class Client
    {
        protected readonly Connection _networkRef;

        protected Client(Connection connection)
        {
            _networkRef = connection ?? throw new ArgumentNullException(nameof(connection));
        }
        
        public virtual void Update()
        {
            _networkRef.UpdateReceiver();
        }
    }

    public sealed class PlayerClient : Client
    {
        public enum PlayerClientScene
        {
            STORYBOARD,
            BATTLEGROUND
        }

        private readonly PlayerStoryScene _storyboard;
        private readonly PlayerBattleground _battleground;

        public PlayerStoryScene Storyboard => _storyboard;
        public PlayerBattleground Battleground => _battleground;

        public PlayerClient(Connection connection) :
            base(connection)
        {
            _storyboard = new PlayerStoryScene(connection, this);
            _battleground = new PlayerBattleground(this);
        }

        public void UseScene(PlayerClientScene scene)
        {
            switch (scene)
            {
                case PlayerClientScene.STORYBOARD:
                    break;
                case PlayerClientScene.BATTLEGROUND:
                    break;
                default:
                    break;
            }
        }

    }

    public sealed class DMClient : Client
    {
        public enum DMClientScene
        {
            STORYBOARD,
            BATTLEGROUND,
            CAMPAIGNMAP
        }
        
        private readonly DMStoryScene _storyboard;
        private readonly DMBattleground _battleground;
        private readonly Queue<DMCheckInfo> _checkInfos;
        
        public DMStoryScene Storyboard => _storyboard;
        public DMBattleground Battleground => _battleground;

        public DMClient(Connection connection) :
            base(connection)
        {
            _storyboard = new DMStoryScene(connection, this);
            _battleground = new DMBattleground(this);
            _checkInfos = new Queue<DMCheckInfo>();
        }

        public void UseScene(DMClientScene scene)
        {
            switch (scene)
            {
                case DMClientScene.STORYBOARD:
                    break;
                case DMClientScene.BATTLEGROUND:
                    break;
                case DMClientScene.CAMPAIGNMAP:
                    break;
                default:
                    break;
            }
        }

        public override void Update()
        {
            base.Update();
            if (_checkInfos.Count > 0 && !_checkInfos.Peek().isSent)
            {

            }
        }

        public void DMCheck(DMCheckInfo checkInfo)
        {
            _checkInfos.Enqueue(checkInfo);
        }
    }

    public struct DMCheckInfo
    {
        public string info;
        public Action<bool> callback;
        public bool isSent;

        public DMCheckInfo(string info, Action<bool> callback)
        {
            this.info = info;
            this.callback = callback;
            this.isSent = false;
        }
    }
    
}
