using GameLogic.Campaign;
using GameLogic.CharacterSystem;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;
using GameLogic.Container;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameLogic.Core
{
    public class JavascriptGlobalObject
    {
        public IJSAPI<CharacterManager> characterManager = (IJSAPI<CharacterManager>)CharacterManager.Instance.GetContext();
        public IJSAPI<CampaignManager> campaignManager = (IJSAPI<CampaignManager>)CampaignManager.Instance.GetContext();
        public IJSAPI<StorySceneContainer> storyScene = (IJSAPI<StorySceneContainer>)StorySceneContainer.Instance.GetContext();
        public IJSAPI<BattleSceneContainer> battleScene = (IJSAPI<BattleSceneContainer>)BattleSceneContainer.Instance.GetContext();
        public IJSAPI<GameEventBus> gameEventBus = (IJSAPI<GameEventBus>)GameEventBus.Instance.GetContext();

    }

    public class MainLogic
    {
        private static bool _gameOver = true;
        private static readonly JavascriptGlobalObject globalObject = new JavascriptGlobalObject();

        private static List<User> users = new List<User>();

        public static void Init()
        {
            IJSEngineRaw engineRaw = JSEngineManager.EngineRaw;
            engineRaw.BindType(nameof(CharacterView), typeof(CharacterView));
            engineRaw.BindType(nameof(Layout), typeof(Layout));
            engineRaw.BindType(nameof(PortraitStyle), typeof(PortraitStyle));
            engineRaw.BindType(nameof(CharacterViewEffect), typeof(CharacterViewEffect));
            engineRaw.BindType(nameof(Vector3), typeof(Vector3));
            engineRaw.BindType(nameof(Quaternion), typeof(Quaternion));
            engineRaw.SetVar("$", globalObject);
            
            

            _gameOver = false;
        }

        public static bool GameOver => _gameOver;

        public static void Update()
        {

        }
    }
}
