using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameLogic.Framework
{
    public class SceneListFile { // persistent
        public struct Router {
            public struct From {
                public string scene;
                public string outcome; // reference to the outcome’s name in scene file
            }

            public From from;
            public string to; // reference to the name of scene
        }

        public struct SceneFileInfo {
            public string name;
            public string path;
        }

        public string fileType;
        public List<SceneFileInfo> scenes;
        public string startup; // reference to the name of scene
        public List<Router> routers;
    }

    public class SceneManger
    {
        private SceneListFile mSceneList = null;

        public SceneManger()
        {
        }

        public SceneManger(string json) : this() {
            this.Load(json);
        }

        public void Load(string json) {
            this.mSceneList = JsonConvert.DeserializeObject<SceneListFile>(json);
        }
    }
}
