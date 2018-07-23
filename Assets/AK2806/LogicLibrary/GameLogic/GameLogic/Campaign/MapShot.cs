using GameLogic.Core;
using GameLogic.Container.Story;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Campaign
{
    public sealed class MapShot : Shot
    {
        private readonly IdentifiedObjList<IStoryObject> _objList;
        private readonly IdentifiedObjList<IStoryObject> _objInSceneList;
        private readonly List<MapShot> _otherPlaces;
        private readonly List<Layout> _pointsLayout;

        public override ShotType Type => ShotType.Map;
        public override StoryShot Story => null;
        public override BattleShot Battle => null;
        public override MapShot Map => this;

        public IdentifiedObjList<IStoryObject> ObjList => _objList;
        public IdentifiedObjList<IStoryObject> ObjInSceneList => _objInSceneList;
        public List<MapShot> OtherPlaces => _otherPlaces;
        public List<Layout> PointsLayout => _pointsLayout;

        public MapShot(bool inversionLoad)
        {
            _objList = new IdentifiedObjList<IStoryObject>();
            _objInSceneList = new IdentifiedObjList<IStoryObject>();
            _otherPlaces = new List<MapShot>();
            _pointsLayout = new List<Layout>();
            if (inversionLoad)
            {
                
            }
        }
    }
}
