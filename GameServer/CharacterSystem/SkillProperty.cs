using GameUtil;
using System;
using System.Collections.Generic;

namespace GameServer.CharacterSystem {
	public struct SkillSituationLimit {
		public bool canAttack;
		public bool damageMental;
		public bool canDefend;
	}

	public struct StuntSituationLimit {
		public CharacterAction usableSituation;
		public CharacterAction resistableSituation;
		public bool canUseOnInteract;
	}

	public struct SkillBattleMapProperty {
		public static readonly SkillBattleMapProperty INIT = new SkillBattleMapProperty() {
			actionPointCost = 1,
			useRange = new Range() { lowOpen = false, low = 0, highOpen = false, high = 0 },
			affectRange = new Range { lowOpen = false, low = 0, highOpen = false, high = 0 },
			islinearUse = false,
			islinearAffect = false,
			linearAffectDirection = BattleMapDirection.POSITIVE_ROW | BattleMapDirection.POSITIVE_COL | BattleMapDirection.NEGATIVE_ROW | BattleMapDirection.NEGATIVE_COL,
			linearUseDirection = BattleMapDirection.POSITIVE_ROW | BattleMapDirection.POSITIVE_COL | BattleMapDirection.NEGATIVE_ROW | BattleMapDirection.NEGATIVE_COL,
			targetMaxCount = 1
		};

		public int actionPointCost;
		public Range useRange;
		public bool islinearUse;
		public BattleMapDirection linearUseDirection;
		public Range affectRange;
		public bool islinearAffect;
		public BattleMapDirection linearAffectDirection;
		public int targetMaxCount;
	}
}
