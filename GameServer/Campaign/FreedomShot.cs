using GameLib.Container.StoryComponent;
using GameLib.Core;
using GameLib.Utilities;
using System.Collections.Generic;

namespace GameLib.Campaign {
	public sealed class FreedomShot : Shot {
		private readonly IdentifiedObjList<ISceneObject> _objList;
		private readonly IdentifiedObjList<ISceneObject> _objInSceneList;
		private readonly List<FreedomShot> _otherPlaces;
		private readonly List<Layout> _pointsLayout;

		public override ShotType Type => ShotType.FREEDOM;
		public override StoryShot Story => null;
		public override BattleShot Battle => null;
		public override FreedomShot Freedom => this;

		public IdentifiedObjList<ISceneObject> ObjList => _objList;
		public IdentifiedObjList<ISceneObject> ObjInSceneList => _objInSceneList;
		public List<FreedomShot> OtherPlaces => _otherPlaces;
		public List<Layout> PointsLayout => _pointsLayout;

		public FreedomShot(bool inversionLoad) {
			_objList = new IdentifiedObjList<ISceneObject>();
			_objInSceneList = new IdentifiedObjList<ISceneObject>();
			_otherPlaces = new List<FreedomShot>();
			_pointsLayout = new List<Layout>();
			if (inversionLoad) {

			}
		}
	}
}
