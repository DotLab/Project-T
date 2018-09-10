using GameServer.Campaign;
using GameServer.Core;
using GameUtil.Network;
using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
using System;

namespace GameServer.ClientComponents {
	public abstract class ClientComponent : IMessageReceiver {
		protected readonly Connection _connection;
		protected readonly User _owner;

		protected ClientComponent(Connection connection, User owner) {
			_connection = connection;
			_owner = owner;
		}

		public abstract void MessageReceived(Message message);
	}

	public class Client : ClientComponent {
		protected bool _isDetermining = false;
		protected Action<int> _determinCallback = null;

		protected readonly CharacterData _characterData;
		protected readonly StoryScene _storyScene;
		protected readonly BattleScene _battleScene;

		public StoryScene StoryScene => _storyScene;
		public BattleScene BattleScene => _battleScene;

		public override void MessageReceived(Message message) {
			if (message.MessageType == ClientInitMessage.MESSAGE_TYPE) {
				_battleScene.SynchronizeData();

				if (CampaignManager.Instance.CurrentContainer == ContainerType.BATTLE) {
					ShowScene(ContainerType.BATTLE);
					_battleScene.SynchronizeState();
				} else if (CampaignManager.Instance.CurrentContainer == ContainerType.STORY) {
					ShowScene(ContainerType.STORY);

				}
			} else if (message.MessageType == UserDeterminResultMessage.MESSAGE_TYPE) {
				var msg = (UserDeterminResultMessage)message;
				Action<int> callback = null;
				if (_determinCallback != null) callback = _determinCallback;
				_determinCallback = null;
				_isDetermining = false;
				if (callback != null) callback(msg.result);
			}
		}
		
		protected Client(Connection connection, User owner, StoryScene storyScene, BattleScene battleScene) :
			base(connection, owner) {
			_connection.AddMessageReceiver(ClientInitMessage.MESSAGE_TYPE, this);
			_connection.AddMessageReceiver(UserDeterminResultMessage.MESSAGE_TYPE, this);
			_characterData = new CharacterData(connection, owner);
			_storyScene = storyScene;
			_battleScene = battleScene;
		}

		public virtual void Update() {
			_connection.UpdateReceiver();
		}

		public void ShowScene(ContainerType scene) {
			switch (scene) {
				case ContainerType.BATTLE:
					_battleScene.Open();
					_storyScene.Close();
					break;
				case ContainerType.STORY:
					_battleScene.Close();
					_storyScene.Open();
					break;
				default:
					return;
			}
			ShowSceneMessage message = new ShowSceneMessage();
			message.sceneType = (byte)scene;
			_connection.SendMessage(message);
		}

		public void PlayBGM(string id) {
			PlayBGMMessage message = new PlayBGMMessage();
			message.bgmID = id;
			_connection.SendMessage(message);
		}

		public void StopBGM() {
			StopBGMMessage message = new StopBGMMessage();
			_connection.SendMessage(message);
		}

		public void PlaySE(string id) {
			PlaySEMessage message = new PlaySEMessage();
			message.seID = id;
			_connection.SendMessage(message);
		}

		public void RequestDetermin(string text, Action<int> result) {
			if (_isDetermining) {
				result(0);
				return;
			}
			_determinCallback = result;
			_isDetermining = true;
			var message = new UserDeterminMessage();
			message.text = text;
			_connection.SendMessage(message);
		}
	}

	public sealed class PlayerClient : Client {
		public PlayerStoryScene PlayerStoryScene => (PlayerStoryScene)_storyScene;
		public PlayerBattleScene PlayerBattleScene => (PlayerBattleScene)_battleScene;
		
		public PlayerClient(Connection connection, Player owner) :
			base(connection, owner, new PlayerStoryScene(connection, owner), new PlayerBattleScene(connection, owner)) {

		}
	}

	public sealed class DMClient : Client {
		private bool _isChecking = false;
		private Action<bool> _resultCallback = null;

		public DMStoryScene DMStoryScene => (DMStoryScene)_storyScene;
		public DMBattleScene DMBattleScene => (DMBattleScene)_battleScene;

		public override void MessageReceived(Message message) {
			base.MessageReceived(message);
			var msg = message as DMCheckResultMessage;
			Action<bool> callback = null;
			if (msg != null) {
				if (_resultCallback != null) callback = _resultCallback;
				_resultCallback = null;
				_isChecking = false;
			}
			if (callback != null) callback(msg.result);
		}

		public DMClient(Connection connection, DM owner) :
			base(connection, owner, new DMStoryScene(connection, owner), new DMBattleScene(connection, owner)) {
			_connection.AddMessageReceiver(DMCheckResultMessage.MESSAGE_TYPE, this);
		}

		public void RequestDMCheck(User invoker, string text, Action<bool> result) {
			if (_isChecking) {
				result(false);
				return;
			}
			if (invoker.IsDM) {
				result(true);
				return;
			}
			_resultCallback = result;
			_isChecking = true;
			var message = new DMCheckMessage();
			message.text = text;
			_connection.SendMessage(message);
		}
	}

}
