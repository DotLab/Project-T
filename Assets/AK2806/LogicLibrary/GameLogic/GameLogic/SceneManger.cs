using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameLogic
{
    public struct SceneListFile { // persistent
        public struct Router {
            public struct From {
                public string scene;
                public string outcome; // reference to the outcome’s name in scene file
            }

            public From from;
            public string to; // reference to the name of scene
        }

        public string fileType;
        public List<Scene.SceneFile> scenes;
        public string startup; // reference to the name of scene
        public List<Router> routers;
    }

    public class SceneManger
    {
        private SceneListFile mSceneList;

        public SceneManger()
        {
        }

        public void Load(string json) {
            this.mSceneList = JsonConvert.DeserializeObject<SceneListFile>(json);
        }
    }
}
