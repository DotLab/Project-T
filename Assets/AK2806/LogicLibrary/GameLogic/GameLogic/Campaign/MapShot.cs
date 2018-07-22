using GameLogic.Core;
using GameLogic.Scene.Story;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Campaign
{
    public sealed class MapShot : Shot
    {
        private readonly IdentifiedObjList<ISceneObject> _objList;
        private readonly IdentifiedObjList<ISceneObject> _objInSceneList;
        private readonly List<MapShot> _otherPlaces;
        private readonly List<Layout> _pointsLayout;
        private readonly View _background;

        public override ShotType Type => ShotType.Map;
        public override StoryShot Story => null;
        public override BattleShot Battle => null;
        public override MapShot Map => this;

        public IdentifiedObjList<ISceneObject> ObjList => _objList;
        public IdentifiedObjList<ISceneObject> ObjInSceneList => _objInSceneList;
        public List<MapShot> OtherPlaces => _otherPlaces;
        public List<Layout> PointsLayout => _pointsLayout;
        public View Background => _background;

        public MapShot(View background, bool inversionLoad)
        {
            _objList = new IdentifiedObjList<ISceneObject>();
            _objInSceneList = new IdentifiedObjList<ISceneObject>();
            _otherPlaces = new List<MapShot>();
            _pointsLayout = new List<Layout>();
            _background = background ?? throw new ArgumentNullException(nameof(background));
            if (inversionLoad)
            {
                
            }
        }
    }
}
