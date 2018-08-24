using GameLib.CharacterSystem;
using GameLib.Container.BattleComponent;
using GameLib.Utilities.Network;

namespace GameLib.ClientComponents {
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

		public static BattleSceneGridObjData CreateBattleSceneGridObjData(GridObject gridObject) {
			var ret = new BattleSceneGridObjData();
			ret.obj.row = gridObject.GridRef.PosRow;
			ret.obj.col = gridObject.GridRef.PosCol;
			ret.highland = gridObject.IsHighland;
			ret.obj.id = gridObject.ID;
			ret.direction = (int)gridObject.Direction;
			ret.obstacle = gridObject.IsObstacle;
			ret.stagnate = gridObject.Stagnate;
			ret.terrain = gridObject.IsTerrain;
			bool actable = ret.actable = gridObject is ActableGridObject;
			if (actable) {
				var actableObject = (ActableGridObject)gridObject;
				ret.actableObjData.actionPoint = actableObject.ActionPoint;
				ret.actableObjData.movable = actableObject.Movable;
				ret.actableObjData.movePoint = actableObject.MovePoint;
			}
			return ret;
		}

		public static BattleSceneLadderObjData CreateBattleSceneLadderObjData(LadderObject ladderObject) {
			var ret = new BattleSceneLadderObjData();
			ret.obj.row = ladderObject.GridRef.PosRow;
			ret.obj.col = ladderObject.GridRef.PosCol;
			ret.obj.id = ladderObject.ID;
			ret.direction = (int)ladderObject.DirectionOnFirstGrid;
			return ret;
		}
	}
}
