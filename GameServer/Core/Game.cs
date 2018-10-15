using GameServer.Campaign;
using GameServer.CharacterComponents;
using GameServer.Playground;
using GameServer.Core.ScriptSystem;
using GameServer.EventSystem;
using GameUtil;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GameServer.Core {
	public class JavascriptGlobalObject {
		public IJSAPI<CharacterManager> characterManager = (IJSAPI<CharacterManager>)CharacterManager.Instance.GetContext();
		public IJSAPI<CampaignManager> campaignManager = (IJSAPI<CampaignManager>)CampaignManager.Instance.GetContext();
		public IJSAPI<StoryScene> storyScene = (IJSAPI<StoryScene>)StoryScene.Instance.GetContext();
		public IJSAPI<BattleScene> battleScene = (IJSAPI<BattleScene>)BattleScene.Instance.GetContext();
		public IJSAPI<GameEventBus> gameEventBus = (IJSAPI<GameEventBus>)GameEventBus.Instance.GetContext();
		
	}

	public static class Game {
		private static readonly JavascriptGlobalObject _globalObject = new JavascriptGlobalObject();

		private static bool _gameOver = true;
		private static List<Player> _players = null;
		private static DM _dm = null;

		public static bool GameOver { get => _gameOver; set => _gameOver = value; }
		public static IReadOnlyList<Player> Players => _players;
		public static DM DM => _dm;

		public static void InitGame(DM dm, IEnumerable<Player> players) {
			//Logger.ApplyLogger();

			_dm = dm;
			_players = new List<Player>(players);

			foreach (Player player in _players) {
				CharacterManager.Instance.PlayerCharacters.Add(player.Character);
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
			engineRaw.BindType(nameof(SkillBattleMapProperty), typeof(SkillBattleMapProperty));
			engineRaw.SetVar("$", _globalObject);



			_gameOver = false;
		}
		
		public static void ListenClientProxy(Func<bool> stopCondition = null) {
			while (!_gameOver) {
				if (stopCondition != null && stopCondition()) break;
				foreach (User player in _players) {
					player.UpdateClientProxy();
				}
				_dm.UpdateClientProxy();
				Thread.Sleep(100);
			}
		}
	}
}
