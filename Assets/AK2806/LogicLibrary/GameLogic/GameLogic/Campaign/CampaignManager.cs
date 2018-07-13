using System;
using System.Collections.Generic;
using GameLogic.Scene;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.Campaign
{
    public sealed class CampaignManager : IJSContextProvider
    {
        private sealed class API : IJSAPI
        {
            private CampaignManager _outer;

            public API(CampaignManager outer)
            {
                _outer = outer;
            }

            public IJSContextProvider Origin(JSContextHelper proof)
            {
                if (proof == JSContextHelper.Instance)
                {
                    return _outer;
                }
                return null;
            }
        }

        private API _apiObj;

        private static CampaignManager _instance = new CampaignManager();
        public static CampaignManager Instance => _instance;
        
        private ICampaignBlock _currentCampaign;
        private ICampaignBlock _currentBlock;
        private StoryScene _storyScene;
        private BattleScene _battleScene;
        
        private CampaignManager()
        {
            _apiObj = new API(this);
        }

        public ICampaignBlock CurrentCampaign { get => _currentCampaign; set => _currentCampaign = value; }
        public ICampaignBlock CurrentBlock { get => _currentBlock; set => _currentBlock = value; }
        public StoryScene StoryScene { get => _storyScene; set => _storyScene = value; }
        public BattleScene BattleScene { get => _battleScene; set => _battleScene = value; }

        public void Load(string json)
        {
            //this.mSceneList = JsonConvert.DeserializeObject<SceneListFile>(json);
        }

        public object GetContext()
        {
            return _apiObj;
        }
        
        public void SetContext(object context) { }
    }
}
