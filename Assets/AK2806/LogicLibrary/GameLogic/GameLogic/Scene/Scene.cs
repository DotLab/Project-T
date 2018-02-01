using System;
using System.Collections.Generic;

namespace GameLogic.Scene
{
    public struct SceneFile { // persistent
        public struct Action {
            public enum ActionType
            {
                Tween, Cutscene, SetBGImg, SetBGAnimation, AddImg, RmImg,
                AddAnimation, RmAnimation, SetEffect, AddChara, CharaBehave,
                PlayBGM, StopBGM, PlaySE, ShowTip, ShowMessage, Battle
            }

            public struct Next {
                public string condition;
                public int index;
                public string tip;
            }

            public ActionType actionType;
            public string param;
            public bool wait;
            public List<Next> nexts;
        }

        public string fileType;
        public List<Action> actions;
    }

    public class Scene
    {
        private SceneFile mScene;

        public Scene()
        {
        }
    }
}
