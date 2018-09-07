using GameLib.Utilities;
using System;
using System.Collections.Generic;

namespace GameLib.CharacterSystem {
	public struct SkillSituationLimit {
		public bool canAttack;
		public bool damageMental;
		public bool canDefend;
	}

	public struct StuntSituationLimit {
		public CharacterAction usableSituation;
		public CharacterAction resistableSituation;
		public bool canUseInSpecialAction;
	}

	public struct BattleMapSkillProperty {
		public static readonly BattleMapSkillProperty INIT = new BattleMapSkillProperty {
			actionPointCost = 1,
			useRange = new Range { lowOpen = false, low = 0, highOpen = false, high = 0 },
			affectRange = new Range { lowOpen = false, low = 0, highOpen = false, high = 0 },
			islinearUse = false,
			islinearAffect = false,
			linearAffectDirection = BattleMapDirection.POSITIVE_ROW | BattleMapDirection.POSITIVE_COL | BattleMapDirection.NEGATIVE_ROW | BattleMapDirection.NEGATIVE_COL,
			linearUseDirection = BattleMapDirection.POSITIVE_ROW | BattleMapDirection.POSITIVE_COL | BattleMapDirection.NEGATIVE_ROW | BattleMapDirection.NEGATIVE_COL,
			targetCount = 1
		};

		public int actionPointCost;
		public Range useRange;
		public bool islinearUse;
		public BattleMapDirection linearUseDirection;
		public Range affectRange;
		public bool islinearAffect;
		public BattleMapDirection linearAffectDirection;
		public int targetCount;
	}
}
