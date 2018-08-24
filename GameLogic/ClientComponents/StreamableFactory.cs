using GameLogic.CharacterSystem;
using GameLogic.Container.BattleComponent;
using GameLogic.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.ClientComponents {
	public static class StreamableFactory {
		public static SkillTypeDescription CreateSkillTypeDescription(SkillType skillType) {
			var ret = new SkillTypeDescription() {
				id = skillType.ID,
				name = skillType.Name
			};
			return ret;
		}
		
		public static CharacterPropertyDescription CreateCharacterPropertyDescription(ICharacterProperty characterProperty) {
			var ret = new CharacterPropertyDescription() {
				propertyID = characterProperty.ID,
				describable = new Describable(characterProperty)
			};
			return ret;
		}

		public static CharacterPropertyDescription CreateCharacterPropertyDescription(Skill skill) {
			var ret = new CharacterPropertyDescription() {
				propertyID = skill.SkillType.ID,
				describable = new Describable(skill)
			};
			return ret;
		}
		
		public static BattleSceneObj CreateBattleSceneObj(GridObject gridObject) {
			var ret = new BattleSceneObj() {
				row = gridObject.GridRef.PosRow,
				col = gridObject.GridRef.PosCol,
				id = gridObject.ID
			};
			return ret;
		}

		public static BattleSceneObj CreateBattleSceneObj(LadderObject ladderObject) {
			var ret = new BattleSceneObj() {
				row = ladderObject.GridRef.PosRow,
				col = ladderObject.GridRef.PosCol,
				id = ladderObject.ID
			};
			return ret;
		}
	}
}
