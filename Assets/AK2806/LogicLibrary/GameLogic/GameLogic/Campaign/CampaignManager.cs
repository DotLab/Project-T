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
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private API _apiObj;

        private static readonly CampaignManager _instance = new CampaignManager();
        public static CampaignManager Instance => _instance;
        
        private CampaignBlock _currentCampaign;
        private CampaignBlock _currentBlock;
        
        private CampaignManager()
        {
            _apiObj = new API(this);
        }

        public CampaignBlock CurrentCampaign { get => _currentCampaign; set => _currentCampaign = value; }
        public CampaignBlock CurrentBlock { get => _currentBlock; set => _currentBlock = value; }
        /*
        public void Load(string json)
        {
            //this.mSceneList = JsonConvert.DeserializeObject<SceneListFile>(json);
        }
        */
        public object GetContext()
        {
            return _apiObj;
        }
        
        public void SetContext(object context) { }
    }
}
