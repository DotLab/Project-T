using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Campaign
{
    public class Movie : CampaignBlock
    {
        public override CBType Type => CBType.Movie;
        public override Story StoryBlock => null;
        public override Battle BattleBlock => null;
        public override Movie MovieBlock => this;

        public Movie(List<CampaignBlock> nexts) :
            base(nexts)
        {

        }
    }
}
