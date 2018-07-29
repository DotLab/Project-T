using GameLogic.Core;
using GameLogic.Container.StoryComponent;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Campaign
{
    public sealed class FreedomShot : Shot
    {
        private readonly IdentifiedObjList<IStoryObject> _objList;
        private readonly IdentifiedObjList<IStoryObject> _objInSceneList;
        private readonly List<FreedomShot> _otherPlaces;
        private readonly List<Layout> _pointsLayout;

        public override ShotType Type => ShotType.FREEDOM;
        public override StoryShot Story => null;
        public override BattleShot Battle => null;
        public override FreedomShot Freedom => this;

        public IdentifiedObjList<IStoryObject> ObjList => _objList;
        public IdentifiedObjList<IStoryObject> ObjInSceneList => _objInSceneList;
        public List<FreedomShot> OtherPlaces => _otherPlaces;
        public List<Layout> PointsLayout => _pointsLayout;

        public FreedomShot(bool inversionLoad)
        {
            _objList = new IdentifiedObjList<IStoryObject>();
            _objInSceneList = new IdentifiedObjList<IStoryObject>();
            _otherPlaces = new List<FreedomShot>();
            _pointsLayout = new List<Layout>();
            if (inversionLoad)
            {
                
            }
        }
    }
}
