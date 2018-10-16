using GameServer.CharacterComponents;
using GameServer.Playground;
using GameServer.Playground.StoryComponent;
using GameServer.Core;
using GameServer.Core.DataSystem;
using GameUtil;
using GameUtil.Network;
using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
using System;
using GameServer.Campaign;

namespace GameServer.Client {
	public sealed class TextBox : ClientComponentProxy {
		private bool _ignoreOperating = false;

		public override bool IsUsing => throw new NotImplementedException();

		public override void MessageReceived(Message message) {
			TextSelectedMessage selectedMessage = (TextSelectedMessage)message;
			this.OnSelectItem(selectedMessage.selection);
		}

		private void OnSelectItem(int selection) {

		}

		public TextBox(Connection connection, User owner) :
			base(connection, owner) {
			_connection.AddMessageReceiver(TextSelectedMessage.MESSAGE_TYPE, this);
		}

		public void AppendParagraph(string text) {
			TextBoxAddParagraphMessage message = new TextBoxAddParagraphMessage();
			message.text = text;
			_connection.SendMessage(message);
		}

		public void AppendSelectableParagraph(string text, int selection) {
			TextBoxAddSelectionMessage message = new TextBoxAddSelectionMessage();
			message.text = text;
			message.selectionCode = selection;
			_connection.SendMessage(message);
		}

		public void Clear() {
			TextBoxClearMessage message = new TextBoxClearMessage();
			_connection.SendMessage(message);
		}

		public void SetCharacterView(CharacterView view) {
			TextBoxSetPortraitMessage message = new TextBoxSetPortraitMessage();
			message.view = view;
			_connection.SendMessage(message);
		}

		public void SetCharacterViewEffect(CharacterViewEffect effect) {
			TextBoxPortraitEffectMessage message = new TextBoxPortraitEffectMessage();
			message.effect = effect;
			_connection.SendMessage(message);
		}

		public void SetPortraitStyle(PortraitStyle portrait) {
			TextBoxPortraitStyleMessage message = new TextBoxPortraitStyleMessage();
			message.style = portrait;
			_connection.SendMessage(message);
		}

		public override void WaitUserDecision(bool enabled) {
			_ignoreOperating = enabled;
		}
	}

	public class StorySceneProxy : ClientComponentProxy {
		protected readonly TextBox _textBox;
		protected bool _ignoreOperating = false;

		public TextBox TextBox => _textBox;

		public override bool IsUsing => CampaignManager.Instance.CurrentScene == SceneType.STORY;

		public override void MessageReceived(Message message) { }

		protected StorySceneProxy(Connection connection, User owner) :
			base(connection, owner) {
			_textBox = new TextBox(connection, owner);
		}
		
		public void Reset() {
			StorySceneResetMessage message = new StorySceneResetMessage();
			_connection.SendMessage(message);
		}

		public void AddPlayerCharacter(Character character) {
			StorySceneAddPlayerCharacterMessage message = new StorySceneAddPlayerCharacterMessage();

			_connection.SendMessage(message);
		}

		public void RemovePlayerCharacter(Character character) {
			StorySceneRemovePlayerCharacterMessage message = new StorySceneRemovePlayerCharacterMessage();

			_connection.SendMessage(message);
		}

		public void AddObject(SceneObject obj) {
			StorySceneObjectAddMessage message = new StorySceneObjectAddMessage();
			message.objID = obj.ID;
			message.view = obj.CharacterRef.View;
			_connection.SendMessage(message);
		}

		public void RemoveObject(string objID) {
			StorySceneObjectRemoveMessage message = new StorySceneObjectRemoveMessage();
			message.objID = objID;
			_connection.SendMessage(message);
		}

		public void RemoveObject(SceneObject obj) {
			this.RemoveObject(obj.ID);
		}

		public void TransformObject(SceneObject obj, Layout to) {
			StorySceneObjectTransformMessage message = new StorySceneObjectTransformMessage();
			message.objID = obj.ID;
			message.to = to;
			_connection.SendMessage(message);
		}

		public void SetObjectViewEffect(SceneObject obj, CharacterViewEffect effect) {
			StorySceneObjectViewEffectMessage message = new StorySceneObjectViewEffectMessage();
			message.objID = obj.ID;
			message.effect = effect;
			_connection.SendMessage(message);
		}

		public void SetObjectPortraitStyle(SceneObject obj, PortraitStyle portrait) {
			StorySceneObjectPortraitStyleMessage message = new StorySceneObjectPortraitStyleMessage();
			message.objID = obj.ID;
			message.portrait = portrait;
			_connection.SendMessage(message);
		}

		public void TransformCamera(Layout to) {
			StorySceneCameraTransformMessage message = new StorySceneCameraTransformMessage();
			message.to = to;
			_connection.SendMessage(message);
		}

		public void SetCameraEffect(CameraEffect effect) {
			StorySceneCameraEffectMessage message = new StorySceneCameraEffectMessage();
			message.effect = effect;
			_connection.SendMessage(message);
		}

		public override void WaitUserDecision(bool enabled) {
			_ignoreOperating = enabled;
			_textBox.WaitUserDecision(enabled);
		}
	}

	public sealed class DMStoryScene : StorySceneProxy {
		public DMStoryScene(Connection connection, DM owner) :
			base(connection, owner) {
			_connection.AddMessageReceiver(StorySceneNextActionMessage.MESSAGE_TYPE, this);
		}

		public override void MessageReceived(Message message) {
			if (!IsUsing || _ignoreOperating) return;
			this.OnNextAction();
		}

		private void OnNextAction() {

		}

	}

	public sealed class PlayerStoryScene : StorySceneProxy {
		public PlayerStoryScene(Connection connection, Player owner) :
			base(connection, owner) {
			_connection.AddMessageReceiver(StorySceneObjectActionMessage.MESSAGE_TYPE, this);
		}

		public override void MessageReceived(Message message) {
			if (!IsUsing || _ignoreOperating) return;
			StorySceneObjectActionMessage objectMessage = (StorySceneObjectActionMessage)message;
			switch (objectMessage.action) {
				case StorySceneObjectActionMessage.PlayerAction.INTERACT:
					this.OnInteract(objectMessage.objID);
					break;
				case StorySceneObjectActionMessage.PlayerAction.CREATE_ASPECT:
					this.OnCreateAspect(objectMessage.objID);
					break;
				case StorySceneObjectActionMessage.PlayerAction.ATTACK:
					this.OnAttack(objectMessage.objID);
					break;
				case StorySceneObjectActionMessage.PlayerAction.HINDER:
					this.OnHinder(objectMessage.objID);
					break;
				default:
					return;
			}
		}

		private void OnInteract(string objID) {

		}

		private void OnCreateAspect(string objID) {

		}

		private void OnAttack(string objID) {

		}

		private void OnHinder(string objID) {

		}


	}

}
