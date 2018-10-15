using GameServer.Playground.StoryComponent;
using GameServer.Core;
using GameUtil;
using System.Collections.Generic;

namespace GameServer.Campaign {
	public sealed class FreedomShot : Shot {
		private readonly IdentifiedObjectList<ISceneObject> _objList;
		private readonly IdentifiedObjectList<ISceneObject> _objInSceneList;
		private readonly List<FreedomShot> _otherPlaces;
		private readonly List<Layout> _pointsLayout;

		public override ShotType Type => ShotType.FREEDOM;
		public override StoryShot Story => null;
		public override BattleShot Battle => null;
		public override FreedomShot Freedom => this;

		public IdentifiedObjectList<ISceneObject> ObjList => _objList;
		public IdentifiedObjectList<ISceneObject> ObjInSceneList => _objInSceneList;
		public List<FreedomShot> OtherPlaces => _otherPlaces;
		public List<Layout> PointsLayout => _pointsLayout;

		public FreedomShot(bool inversionLoad) {
			_objList = new IdentifiedObjectList<ISceneObject>();
			_objInSceneList = new IdentifiedObjectList<ISceneObject>();
			_otherPlaces = new List<FreedomShot>();
			_pointsLayout = new List<Layout>();
			if (inversionLoad) {

			}
		}
	}
}
