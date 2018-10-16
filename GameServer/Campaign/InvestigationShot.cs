using GameServer.Playground.StoryComponent;
using GameServer.Core;
using GameUtil;
using System.Collections.Generic;

namespace GameServer.Campaign {
	public sealed class InvestigationShot : Shot {
		private readonly StoryAction _actionOnLeaved;

		public override ShotType Type => ShotType.INVESTIGATION;
		public override StoryShot Story => null;
		public override BattleShot Battle => null;
		public override InvestigationShot Investigation => this;
		
		public InvestigationShot(StoryAction actionOnLeaved) {
			_actionOnLeaved = actionOnLeaved;
		}
	}
}
