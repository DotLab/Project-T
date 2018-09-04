using GameLib.Utilities.Network.ClientMessages;
using GameLib.Utilities.Network.ServerMessages;
using GameLib.Utilities.Network.Streamable;
using System;
using System.Collections.Generic;

namespace GameLib.Utilities.Network {
	public abstract class Message : IStreamable {
		#region Message Creator
		public static Message New(int messageType) {
			switch (messageType) {
				case IdentifiedMessage.MESSAGE_TYPE:
					return new IdentifiedMessage();

				// server message
				case ServerReadyMessage.MESSAGE_TYPE:
					return new ServerReadyMessage();
				case StorySceneResetMessage.MESSAGE_TYPE:
					return new StorySceneResetMessage();
				case StorySceneObjectAddMessage.MESSAGE_TYPE:
					return new StorySceneObjectAddMessage();
				case StorySceneObjectRemoveMessage.MESSAGE_TYPE:
					return new StorySceneObjectRemoveMessage();
				case StorySceneObjectTransformMessage.MESSAGE_TYPE:
					return new StorySceneObjectTransformMessage();
				case StorySceneObjectViewEffectMessage.MESSAGE_TYPE:
					return new StorySceneObjectViewEffectMessage();
				case StorySceneObjectPortraitStyleMessage.MESSAGE_TYPE:
					return new StorySceneObjectPortraitStyleMessage();
				case StorySceneCameraTransformMessage.MESSAGE_TYPE:
					return new StorySceneCameraTransformMessage();
				case StorySceneCameraEffectMessage.MESSAGE_TYPE:
					return new StorySceneCameraEffectMessage();
				case PlayBGMMessage.MESSAGE_TYPE:
					return new PlayBGMMessage();
				case StopBGMMessage.MESSAGE_TYPE:
					return new StopBGMMessage();
				case PlaySEMessage.MESSAGE_TYPE:
					return new PlaySEMessage();
				case ShowSceneMessage.MESSAGE_TYPE:
					return new ShowSceneMessage();
				case TextBoxAddParagraphMessage.MESSAGE_TYPE:
					return new TextBoxAddParagraphMessage();
				case TextBoxAddSelectionMessage.MESSAGE_TYPE:
					return new TextBoxAddSelectionMessage();
				case TextBoxClearMessage.MESSAGE_TYPE:
					return new TextBoxClearMessage();
				case TextBoxSetPortraitMessage.MESSAGE_TYPE:
					return new TextBoxSetPortraitMessage();
				case TextBoxPortraitStyleMessage.MESSAGE_TYPE:
					return new TextBoxPortraitStyleMessage();
				case TextBoxPortraitEffectMessage.MESSAGE_TYPE:
					return new TextBoxPortraitEffectMessage();
				case CharacterInfoDataMessage.MESSAGE_TYPE:
					return new CharacterInfoDataMessage();
				case CharacterAspectsDescriptionMessage.MESSAGE_TYPE:
					return new CharacterAspectsDescriptionMessage();
				case CharacterStuntsDescriptionMessage.MESSAGE_TYPE:
					return new CharacterStuntsDescriptionMessage();
				case CharacterExtrasDescriptionMessage.MESSAGE_TYPE:
					return new CharacterExtrasDescriptionMessage();
				case CharacterConsequencesDescriptionMessage.MESSAGE_TYPE:
					return new CharacterConsequencesDescriptionMessage();
				case CharacterStressDataMessage.MESSAGE_TYPE:
					return new CharacterStressDataMessage();
				case CharacterFatePointDataMessage.MESSAGE_TYPE:
					return new CharacterFatePointDataMessage();
				case AspectDataMessage.MESSAGE_TYPE:
					return new AspectDataMessage();
				case ConsequenceDataMessage.MESSAGE_TYPE:
					return new ConsequenceDataMessage();
				case SkillDataMessage.MESSAGE_TYPE:
					return new SkillDataMessage();
				case StuntDataMessage.MESSAGE_TYPE:
					return new StuntDataMessage();
				case ExtraDataMessage.MESSAGE_TYPE:
					return new ExtraDataMessage();
				case DirectResistSkillsDataMessage.MESSAGE_TYPE:
					return new DirectResistSkillsDataMessage();
				case SkillTypeListDataMessage.MESSAGE_TYPE:
					return new SkillTypeListDataMessage();
				case StorySceneCheckerPanelShowMessage.MESSAGE_TYPE:
					return new StorySceneCheckerPanelShowMessage();
				case StorySceneCheckerPanelHideMessage.MESSAGE_TYPE:
					return new StorySceneCheckerPanelHideMessage();
				case DMCheckMessage.MESSAGE_TYPE:
					return new DMCheckMessage();
				case DisplayDicePointsMessage.MESSAGE_TYPE:
					return new DisplayDicePointsMessage();
				case StorySceneCheckerNotifyInitiativeSelectSkillOrStuntMessage.MESSAGE_TYPE:
					return new StorySceneCheckerNotifyInitiativeSelectSkillOrStuntMessage();
				case StorySceneCheckerNotifyPassiveSelectSkillOrStuntMessage.MESSAGE_TYPE:
					return new StorySceneCheckerNotifyPassiveSelectSkillOrStuntMessage();
				case CheckerSelectSkillOrStuntCompleteMessage.MESSAGE_TYPE:
					return new CheckerSelectSkillOrStuntCompleteMessage();
				case StorySceneCheckerNotifySelectAspectMessage.MESSAGE_TYPE:
					return new StorySceneCheckerNotifySelectAspectMessage();
				case CheckerSelectAspectCompleteMessage.MESSAGE_TYPE:
					return new CheckerSelectAspectCompleteMessage();
				case StorySceneCheckerUpdateSumPointMessage.MESSAGE_TYPE:
					return new StorySceneCheckerUpdateSumPointMessage();
				case StorySceneCheckerDisplaySkillReadyMessage.MESSAGE_TYPE:
					return new StorySceneCheckerDisplaySkillReadyMessage();
				case StorySceneCheckerDisplayUsingAspectMessage.MESSAGE_TYPE:
					return new StorySceneCheckerDisplayUsingAspectMessage();
				case StorySceneAddPlayerCharacterMessage.MESSAGE_TYPE:
					return new StorySceneAddPlayerCharacterMessage();
				case StorySceneRemovePlayerCharacterMessage.MESSAGE_TYPE:
					return new StorySceneRemovePlayerCharacterMessage();
				case BattleScenePushGridObjectMessage.MESSAGE_TYPE:
					return new BattleScenePushGridObjectMessage();
				case BattleSceneRemoveGridObjectMessage.MESSAGE_TYPE:
					return new BattleSceneRemoveGridObjectMessage();
				case BattleSceneAddLadderObjectMessage.MESSAGE_TYPE:
					return new BattleSceneAddLadderObjectMessage();
				case BattleSceneRemoveLadderObjectMessage.MESSAGE_TYPE:
					return new BattleSceneRemoveLadderObjectMessage();
				case BattleSceneResetMessage.MESSAGE_TYPE:
					return new BattleSceneResetMessage();
				case BattleSceneSetActingOrderMessage.MESSAGE_TYPE:
					return new BattleSceneSetActingOrderMessage();
				case BattleSceneChangeTurnMessage.MESSAGE_TYPE:
					return new BattleSceneChangeTurnMessage();
				case BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage.MESSAGE_TYPE:
					return new BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage();
				case BattleSceneCheckerNotifySelectAspectMessage.MESSAGE_TYPE:
					return new BattleSceneCheckerNotifySelectAspectMessage();
				case BattleSceneCheckerUpdateSumPointMessage.MESSAGE_TYPE:
					return new BattleSceneCheckerUpdateSumPointMessage();
				case BattleSceneCheckerDisplaySkillReadyMessage.MESSAGE_TYPE:
					return new BattleSceneCheckerDisplaySkillReadyMessage();
				case BattleSceneCheckerDisplayUsingAspectMessage.MESSAGE_TYPE:
					return new BattleSceneCheckerDisplayUsingAspectMessage();
				case BattleSceneMovePathInfoMessage.MESSAGE_TYPE:
					return new BattleSceneMovePathInfoMessage();
				case BattleSceneDisplayActableObjectMovingMessage.MESSAGE_TYPE:
					return new BattleSceneDisplayActableObjectMovingMessage();
				case BattleSceneGridObjectDataMessage.MESSAGE_TYPE:
					return new BattleSceneGridObjectDataMessage();
				case BattleSceneLadderObjectDataMessage.MESSAGE_TYPE:
					return new BattleSceneLadderObjectDataMessage();
				case BattleSceneDisplayTakeExtraMovePointMessage.MESSAGE_TYPE:
					return new BattleSceneDisplayTakeExtraMovePointMessage();
				case BattleSceneUpdateActionPointMessage.MESSAGE_TYPE:
					return new BattleSceneUpdateActionPointMessage();
				case BattleSceneObjectUsableSkillListMessage.MESSAGE_TYPE:
					return new BattleSceneObjectUsableSkillListMessage();
				case BattleSceneObjectUsableStuntListMessage.MESSAGE_TYPE:
					return new BattleSceneObjectUsableStuntListMessage();
				case BattleSceneCanTakeExtraMoveMessage.MESSAGE_TYPE:
					return new BattleSceneCanTakeExtraMoveMessage();
				case BattleSceneUpdateGridInfoMessage.MESSAGE_TYPE:
					return new BattleSceneUpdateGridInfoMessage();
				case BattleSceneUpdateMovePointMessage.MESSAGE_TYPE:
					return new BattleSceneUpdateMovePointMessage();
				case BattleSceneStartCheckMessage.MESSAGE_TYPE:
					return new BattleSceneStartCheckMessage();
				case BattleSceneCheckNextoneMessage.MESSAGE_TYPE:
					return new BattleSceneCheckNextoneMessage();
				case BattleSceneEndCheckMessage.MESSAGE_TYPE:
					return new BattleSceneEndCheckMessage();
				case UserDeterminMessage.MESSAGE_TYPE:
					return new UserDeterminMessage();
				case CheckerCheckResultMessage.MESSAGE_TYPE:
					return new CheckerCheckResultMessage();
				case BattleSceneStuntTargetSelectableMessage.MESSAGE_TYPE:
					return new BattleSceneStuntTargetSelectableMessage();

				// client message
				case ClientInitMessage.MESSAGE_TYPE:
					return new ClientInitMessage();
				case StorySceneObjectActionMessage.MESSAGE_TYPE:
					return new StorySceneObjectActionMessage();
				case TextSelectedMessage.MESSAGE_TYPE:
					return new TextSelectedMessage();
				case StorySceneNextActionMessage.MESSAGE_TYPE:
					return new StorySceneNextActionMessage();
				case CheckerSkillSelectedMessage.MESSAGE_TYPE:
					return new CheckerSkillSelectedMessage();
				case CheckerAspectSelectedMessage.MESSAGE_TYPE:
					return new CheckerAspectSelectedMessage();
				case CheckerStuntSelectedMessage.MESSAGE_TYPE:
					return new CheckerStuntSelectedMessage();
				case GetCharacterDataMessage.MESSAGE_TYPE:
					return new GetCharacterDataMessage();
				case GetAspectDataMessage.MESSAGE_TYPE:
					return new GetAspectDataMessage();
				case GetConsequenceDataMessage.MESSAGE_TYPE:
					return new GetConsequenceDataMessage();
				case GetSkillDataMessage.MESSAGE_TYPE:
					return new GetSkillDataMessage();
				case GetStuntDataMessage.MESSAGE_TYPE:
					return new GetStuntDataMessage();
				case GetExtraDataMessage.MESSAGE_TYPE:
					return new GetExtraDataMessage();
				case GetDirectResistSkillsMessage.MESSAGE_TYPE:
					return new GetDirectResistSkillsMessage();
				case GetSkillTypeListMessage.MESSAGE_TYPE:
					return new GetSkillTypeListMessage();
				case DMCheckResultMessage.MESSAGE_TYPE:
					return new DMCheckResultMessage();
				case BattleSceneSetSkipSelectAspectMessage.MESSAGE_TYPE:
					return new BattleSceneSetSkipSelectAspectMessage();
				case SelectAspectOverMessage.MESSAGE_TYPE:
					return new SelectAspectOverMessage();
				case BattleSceneGetActableObjectMovePathInfoMessage.MESSAGE_TYPE:
					return new BattleSceneGetActableObjectMovePathInfoMessage();
				case BattleSceneActableObjectMoveMessage.MESSAGE_TYPE:
					return new BattleSceneActableObjectMoveMessage();
				case BattleSceneActableObjectDoActionMessage.MESSAGE_TYPE:
					return new BattleSceneActableObjectDoActionMessage();
				case BattleSceneActableObjectDoSpecialActionMessage.MESSAGE_TYPE:
					return new BattleSceneActableObjectDoSpecialActionMessage();
				case BattleSceneTakeExtraMovePointMessage.MESSAGE_TYPE:
					return new BattleSceneTakeExtraMovePointMessage();
				case BattleSceneGetGridObjectDataMessage.MESSAGE_TYPE:
					return new BattleSceneGetGridObjectDataMessage();
				case BattleSceneGetLadderObjectDataMessage.MESSAGE_TYPE:
					return new BattleSceneGetLadderObjectDataMessage();
				case BattleSceneGetInitiativeUsableSkillOrStuntListMessage.MESSAGE_TYPE:
					return new BattleSceneGetInitiativeUsableSkillOrStuntListMessage();
				case BattleSceneGetPassiveUsableSkillOrStuntListMessage.MESSAGE_TYPE:
					return new BattleSceneGetPassiveUsableSkillOrStuntListMessage();
				case BattleSceneGetCanExtraMoveMessage.MESSAGE_TYPE:
					return new BattleSceneGetCanExtraMoveMessage();
				case BattleSceneTurnOverMessage.MESSAGE_TYPE:
					return new BattleSceneTurnOverMessage();
				case UserDeterminResultMessage.MESSAGE_TYPE:
					return new UserDeterminResultMessage();
				case BattleSceneGetStuntTargetSelectableMessage.MESSAGE_TYPE:
					return new BattleSceneGetStuntTargetSelectableMessage();

				default:
					throw new NotImplementedException();
			}
		}
		#endregion

