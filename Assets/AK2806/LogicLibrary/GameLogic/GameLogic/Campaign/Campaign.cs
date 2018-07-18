using System;
using System.Collections.Generic;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.Campaign
{
    public enum CBType
    {
        Story, Battle, Movie
    }

    public abstract class CampaignBlock
    {
        protected readonly List<CampaignBlock> _nexts;
        protected string _comment;
        protected string _name;

        public abstract CBType Type { get; }
        public abstract Story StoryBlock { get; }
        public abstract Battle BattleBlock { get; }
        public abstract Movie MovieBlock { get; }

        public List<CampaignBlock> Nexts => _nexts;
        public string Comment { get => _comment; set => _comment = value; }
        public string Name { get => _name; set => _name = value; }

        public CampaignBlock(List<CampaignBlock> nexts, string name = "", string comment = "")
        {
            _nexts = nexts ?? throw new ArgumentNullException(nameof(nexts));
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _comment = comment ?? throw new ArgumentNullException(nameof(comment));
        }
    }

    public sealed class Campaign : IDescribable
    {
        private List<CampaignBlock> _blocks;
        private CampaignBlock _startup;
        private List<Campaign> _endings;
        private string _comment;
        private string _name;

        public List<CampaignBlock> Blocks { get => _blocks; set => _blocks = value; }
        public CampaignBlock Startup { get => _startup; set => _startup = value; }
        public List<Campaign> Endings { get => _endings; set => _endings = value; }
        public string Comment { get => _comment; set => _comment = value; }
        public string Name { get => _name; set => _name = value; }

        private string _description;
        public string Description { get => _description; set => _description = value; }
        

        public Campaign()
        {

        }
        
    }
    
    public sealed class CampaignList
    {
        private List<Campaign> _campaigns;
        private Campaign _startup;

        public List<Campaign> Campaigns { get => _campaigns; set => _campaigns = value; }
        public Campaign Startup { get => _startup; set => _startup = value; }


    }

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
        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
