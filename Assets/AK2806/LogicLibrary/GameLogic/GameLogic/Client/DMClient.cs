using GameLogic.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public interface IDMClientEventHandler
    {
        void BindHandler(DMClient client);
    }

    public sealed class DMClient
    {
        public enum DMClientScene
        {
            STORYBOARD,
            BATTLEGROUND,
            CAMPAIGNMAP
        }
        
        private readonly CampaignMap _campaignMap;
        private readonly DMStoryboard _storyboard;
        private readonly DMBattleground _battleground;
        private readonly Queue<DMCheckInfo> _checkInfos;

        public CampaignMap CampaignMap => _campaignMap;
        public DMStoryboard Storyboard => _storyboard;
        public DMBattleground Battleground => _battleground;


        public DMClient()
        {
            _campaignMap = new CampaignMap(this);
            _storyboard = new DMStoryboard(this);
            _battleground = new DMBattleground(this);
            _checkInfos = new Queue<DMCheckInfo>();
        }

        public void AddClientEventHandler(IDMClientEventHandler handler)
        {
            handler.BindHandler(this);
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

        public void Update()
        {
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

    public sealed class CampaignMap
    {
        private readonly DMClient _gameClient;

        public DMClient GameClient => _gameClient;

        public CampaignMap(DMClient parent)
        {
            _gameClient = parent;
        }

    }

    public sealed class DMStoryboard : Storyboard
    {
        private readonly DMClient _gameClient;

        public DMClient GameClient => _gameClient;

        public DMStoryboard(DMClient parent)
        {
            _gameClient = parent;
        }
        
        public event EventHandler OnNextSceneAction;
        
    }

    public sealed class DMBattleground : Battleground
    {
        private readonly DMClient _gameClient;

        public DMClient GameClient => _gameClient;

        public DMBattleground(DMClient parent)
        {
            _gameClient = parent;
        }
        
    }
    
}