		public abstract void WriteTo(IDataOutputStream stream);
		public abstract void ReadFrom(IDataInputStream stream);

		public abstract int MessageType { get; }
	}

	public sealed class IdentifiedMessage : Message {
		public Message innerMessage;
		public Guid guid;
		public bool resp;

		public const int MESSAGE_TYPE = 0;
		public override int MessageType => MESSAGE_TYPE;
		public int InnerMsgType => innerMessage == null ? 0 : innerMessage.MessageType;

		public IdentifiedMessage() { }

		public IdentifiedMessage(Message message) {
			this.innerMessage = message;
			this.guid = Guid.NewGuid();
			this.resp = false;
		}

		public override void ReadFrom(IDataInputStream stream) {
			int innerType = stream.ReadInt32();
			if (innerType == 0) {
				innerMessage = null;
			} else {
				innerMessage = New(innerType);
				innerMessage.ReadFrom(stream);
			}
			guid = InputStreamHelper.ReadGuid(stream);
			resp = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(this.InnerMsgType);
			if (innerMessage != null) innerMessage.WriteTo(stream);
			OutputStreamHelper.WriteGuid(stream, guid);
			stream.WriteBoolean(resp);
		}
	}

	public interface IMessageReceiver {
		void MessageReceived(Message message);
	}

