﻿using System;
using System.Collections.Generic;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.Campaign
{
    public enum ShotType
    {
        Story, Battle, Map
    }

    public abstract class Shot : IDescribable
    {
        protected List<Shot> _nexts;
        protected string _description;
        protected string _name;

        public abstract ShotType Type { get; }
        public abstract StoryShot Story { get; }
        public abstract BattleShot Battle { get; }
        public abstract MapShot Map { get; }

        public List<Shot> Nexts { get => _nexts; set => _nexts = value; }
        public string Description { get => _description; set => _description = value; }
        public string Name { get => _name; set => _name = value; }

        public Shot(string name = "", string description = "")
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }

    public sealed class Campaign : IDescribable
    {
        private Dictionary<string, Shot> _shots;
        private Dictionary<string, Shot> _backupShots;
        private Shot _startup;
        private List<Campaign> _nexts;
        private string _description;
        private string _name;

        public Dictionary<string, Shot> Shots { get => _shots; set => _shots = value; }
        public Dictionary<string, Shot> BackupShots { get => _backupShots; set => _backupShots = value; }
        public Shot Startup { get => _startup; set => _startup = value; }
        public List<Campaign> Nexts { get => _nexts; set => _nexts = value; }
        public string Description { get => _description; set => _description = value; }
        public string Name { get => _name; set => _name = value; }

        public void JumpTo(string name)
        {

        }

        public void UseBackupShot(string name)
        {

        }

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
        private sealed class API : IJSAPI<CampaignManager>
        {
            private CampaignManager _outer;

            public API(CampaignManager outer)
            {
                _outer = outer;
            }

            public CampaignManager Origin(JSContextHelper proof)
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

        private Shot _currentCampaign;
        private Shot _currentBlock;

        private CampaignManager()
        {
            _apiObj = new API(this);
        }

        public Shot CurrentCampaign { get => _currentCampaign; set => _currentCampaign = value; }
        public Shot CurrentBlock { get => _currentBlock; set => _currentBlock = value; }
        
        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
