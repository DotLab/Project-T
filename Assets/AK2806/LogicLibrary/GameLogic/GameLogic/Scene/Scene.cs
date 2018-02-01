using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameLogic.Scene
{
    public class SceneFile { // persistent
        public struct Action {
            public enum ActionType {
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
        private SceneFile mScene = null;
        private List<IRenderer> renderers = null;

        public Scene()
        {
            this.renderers = new List<IRenderer>();
        }

        public Scene(string json) : this() {
            this.Load(json);
        }

        public void Load(string json) {
            this.mScene = JsonConvert.DeserializeObject<SceneFile>(json);
        }

        public void AddRenderer(IRenderer renderer) {
            this.renderers.Add(renderer);
        }

        public void NextAction() {
            
        }
    }
}
