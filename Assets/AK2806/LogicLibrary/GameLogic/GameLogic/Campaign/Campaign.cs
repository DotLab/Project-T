using System;
using System.Collections.Generic;
using GameLogic.Core;

namespace GameLogic.Campaign
{
    public enum CBType
    {
        Story, Battle
    }

    public interface ICampaignBlock
    {
        CBType Type { get; }
        IStory Story { get; }
        IBattle Battle { get; }
        List<ICampaignBlock> Nexts { get; }
    }

    public sealed class Campaign : IDescribable
    {
        private List<ICampaignBlock> _blocks;
        private ICampaignBlock _startup;
        private List<Campaign> _endings;

        public List<ICampaignBlock> Blocks { get => _blocks; set => _blocks = value; }
        public ICampaignBlock Startup { get => _startup; set => _startup = value; }
        public List<Campaign> Endings { get => _endings; set => _endings = value; }

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
}
