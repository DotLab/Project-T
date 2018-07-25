using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public class ClientComponent
    {
        protected readonly Connection _connectionRef;
        protected readonly User _owner;

        protected ClientComponent(Connection connection, User owner)
        {
            _connectionRef = connection ?? throw new ArgumentNullException(nameof(connection));
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
    }

    public class Client : ClientComponent, IMessageReceiver
    {
        public enum ClientScene
        {
            STORY,
            BATTLE,
            MAP
        }

        
        public virtual void MessageReceived(long timestamp, Streamable message)
        {
            this.SynchronizeData();
        }

        protected virtual void SynchronizeData()
        {

        }

        protected Client(Connection connection, User owner) :
            base(connection, owner)
        {
            _connectionRef.AddMessageReceiver(ClientInitMessage.MESSAGE_ID, this);
        }
        
        public virtual void Update()
        {
            _connectionRef.UpdateReceiver();
        }
        
        public void ShowScene(ClientScene scene)
        {
            ShowSceneMessage message = new ShowSceneMessage();
            message.sceneType = (int)scene;
            _connectionRef.SendMessage(message);
        }

        public void PlayBGM(string id)
        {
            PlayBGMMessage message = new PlayBGMMessage();
            message.bgmID = id;
            _connectionRef.SendMessage(message);
        }

        public void StopBGM()
        {
            StopBGMMessage message = new StopBGMMessage();
            _connectionRef.SendMessage(message);
        }

        public void PlaySE(string id)
        {
            PlaySEMessage message = new PlaySEMessage();
            message.seID = id;
            _connectionRef.SendMessage(message);
        }

    }

    public sealed class PlayerClient : Client
    {
        
        private readonly PlayerStoryScene _storyScene;
        private readonly PlayerBattleSceme _battleScene;

        public PlayerStoryScene StoryScene => _storyScene;
        public PlayerBattleSceme BattleScene => _battleScene;

        public PlayerClient(Connection connection, User owner) :
            base(connection, owner)
        {
            _storyScene = new PlayerStoryScene(connection, owner);
            _battleScene = new PlayerBattleSceme();
        }

    }

    public sealed class DMClient : Client
    {
        private readonly DMStoryScene _storyScene;
        private readonly DMBattleScene _battleScene;
        private readonly Queue<DMCheckInfo> _checkInfos;
        
        public DMStoryScene StoryScene => _storyScene;
        public DMBattleScene BattleScene => _battleScene;
        
        public DMClient(Connection connection, User owner) :
            base(connection, owner)
        {
            _storyScene = new DMStoryScene(connection, owner);
            _battleScene = new DMBattleScene();
            _checkInfos = new Queue<DMCheckInfo>();
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