	public interface IRequestHandler {
		Message MakeResponse(Message request);
	}

	public abstract class Connection : IMessageReceiver {
		private readonly Dictionary<Guid, Action<Message>> _callbackDict = new Dictionary<Guid, Action<Message>>();
		private readonly Dictionary<int, IRequestHandler> _reqHandlerDict = new Dictionary<int, IRequestHandler>();

		public void MessageReceived(Message message) {
			var identifiedMsg = (IdentifiedMessage)message;
			if (identifiedMsg.resp) {
				if (_callbackDict.TryGetValue(identifiedMsg.guid, out Action<Message> callback)) {
					callback(identifiedMsg.innerMessage);
					_callbackDict.Remove(identifiedMsg.guid);
				}
			} else {
				Message resp = null;
				if (_reqHandlerDict.TryGetValue(identifiedMsg.InnerMsgType, out IRequestHandler handler)) {
					resp = handler.MakeResponse(identifiedMsg.innerMessage);
				}
				var respWrapper = new IdentifiedMessage() { innerMessage = resp, guid = identifiedMsg.guid, resp = true };
				SendMessage(respWrapper);
			}
		}

		public void Request(Message message, Action<Message> callback) {
			var identifiedMsg = new IdentifiedMessage(message);
			while (_callbackDict.ContainsKey(identifiedMsg.guid)) identifiedMsg.guid = Guid.NewGuid();
			_callbackDict.Add(identifiedMsg.guid, callback);
			SendMessage(identifiedMsg);
		}

		public void SetRequestHandler(int messageType, IRequestHandler handler) {
			_reqHandlerDict[messageType] = handler;
		}

		public Connection() {
			AddMessageReceiver(IdentifiedMessage.MESSAGE_TYPE, this);
		}

		protected void OnEventCaught(NetworkEventCaughtEventArgs args) {
			if (EventCaught != null) EventCaught(this, args);
		}

		public event EventHandler<NetworkEventCaughtEventArgs> EventCaught; // multiple threads would invoke

		public abstract void SendMessage(Message message);
		public abstract void AddMessageReceiver(int messageType, IMessageReceiver receiver);
		public abstract bool RemoveMessageReceiver(int messageType, IMessageReceiver receiver);
		public abstract void UpdateReceiver();
	}

	public sealed class NetworkEventCaughtEventArgs : EventArgs {
		public string message;
		// ...
	}
}