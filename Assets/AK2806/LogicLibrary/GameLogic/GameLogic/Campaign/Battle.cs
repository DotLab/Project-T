using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Scene;

namespace GameLogic.Campaign
{
    public interface IBattle
    {
        void InitBattleScene(BattleScene scene);
    }

    public abstract class AbstractBattle : IBattle, ICampaignBlock
    {
        public CBType Type => CBType.Battle;

        public IStory Story => null;

        public IBattle Battle => this;

        public abstract List<ICampaignBlock> Nexts { get; }

        public abstract void InitBattleScene(BattleScene scene);
    }

    public class Battle : AbstractBattle
    {
        public override List<ICampaignBlock> Nexts => throw new NotImplementedException();

        public override void InitBattleScene(BattleScene scene)
        {
            throw new NotImplementedException();
        }
    }
}
