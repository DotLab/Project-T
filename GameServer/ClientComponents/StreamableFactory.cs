using GameLib.CharacterSystem;
using GameLib.Container.BattleComponent;
using GameLib.Core;
using GameLib.Utilities.Network.Streamable;

namespace GameLib.ClientComponents {
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
		
		public static BattleSceneObject CreateBattleSceneObj(SceneObject sceneObject) {
			var ret = new BattleSceneObject() {
				row = sceneObject.GridRef.PosRow,
				col = sceneObject.GridRef.PosCol,
				id = sceneObject.ID
			};
			return ret;
		}
		
		public static GridObjectData CreateBattleSceneGridObjData(GridObject gridObject) {
			var ret = new GridObjectData();
			ret.obj.row = gridObject.GridRef.PosRow;
			ret.obj.col = gridObject.GridRef.PosCol;
			ret.highland = gridObject.Highland;
			ret.obj.id = gridObject.ID;
			ret.direction = gridObject.Direction;
			ret.obstacle = gridObject.Obstacle;
			ret.stagnate = gridObject.Stagnate;
			ret.terrain = gridObject.Terrain;
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
			ret.obj.row = ladderObject.GridRef.PosRow;
			ret.obj.col = ladderObject.GridRef.PosCol;
			ret.obj.id = ladderObject.ID;
			ret.direction = ladderObject.DirectionOnFirstGrid;
			return ret;
		}
	}
}
