﻿using GameUtil;
using System;
using System.Collections.Generic;

namespace GameServer.CharacterSystem {
	public struct SkillSituationLimit {
		public static readonly SkillSituationLimit INIT = new SkillSituationLimit() {
			usableSituation = CharacterAction.CREATE_ASPECT | CharacterAction.HINDER,
			resistableSituation = CharacterAction.CREATE_ASPECT | CharacterAction.HINDER,
			canUseOnInteract = true,
			damageMental = false
		};
		public CharacterAction usableSituation;
		public CharacterAction resistableSituation;
		public bool canUseOnInteract;
		public bool damageMental;
	}

	public struct StuntSituationLimit {
		public static readonly StuntSituationLimit INIT = new StuntSituationLimit() {
			canUseOnInteract = false,
			usableSituation = 0,
			resistableSituation = 0
		};
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
