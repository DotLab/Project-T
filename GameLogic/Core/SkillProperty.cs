using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core {
	public enum BattleMapDirection {
		POSITIVE_ROW = 0b0001,
		POSITIVE_COL = 0b0010,
		NEGATIVE_ROW = 0b0100,
		NEGATIVE_COL = 0b1000
	}

	public struct SkillProperty {
		public static readonly SkillProperty INIT = new SkillProperty {
			level = 0,
			canAttack = false,
			canDefend = false,
			useRange = new Range { lowOpen = false, low = 0, highOpen = false, high = 0 },
			affectRange = new Range { lowOpen = false, low = 0, highOpen = false, high = 0 },
			islinearUse = false,
			islinearAffect = false,
			linearAffectDirection = BattleMapDirection.POSITIVE_ROW & BattleMapDirection.POSITIVE_COL & BattleMapDirection.NEGATIVE_ROW & BattleMapDirection.NEGATIVE_COL,
			linearUseDirection = BattleMapDirection.POSITIVE_ROW & BattleMapDirection.POSITIVE_COL & BattleMapDirection.NEGATIVE_ROW & BattleMapDirection.NEGATIVE_COL,
			targetCount = 1
		};

		public int level;
		public bool canAttack;
		public bool canDefend;
		public Range useRange;
		public bool islinearUse;
		public BattleMapDirection linearUseDirection;
		public Range affectRange;
		public bool islinearAffect;
		public BattleMapDirection linearAffectDirection;
		public int targetCount;
	}
}
