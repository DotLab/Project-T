using GameLib.Campaign;
using GameLib.CharacterSystem;
using GameLib.Container;
using GameLib.Core.ScriptSystem;
using GameLib.EventSystem;
using GameLib.Utilities;
using System.Collections.Generic;

namespace GameLib.Core {
	public class JavascriptGlobalObject {
		public IJSAPI<CharacterManager> characterManager = (IJSAPI<CharacterManager>)CharacterManager.Instance.GetContext();
		public IJSAPI<CampaignManager> campaignManager = (IJSAPI<CampaignManager>)CampaignManager.Instance.GetContext();
		public IJSAPI<StorySceneContainer> storyScene = (IJSAPI<StorySceneContainer>)StorySceneContainer.Instance.GetContext();
		public IJSAPI<BattleSceneContainer> battleScene = (IJSAPI<BattleSceneContainer>)BattleSceneContainer.Instance.GetContext();
		public IJSAPI<GameEventBus> gameEventBus = (IJSAPI<GameEventBus>)GameEventBus.Instance.GetContext();
		
	}

	public static class Game {
		private static readonly JavascriptGlobalObject _globalObject = new JavascriptGlobalObject();

		private static bool _gameOver = true;
		private static List<Player> _players = null;
		private static DM _dm = null;

		public static bool GameOver => _gameOver;
		public static IReadOnlyList<Player> Players => _players;
		public static DM DM => _dm;

		public static void Init(DM dm, IEnumerable<Player> players) {
			//Logger.ApplyLogger();

			_dm = dm;
			_players = new List<Player>(players);

			foreach (Player player in _players) {
				foreach (Character character in player.Characters) {
					CharacterManager.Instance.PlayerCharacters.Add(character);
				}
			}

			IJSEngineRaw engineRaw = JSEngineManager.EngineRaw;
			engineRaw.BindType(nameof(Vec2), typeof(Vec2));
			engineRaw.BindType(nameof(Vec3), typeof(Vec3));
			engineRaw.BindType(nameof(Vec4), typeof(Vec4));
			engineRaw.BindType(nameof(Range), typeof(Range));
			engineRaw.BindType(nameof(CharacterView), typeof(CharacterView));
			engineRaw.BindType(nameof(Layout), typeof(Layout));
			engineRaw.BindType(nameof(GridPos), typeof(GridPos));
			engineRaw.BindType(nameof(CameraEffect), typeof(CameraEffect));
			engineRaw.BindType(nameof(CharacterViewEffect), typeof(CharacterViewEffect));
			engineRaw.BindType(nameof(PortraitStyle), typeof(PortraitStyle));
			engineRaw.BindType(nameof(SkillSituationLimit), typeof(SkillSituationLimit));
			engineRaw.BindType(nameof(StuntSituationLimit), typeof(StuntSituationLimit));
			engineRaw.BindType(nameof(BattleMapSkillProperty), typeof(BattleMapSkillProperty));
			engineRaw.SetVar("$", _globalObject);



			_gameOver = false;
		}

		public static void Update() {
			foreach (User player in _players) {
				player.UpdateClient();
			}
			_dm.UpdateClient();
		}

		public static void Cleanup() {

		}
	}
}
