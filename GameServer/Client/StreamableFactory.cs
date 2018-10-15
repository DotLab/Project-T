using GameServer.CharacterComponents;
using GameServer.Playground.BattleComponent;
using GameServer.Core;
using GameUtil.Network.Streamable;

namespace GameServer.Client {
	public static class StreamableFactory {
		public static Describable CreateDescribable(IDescribable describable) {
			var ret = new Describable() {
				name = describable.Name,
				description = describable.Description
			};
			return ret;
		}

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
				describable = CreateDescribable(characterProperty)
			};
			return ret;
		}
		
		public static GridObjectData CreateBattleSceneGridObjData(GridObject gridObject) {
			var ret = new GridObjectData();
			ret.id = gridObject.ID;
			ret.row = gridObject.GridRef.PosRow;
			ret.col = gridObject.GridRef.PosCol;
			ret.highland = gridObject.Highland;
			ret.direction = gridObject.Direction;
			ret.obstacle = gridObject.Obstacle;
			ret.stagnate = gridObject.Stagnate;
			bool actable = ret.actable = gridObject is ActableGridObject;
			if (actable) {
				var actableObject = (ActableGridObject)gridObject;
				ret.actableObjData.actionPoint = actableObject.ActionPoint;
				ret.actableObjData.actionPointMax = actableObject.ActionPointMax;
				ret.actableObjData.movable = actableObject.Movable;
				ret.actableObjData.movePoint = actableObject.MovePoint;
			}
			return ret;
		}

		public static LadderObjectData CreateBattleSceneLadderObjData(LadderObject ladderObject) {
			var ret = new LadderObjectData();
			ret.id = ladderObject.ID;
			ret.row = ladderObject.GridRef.PosRow;
			ret.col = ladderObject.GridRef.PosCol;
			ret.direction = ladderObject.DirectionOnFirstGrid;
			return ret;
		}
	}
}
