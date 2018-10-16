using GameServer.Core;
using GameServer.Playground;

namespace GameServer.Campaign {
	public sealed class BattleShot : Shot {
		private readonly Command _initCommand;

		public override ShotType Type => ShotType.BATTLE;
		public override StoryShot Story => null;
		public override InvestigationShot Investigation => null;
		public override BattleShot Battle => this;

		public BattleShot(Command initCommand) {
			_initCommand = initCommand;
		}

		public void InitBattleScene() {
			_initCommand.DoAction();
		}
	}

}
