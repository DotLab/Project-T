﻿using GameLogic.Campaign;
using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameLogic.Client
{
    public abstract class ClientComponent : IMessageReceiver
    {
        protected readonly Connection _connection;
        protected readonly User _owner;

        protected ClientComponent(Connection connection, User owner)
        {
            Debug.Assert(connection != null && owner != null);
            _connection = connection;
            _owner = owner;
        }

        public abstract void MessageReceived(ulong timestamp, Message message);
    }

    public class Client : ClientComponent
    {
        protected readonly CharacterData _characterData;
        protected readonly SkillCheckPanel _skillCheckPanel;

        public SkillCheckPanel SkillCheckPanel => _skillCheckPanel;

        public override void MessageReceived(ulong timestamp, Message message)
        {
            this.SynchronizeData();
        }

        protected virtual void SynchronizeData()
        {

        }

        protected Client(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(ClientInitMessage.MESSAGE_TYPE, this);
            _characterData = new CharacterData(connection, owner);
            _skillCheckPanel = new SkillCheckPanel(connection, owner);
        }
        
        public virtual void Update()
        {
            _connection.UpdateReceiver();
        }
        
        public void ShowScene(ContainerType scene)
        {
            ShowSceneMessage message = new ShowSceneMessage();
            message.sceneType = (int)scene;
            _connection.SendMessage(message);
        }

        public void PlayBGM(string id)
        {
            PlayBGMMessage message = new PlayBGMMessage();
            message.bgmID = id;
            _connection.SendMessage(message);
        }

        public void StopBGM()
        {
            StopBGMMessage message = new StopBGMMessage();
            _connection.SendMessage(message);
        }

        public void PlaySE(string id)
        {
            PlaySEMessage message = new PlaySEMessage();
            message.seID = id;
            _connection.SendMessage(message);
        }

    }

    public sealed class PlayerClient : Client
    {
        private readonly PlayerStoryScene _storyScene;
        private readonly PlayerBattleSceme _battleScene;

        public PlayerStoryScene StoryScene => _storyScene;
        public PlayerBattleSceme BattleScene => _battleScene;

        public PlayerClient(Connection connection, Player owner) :
            base(connection, owner)
        {
            _storyScene = new PlayerStoryScene(connection, owner);
            _battleScene = new PlayerBattleSceme(connection, owner);
        }

    }

    public sealed class DMClient : Client
    {
        private readonly DMStoryScene _storyScene;
        private readonly DMBattleScene _battleScene;
        private readonly DMCheckDialog _dmCheckDialog;
        
        public DMStoryScene StoryScene => _storyScene;
        public DMBattleScene BattleScene => _battleScene;
        public DMCheckDialog DMCheckDialog => _dmCheckDialog;

        public DMClient(Connection connection, DM owner) :
            base(connection, owner)
        {
            _storyScene = new DMStoryScene(connection, owner);
            _battleScene = new DMBattleScene(connection, owner);
            _dmCheckDialog = new DMCheckDialog(connection, owner);
        }
        
    }
    
}