using GameLogic.Container;

namespace GameLogic.Campaign {
	public sealed class BattleShot : Shot {
		public override ShotType Type => ShotType.BATTLE;
		public override StoryShot Story => null;
		public override FreedomShot Freedom => null;
		public override BattleShot Battle => this;

		public BattleShot() {

		}

		public void InitBattleScene(BattleSceneContainer scene) {

		}
	}

}
