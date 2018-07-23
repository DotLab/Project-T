using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Container;

namespace GameLogic.Campaign
{
    public sealed class BattleShot : Shot
    {
        public override ShotType Type => ShotType.Battle;
        public override StoryShot Story => null;
        public override MapShot Map => null;
        public override BattleShot Battle => this;
        
        public BattleShot()
        {

        }

        public void InitBattleScene(BattleSceneContainer scene)
        {

        }
    }
    
}
