using GameLogic.Campaign;
using GameLogic.CharacterSystem;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;
using GameLogic.Scene;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameLogic.Core
{
    public class JavascriptGlobalObject
    {
        public IJSAPI characterManager = (IJSAPI)CharacterManager.Instance.GetContext();
        public IJSAPI campaignManager = (IJSAPI)CampaignManager.Instance.GetContext();
        public IJSAPI storyScene = (IJSAPI)StoryScene.Instance.GetContext();
        public IJSAPI battleScene = (IJSAPI)BattleScene.Instance.GetContext();
        public IJSAPI gameEventBus = (IJSAPI)GameEventBus.Instance.GetContext();

    }

    public class MainLogic
    {
        private static bool _gameOver = true;
        private static readonly JavascriptGlobalObject globalObject = new JavascriptGlobalObject();

        public static void Init()
        {
            IJSEngineRaw engineRaw = JSEngineManager.EngineRaw;
            engineRaw.BindType("Layout", typeof(Layout));
            engineRaw.BindType("PortraitStyle", typeof(PortraitStyle));
            engineRaw.BindType("StoryViewEffect", typeof(StoryViewEffect));
            engineRaw.BindType("Vec3", typeof(Vector3));
            engineRaw.BindType("Quat", typeof(Quaternion));
            engineRaw.SetVar("$", globalObject);
            
            _gameOver = false;
        }

        public static bool IsGameover()
        {
            return _gameOver;
        }

        public static void Loop()
        {

        }
    }
}
