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
        private static readonly JavascriptGlobalObject _globalObject = new JavascriptGlobalObject();

        private static List<User> _players = null;
        private static User _dm = null;

        public static IEnumerable<User> Players => _players;
        public static User Dm => _dm;

        public static void Init(User dm, IEnumerable<User> players)
        {
            _dm = dm ?? throw new ArgumentNullException(nameof(dm));
            _players = new List<User>(players);

            foreach (User user in _players)
            {
                foreach (Character character in user.AsPlayer.Characters)
                {
                    CharacterManager.Instance.PlayerCharacters.Add(character);
                }
            }

            IJSEngineRaw engineRaw = JSEngineManager.EngineRaw;
            engineRaw.BindType(nameof(CharacterView), typeof(CharacterView));
            engineRaw.BindType(nameof(CameraEffect), typeof(CameraEffect));
            engineRaw.BindType(nameof(Layout), typeof(Layout));
            engineRaw.BindType(nameof(PortraitStyle), typeof(PortraitStyle));
            engineRaw.BindType(nameof(CharacterViewEffect), typeof(CharacterViewEffect));
            engineRaw.BindType(nameof(Vector3), typeof(Vector3));
            engineRaw.BindType(nameof(Quaternion), typeof(Quaternion));
            engineRaw.SetVar("$", _globalObject);
            
            

            _gameOver = false;
        }

        public static bool GameOver => _gameOver;

        public static void Update()
        {
            foreach (User player in _players)
            {
                player.UpdateClient();
            }
            _dm.UpdateClient();
        }

        public static void Cleanup()
        {

        }
    }
}
