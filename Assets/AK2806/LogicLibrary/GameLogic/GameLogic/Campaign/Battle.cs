using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Scene;

namespace GameLogic.Campaign
{
    public class Battle : CampaignBlock
    {
        public override CBType Type => CBType.Battle;
        public override Story StoryBlock => null;
        public override Movie MovieBlock => null;
        public override Battle BattleBlock => this;
        
        public Battle(List<CampaignBlock> nexts) :
            base(nexts)
        {

        }

        public void InitBattleScene(BattleScene scene)
        {

        }
    }
    
}
