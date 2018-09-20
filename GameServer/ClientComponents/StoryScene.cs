using GameServer.CharacterSystem;
using GameServer.Container;
using GameServer.Container.StoryComponent;
using GameServer.Core;
using GameServer.Core.DataSystem;
using GameUtil;
using GameUtil.Network;
using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
using System;

namespace GameServer.ClientComponents {
	public sealed class SkillCheckPanel : ClientComponent {
		public enum ClientPosition {
			INITIATIVE,
			PASSIVE,
			OBSERVER
		}

		private ClientPosition _position = ClientPosition.OBSERVER;
		private bool _isUsing = false;
		private bool _ignoreOperating = false;

		public SkillCheckPanel(Connection connection, User owner) :
			base(connection, owner) {
			_connection.AddMessageReceiver(CheckerSkillSelectedMessage.MESSAGE_TYPE, this);
			_connection.AddMessageReceiver(CheckerAspectSelectedMessage.MESSAGE_TYPE, this);
			_connection.AddMessageReceiver(CheckerStuntSelectedMessage.MESSAGE_TYPE, this);
		}

		public override void MessageReceived(Message message) {
			try {
				if (!_isUsing || _ignoreOperating || _position == ClientPosition.OBSERVER) return;
				if (message.MessageType == CheckerSkillSelectedMessage.MESSAGE_TYPE) {
					CheckerSkillSelectedMessage skillSelectedMessage = (CheckerSkillSelectedMessage)message;
					if (SkillType.SkillTypes.TryGetValue(skillSelectedMessage.skillTypeID, out SkillType skillType)) {
						this.OnSelectSkill(skillType);
					}
				} else if (message.MessageType == CheckerStuntSelectedMessage.MESSAGE_TYPE) {
					CheckerStuntSelectedMessage stuntSelectedMessage = (CheckerStuntSelectedMessage)message;
					this.OnSelectStunt(stuntSelectedMessage.stuntID);
				} else if (message.MessageType == CheckerAspectSelectedMessage.MESSAGE_TYPE) {
					Aspect result = null;
					CheckerAspectSelectedMessage aspectSelectedMessage = (CheckerAspectSelectedMessage)message;
					Character character = CharacterManager.Instance.FindCharacterOrItemRecursivelyByID(aspectSelectedMessage.characterID);
					if (character != null) {
						result = character.FindAspectByID(aspectSelectedMessage.aspectID);
					}
					this.OnSelectAspects(result, aspectSelectedMessage.reroll);
				}
			} catch (Exception e) {
				Logger.WriteLine(e.Message);
			}
		}

		private void OnSelectSkill(SkillType skillType) {
			if (_position == ClientPosition.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_SKILL) {
				StorySceneContainer.Instance.InitiativeUseSkill(skillType, false, false);
			} else if (_position == ClientPosition.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL) {
				StorySceneContainer.Instance.PassiveUseSkill(skillType, false, false);
			}
		}

		private void OnSelectStunt(string stuntID) {
			if (_position == ClientPosition.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_SKILL) {
				var stunt = SkillChecker.Instance.Initiative.FindStuntByID(stuntID);
				StorySceneContainer.Instance.InitiativeUseStunt(stunt);
			} else if (_position == ClientPosition.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL) {
				var stunt = SkillChecker.Instance.Passive.FindStuntByID(stuntID);
				StorySceneContainer.Instance.PassiveUseStunt(stunt);
			}
		}

		private void OnSelectAspects(Aspect aspect, bool reroll) {
			if (_position == ClientPosition.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_ASPECT) {
				StorySceneContainer.Instance.InitiativeUseAspect(aspect, reroll);
			} else if (_position == ClientPosition.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_ASPECT) {
				StorySceneContainer.Instance.PassiveUseAspect(aspect, reroll);
			}
		}

		public void Open(Character initiative, Character passive, ClientPosition clientState) {
			if (_isUsing) return;
			StorySceneCheckerPanelShowMessage message = new StorySceneCheckerPanelShowMessage();
			message.initiativeCharacterID = initiative.ID;
			message.initiativeView = initiative.View;
			message.passiveCharacterID = passive.ID;
			message.passiveView = passive.View;
			message.playerState = (int)(_position = clientState);
			_connection.SendMessage(message);
			_isUsing = true;
		}

		public void Close() {
			if (!_isUsing) return;
			StorySceneCheckerPanelHideMessage message = new StorySceneCheckerPanelHideMessage();
			_connection.SendMessage(message);
			_position = ClientPosition.OBSERVER;
			_isUsing = false;
		}

