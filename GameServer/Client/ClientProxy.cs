using GameServer.Campaign;
using GameServer.Core;
using GameUtil.Network;
using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace GameServer.Client {
	public abstract class ClientComponentProxy : IMessageReceiver {
		protected readonly Connection _connection;
		protected readonly User _owner;

		public abstract bool IsUsing { get; }

		protected ClientComponentProxy(Connection connection, User owner) {
			_connection = connection;
			_owner = owner;
		}
		
		public abstract void WaitUserDecision(bool enabled);
		public abstract void MessageReceived(Message message);
	}

	public class ClientProxy : ClientComponentProxy, IRequestHandler {
		protected readonly CharacterData _characterData;
		protected readonly StorySceneProxy _storyScene;
		protected readonly BattleSceneProxy _battleScene;
		protected readonly SkillCheckerProxy _skillChecker;

		private bool _deciding = false;
		private int _decision = 0;
		
		public StorySceneProxy StoryScene => _storyScene;
		public BattleSceneProxy BattleScene => _battleScene;
		public SkillCheckerProxy SkillChecker => _skillChecker;

		public override bool IsUsing => true;

		public Message MakeResponse(Message request) {
			Message ret = null;
			if (request.MessageType == GetPlayerCharacterMessage.MESSAGE_TYPE) {
				var resp = new PlayerCharacterMessage();
				if (_owner.IsDM) {
					resp.characterID = "";
				} else {
					resp.characterID = _owner.AsPlayer.Character.ID;
				}
				ret = resp;
			}
			return ret;
		}

		public override void MessageReceived(Message message) {
			if (message.MessageType == ClientInitMessage.MESSAGE_TYPE) {
				_battleScene.SynchronizeData();

				if (CampaignManager.Instance.CurrentScene == SceneType.BATTLE) {
					ShowScene(SceneType.BATTLE);
					_battleScene.SynchronizeState();
				} else if (CampaignManager.Instance.CurrentScene == SceneType.STORY) {
					ShowScene(SceneType.STORY);

				}
			} else if (message.MessageType == UserDecisionMessage.MESSAGE_TYPE) {
				var msg = (UserDecisionMessage)message;
				_decision = msg.selectionIndex;
				_deciding = false;
				foreach (Player player in Game.Players) {
					player.Client.WaitUserDecision(false);
				}
				Game.DM.Client.WaitUserDecision(false);
			}
		}
		
		protected ClientProxy(Connection connection, User owner, StorySceneProxy storyScene, BattleSceneProxy battleScene) :
			base(connection, owner) {
			_connection.SetRequestHandler(GetPlayerCharacterMessage.MESSAGE_TYPE, this);
			_connection.AddMessageReceiver(ClientInitMessage.MESSAGE_TYPE, this);
			_connection.AddMessageReceiver(UserDecisionMessage.MESSAGE_TYPE, this);
			_characterData = new CharacterData(connection, owner);
			_storyScene = storyScene;
			_battleScene = battleScene;
			_skillChecker = new SkillCheckerProxy(connection, owner);
		}

		public void FlushUserInputBuffer() {
			_connection.FlushReceivingBuffer();
		}

		public void ShowScene(SceneType scene) {
			var message = new ShowSceneMessage();
			message.sceneType = (byte)scene;
			_connection.SendMessage(message);
		}

		public void DisplayDicePoints(User who, int[] dicePoints) {
			var message = new DisplayDicePointsMessage();
			message.dicePoints = dicePoints;
			message.userID = who.ID;
			_connection.SendMessage(message);
		}

		public void PlayBGM(string id) {
			var message = new PlayBGMMessage();
			message.bgmID = id;
			_connection.SendMessage(message);
		}

		public void StopBGM() {
			var message = new StopBGMMessage();
			_connection.SendMessage(message);
		}

		public void PlaySE(string id) {
			var message = new PlaySEMessage();
			message.seID = id;
			_connection.SendMessage(message);
		}

		public int RequireDetermin(string promptText, string[] selections) {
			var msg = new UserDecidingMessage();
			msg.promptText = promptText ?? throw new ArgumentNullException(nameof(promptText));
			msg.selections = selections;
			_connection.SendMessage(msg);
			_deciding = true;
			foreach (Player player in Game.Players) {
				player.Client.WaitUserDecision(true);
			}
			Game.DM.Client.WaitUserDecision(true);
			Game.ListenClientProxy(() => !_deciding);
			return _decision;
		}

		public override void WaitUserDecision(bool enabled) {
			var message = new WaitUserDecisionMessage();
			message.enabled = enabled;
			_connection.SendMessage(message);
			_storyScene.WaitUserDecision(enabled);
			_battleScene.WaitUserDecision(enabled);
		}
	}

	public sealed class PlayerClient : ClientProxy {
		public PlayerStoryScene PlayerStoryScene => (PlayerStoryScene)_storyScene;
		public PlayerBattleScene PlayerBattleScene => (PlayerBattleScene)_battleScene;
		
		public PlayerClient(Connection connection, Player owner) :
			base(connection, owner, new PlayerStoryScene(connection, owner), new PlayerBattleScene(connection, owner)) {

		}
	}

	public sealed class DMClient : ClientProxy {
		public DMStoryScene DMStoryScene => (DMStoryScene)_storyScene;
		public DMBattleScene DMBattleScene => (DMBattleScene)_battleScene;

		public DMClient(Connection connection, DM owner) :
			base(connection, owner, new DMStoryScene(connection, owner), new DMBattleScene(connection, owner)) {

		}

		public bool RequireDMCheck(User invoker, string promptText) {
			if (invoker == null) throw new ArgumentNullException(nameof(invoker));
			if (invoker.IsDM) return true;
			return RequireDetermin(promptText, new string[] { "是", "否" }) == 0;
		}
	}

}