		public void NotifyInitiativeSelectSkillOrStunt(CharacterAction action, Character initiative, Character passive) {
			if (!_isUsing) return;
			var message = new StorySceneCheckerNotifyInitiativeSelectSkillOrStuntMessage();
			message.initiativeCharacterID = initiative.ID;
			message.passiveCharacterID = passive.ID;
			message.action = action;
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectSkillOrStunt(CharacterAction action, Character passive, Character initiative, SkillType initiativeSkillType) {
			if (!_isUsing) return;
			var message = new StorySceneCheckerNotifyPassiveSelectSkillOrStuntMessage();
			message.passiveCharacterID = passive.ID;
			message.initiativeCharacterID = initiative.ID;
			message.initiativeSkillType = StreamableFactory.CreateSkillTypeDescription(initiativeSkillType);
			message.action = action;
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeCheckSelectAspect(Character initiative) {
			if (!_isUsing) return;
			var message = new StorySceneCheckerNotifySelectAspectMessage();
			message.isInitiative = true;
			message.characterID = initiative.ID;
			_connection.SendMessage(message);
		}

		public void NotifyPassiveCheckSelectAspect(Character passive) {
			if (!_isUsing) return;
			var message = new StorySceneCheckerNotifySelectAspectMessage();
			message.isInitiative = false;
			message.characterID = passive.ID;
			_connection.SendMessage(message);
		}

		public void DisplayDicePoint(bool isInitiative, int[] dicePoints) {
			if (!_isUsing) return;
			DisplayDicePointsMessage message = new DisplayDicePointsMessage();
			message.userID = isInitiative ? SkillChecker.Instance.Initiative.Controller.ID : SkillChecker.Instance.Passive.Controller.ID;
			message.dicePoints = dicePoints;
			_connection.SendMessage(message);
		}

		public void UpdateSumPoint(bool isInitiative, int point) {
			if (!_isUsing) return;
			StorySceneCheckerUpdateSumPointMessage message = new StorySceneCheckerUpdateSumPointMessage();
			message.isInitiative = isInitiative;
			message.point = point;
			_connection.SendMessage(message);
		}

		public void DisplaySkillReady(bool isInitiative, SkillType skillType, bool bigone) {
			if (!_isUsing) return;
			StorySceneCheckerDisplaySkillReadyMessage message = new StorySceneCheckerDisplaySkillReadyMessage();
			message.isInitiative = isInitiative;
			message.skillTypeID = skillType.ID;
			message.bigone = bigone;
			_connection.SendMessage(message);
		}

		public void DisplayUsingAspect(bool isInitiative, Aspect aspect) {
			if (!_isUsing) return;
			StorySceneCheckerDisplayUsingAspectMessage message = new StorySceneCheckerDisplayUsingAspectMessage();
			message.isInitiative = isInitiative;
			message.characterID = aspect.Belong.ID;
			message.aspectID = aspect.ID;
			_connection.SendMessage(message);
		}

		public override void WaitingForUserDetermin(bool enabled) {
			_ignoreOperating = enabled;
		}
	}

	public sealed class TextBox : ClientComponent {
		private bool _ignoreOperating = false;

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

		public override void WaitingForUserDetermin(bool enabled) {
			_ignoreOperating = enabled;
		}
	}

	public class StoryScene : ClientComponent {
		protected readonly TextBox _textBox;
		protected readonly SkillCheckPanel _skillCheckPanel;
		protected bool _isUsing = false;
		protected bool _ignoreOperating = false;

		public TextBox TextBox => _textBox;
		public SkillCheckPanel SkillCheckPanel => _skillCheckPanel;

		public override void MessageReceived(Message message) { }

		protected StoryScene(Connection connection, User owner) :
			base(connection, owner) {
			_textBox = new TextBox(connection, owner);
			_skillCheckPanel = new SkillCheckPanel(connection, owner);
		}

		public void Open() {
			if (_isUsing) return;
			_isUsing = true;
		}

		public void Close() {
			if (!_isUsing) return;
			_skillCheckPanel.Close();
			_isUsing = false;
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

		public void AddObject(ISceneObject obj) {
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

		public void RemoveObject(ISceneObject obj) {
			this.RemoveObject(obj.ID);
		}

		public void TransformObject(ISceneObject obj, Layout to) {
			StorySceneObjectTransformMessage message = new StorySceneObjectTransformMessage();
			message.objID = obj.ID;
			message.to = to;
			_connection.SendMessage(message);
		}

		public void SetObjectViewEffect(ISceneObject obj, CharacterViewEffect effect) {
			StorySceneObjectViewEffectMessage message = new StorySceneObjectViewEffectMessage();
			message.objID = obj.ID;
			message.effect = effect;
			_connection.SendMessage(message);
		}

		public void SetObjectPortraitStyle(ISceneObject obj, PortraitStyle portrait) {
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

		public override void WaitingForUserDetermin(bool enabled) {
			_ignoreOperating = enabled;
			_textBox.WaitingForUserDetermin(enabled);
			_skillCheckPanel.WaitingForUserDetermin(enabled);
		}
	}

	public sealed class DMStoryScene : StoryScene {
		public DMStoryScene(Connection connection, DM owner) :
			base(connection, owner) {
			_connection.AddMessageReceiver(StorySceneNextActionMessage.MESSAGE_TYPE, this);
		}

		public override void MessageReceived(Message message) {
			if (!_isUsing || _ignoreOperating) return;
			this.OnNextAction();
		}

		private void OnNextAction() {

		}

	}

	public sealed class PlayerStoryScene : StoryScene {
		public PlayerStoryScene(Connection connection, Player owner) :
			base(connection, owner) {
			_connection.AddMessageReceiver(StorySceneObjectActionMessage.MESSAGE_TYPE, this);
		}

		public override void MessageReceived(Message message) {
			if (!_isUsing || _ignoreOperating) return;
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
