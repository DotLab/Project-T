using GameServer.Campaign;
using GameServer.CharacterComponents;
using GameServer.Core;
using GameServer.Core.DataSystem;
using GameServer.Playground;
using GameServer.Playground.BattleComponent;
using GameUtil;
using GameUtil.Network;
using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Client {
	public sealed class SkillCheckerProxy : ClientComponentProxy, IRequestHandler {
		public enum ClientPosition {
			INITIATIVE,
			PASSIVE,
			OBSERVER
		}

		private ClientPosition _position = ClientPosition.OBSERVER;
		private bool _skipSelectAspect = false;
		private bool _ignoreOperating = false;

		public override bool IsUsing => SkillChecker.Instance.IsChecking;
		public ClientPosition Position { get => _position; set => _position = value; }

		public Message MakeResponse(Message request) {
			try {
				var checker = SkillChecker.Instance;
				switch (request.MessageType) {
					case CheckerGetInitiativeCanUseSkillMessage.MESSAGE_TYPE: {
							var reqMsg = (CheckerGetInitiativeCanUseSkillMessage)request;
							var resp = new CheckerCanInitiativeUseSkillMessage();
							if (CampaignManager.Instance.CurrentContainer == ContainerType.BATTLE) {
								var skillType = SkillType.SkillTypes[reqMsg.skillTypeID];
								var initiative = (ActableGridObject)BattleScene.Instance.FindObject(reqMsg.initiativeID);
								var battleMapProperty = initiative.CharacterRef.GetSkill(skillType).BattleMapProperty;
								resp.result = checker.CanInitiativeUseSkill(initiative.CharacterRef, skillType, reqMsg.action)
									&& initiative.IsActionPointEnough(battleMapProperty);
							} else {
								// ....
							}
							return resp;
						}
					case CheckerGetInitiativeCanUseStuntMessage.MESSAGE_TYPE: {
							var reqMsg = (CheckerGetInitiativeCanUseStuntMessage)request;
							var resp = new CheckerCanInitiativeUseStuntMessage();
							if (CampaignManager.Instance.CurrentContainer == ContainerType.BATTLE) {
								var initiative = (ActableGridObject)BattleScene.Instance.FindObject(reqMsg.initiativeID);
								var stunt = initiative.CharacterRef.FindStuntByID(reqMsg.stuntID);
								var situation = initiative.GetStuntSituationForUsingCondition(stunt, reqMsg.action);
								resp.result = checker.CanInitiativeUseStunt(initiative.CharacterRef, stunt, situation)
										&& initiative.IsActionPointEnough(stunt.BattleMapProperty);
							} else {
								// ....
							}
							return resp;
						}
					case CheckerGetStuntTargetValidityMessage.MESSAGE_TYPE: {
							var reqMsg = (CheckerGetStuntTargetValidityMessage)request;
							var resp = new CheckerStuntTargetValidityMessage();
							if (CampaignManager.Instance.CurrentContainer == ContainerType.BATTLE) {
								var initiative = (ActableGridObject)BattleScene.Instance.FindObject(reqMsg.initiativeID);
								var target = BattleScene.Instance.FindObject(reqMsg.targetID);
								var stunt = initiative.CharacterRef.FindStuntByID(reqMsg.stuntID);
								var situation = initiative.GetStuntSituationForTargetCondition(target, stunt, reqMsg.action);
								resp.result = checker.CanInitiativeUseStuntOnCharacter(target.CharacterRef, stunt, situation);
							} else {
								// ....
							}
							return resp;
						}
					case CheckerGetPassiveUsableActionListMessage.MESSAGE_TYPE: {
							if (!IsUsing || _position != ClientPosition.PASSIVE) break;
							var reqMsg = (CheckerGetPassiveUsableActionListMessage)request;
							var resp = new CheckerPassiveUsableActionListMessage();
							var skillTypeList = new List<SkillType>();
							var stuntList = new List<Stunt>();
							foreach (var skillType in SkillType.SkillTypes) {
								if (checker.CanCurrentPassiveUseSkill(skillType.Value)) {
									skillTypeList.Add(skillType.Value);
								}
							}
							resp.skillTypesID = new string[skillTypeList.Count];
							for (int i = 0; i < skillTypeList.Count; ++i) {
								resp.skillTypesID[i] = skillTypeList[i].ID;
							}
							if (checker.CurrentPassive.Stunts != null) {
								foreach (var stunt in checker.CurrentPassive.Stunts) {
									if (CampaignManager.Instance.CurrentContainer == ContainerType.BATTLE) {
										var initiativeObj = BattleScene.Instance.FindObject(checker.Initiative.ID);
										var situation = BattleScene.Instance.FindObject(checker.CurrentPassive.ID)
											.GetStuntSituationForPassive(initiativeObj, checker.InitiativeSkillType, stunt, checker.CheckingAction);
										if (checker.CanCurrentPassiveUseStunt(stunt, situation)) {
											stuntList.Add(stunt);
										}
									} else {
										// ....
									}
								}
							}
							resp.stuntsID = new string[stuntList.Count];
							for (int i = 0; i < stuntList.Count; ++i) {
								resp.stuntsID[i] = stuntList[i].ID;
							}
							return resp;
						}
					default:
						break;
				}
				return null;
			} catch (Exception e) {
				Logger.WriteLine(e.Message);
				return null;
			}
		}

		public override void MessageReceived(Message message) {
			if (!IsUsing || _ignoreOperating || _position == ClientPosition.OBSERVER) return;
			var checker = SkillChecker.Instance;
			try {
				switch (message.MessageType) {
					case CheckerSetSkipSelectAspectMessage.MESSAGE_TYPE: {
							var msg = (CheckerSetSkipSelectAspectMessage)message;
							_skipSelectAspect = msg.val;
						}
						break;
					case CheckerPassiveSkillSelectedMessage.MESSAGE_TYPE: {
							var msg = (CheckerPassiveSkillSelectedMessage)message;
							if (!checker.IsChecking || checker.CurrentPassive.Controller != _owner) return;
							var selectedSkillType = SkillType.SkillTypes[msg.skillTypeID];
							checker.CurrentPassiveUseSkillFrameworkInvoking(selectedSkillType);
						}
						break;
					case CheckerPassiveStuntSelectedMessage.MESSAGE_TYPE: {
							var msg = (CheckerPassiveStuntSelectedMessage)message;
							if (!checker.IsChecking || checker.CurrentPassive.Controller != _owner) return;
							var stunt = checker.CurrentPassive.FindStuntByID(msg.stuntID);
							if (stunt != null) {
								if (CampaignManager.Instance.CurrentContainer == ContainerType.BATTLE) {
									var initiativeObj = BattleScene.Instance.FindObject(checker.Initiative.ID);
									var situation = BattleScene.Instance.FindObject(checker.CurrentPassive.ID)
										.GetStuntSituationForPassive(initiativeObj, checker.InitiativeSkillType, stunt, checker.CheckingAction);
									checker.CurrentPassiveUseStuntFrameworkInvoking(stunt, situation, success => {
										if (success) BattleScene.Instance.Update();
									});
								} else {
									// ....
								}
							}
						}
						break;
					case CheckerAspectSelectedMessage.MESSAGE_TYPE: {
							var msg = (CheckerAspectSelectedMessage)message;
							if (!checker.IsChecking) return;
							if (_position == ClientPosition.INITIATIVE) {
								var character = CharacterManager.Instance.FindCharacterByID(msg.aspectOwnerID);
								Aspect aspect;
								if (msg.isConsequence) {
									aspect = character.FindConsequenceByID(msg.aspectID);
								} else {
									aspect = character.FindAspectByID(msg.aspectID);
								}
								checker.InitiativeUseAspect(aspect, msg.reroll);
							} else if (_position == ClientPosition.PASSIVE) {
								var character = CharacterManager.Instance.FindCharacterByID(msg.aspectOwnerID);
								Aspect aspect;
								if (msg.isConsequence) {
									aspect = character.FindConsequenceByID(msg.aspectID);
								} else {
									aspect = character.FindAspectByID(msg.aspectID);
								}
								checker.CurrentPassiveUseAspect(aspect, msg.reroll);
							}
						}
						break;
					case CheckerAspectSelectionOverMessage.MESSAGE_TYPE: {
							var msg = (CheckerAspectSelectionOverMessage)message;
							if (!checker.IsChecking) return;
							if (_position == ClientPosition.INITIATIVE) {
								checker.InitiativeAspectSelectionOver();
							} else if (_position == ClientPosition.PASSIVE) {
								checker.CurrentPassiveAspectSelectionOver();
							}
						}
						break;
					default:
						break;
				}
			} catch (Exception e) {
				Logger.WriteLine(e.Message);
			}
		}

		public override void WaitUserDecision(bool enabled) {
			_ignoreOperating = enabled;
		}

		public SkillCheckerProxy(Connection connection, User owner) :
			base(connection, owner) {
			connection.SetRequestHandler(CheckerGetInitiativeCanUseSkillMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(CheckerGetInitiativeCanUseStuntMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(CheckerGetStuntTargetValidityMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(CheckerGetPassiveUsableActionListMessage.MESSAGE_TYPE, this);

			connection.AddMessageReceiver(CheckerSetSkipSelectAspectMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(CheckerPassiveSkillSelectedMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(CheckerPassiveStuntSelectedMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(CheckerAspectSelectedMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(CheckerAspectSelectionOverMessage.MESSAGE_TYPE, this);
		}

		public void StartCheck(Character initiative, SkillType initiativeSkillType, CharacterAction action, IEnumerable<Character> targets) {
			if (!IsUsing) return;
			var message = new CheckerStartCheckMessage();
			message.initiativeID = initiative.ID;
			message.initiativeSkillTypeID = initiativeSkillType.ID;
			message.action = action;
			int count = 0;
			foreach (var target in targets) ++count;
			message.targetsID = new string[count];
			int i = 0;
			foreach (var target in targets) {
				message.targetsID[i++] = target.ID;
			}
			_connection.SendMessage(message);
		}

		public void CheckNextone(Character nextOne) {
			if (!IsUsing) return;
			var message = new CheckerCheckNextOneMessage();
			message.nextOneID = nextOne.ID;
			_connection.SendMessage(message);
		}

		public void EndCheck() {
			if (!IsUsing) return;
			var message = new CheckerEndCheckMessage();
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeActionAccepted() {
			if (!IsUsing) return;
			var message = new CheckerCharacterActionResponseMessage();
			message.isInitiative = true;
			message.failure = false;
			message.failureDescription = "";
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeActionFailure(string msg) {
			if (!IsUsing) return;
			var message = new CheckerCharacterActionResponseMessage();
			message.isInitiative = true;
			message.failure = true;
			message.failureDescription = msg;
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectAction() {
			if (!IsUsing) return;
			var message = new CheckerNotifyPassiveSelectActionMessage();
			_connection.SendMessage(message);
		}

		public void NotifyPassiveActionAccepted() {
			if (!IsUsing) return;
			var message = new CheckerCharacterActionResponseMessage();
			message.isInitiative = false;
			message.failure = false;
			message.failureDescription = "";
			_connection.SendMessage(message);
		}

		public void NotifyPassiveActionFailure(string msg) {
			if (!IsUsing) return;
			var message = new CheckerCharacterActionResponseMessage();
			message.isInitiative = false;
			message.failure = true;
			message.failureDescription = msg;
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeSelectAspect() {
			if (!IsUsing) return;
			var message = new CheckerNotifySelectAspectMessage();
			message.isInitiative = true;
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeSelectAspectComplete() {
			if (!IsUsing) return;
			var message = new CheckerSelectAspectResponseMessage();
			message.selectionOver = false;
			message.isInitiative = true;
			message.failure = false;
			message.failureDescription = "";
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeSelectAspectFailure(string msg) {
			if (!IsUsing) return;
			var message = new CheckerSelectAspectResponseMessage();
			message.selectionOver = false;
			message.isInitiative = true;
			message.failure = true;
			message.failureDescription = msg;
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeSelectAspectOver() {
			if (!IsUsing) return;
			var message = new CheckerSelectAspectResponseMessage();
			message.selectionOver = true;
			message.isInitiative = true;
			message.failure = false;
			message.failureDescription = "";
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectAspect() {
			if (!IsUsing) return;
			var message = new CheckerNotifySelectAspectMessage();
			message.isInitiative = false;
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectAspectComplete() {
			if (!IsUsing) return;
			var message = new CheckerSelectAspectResponseMessage();
			message.selectionOver = false;
			message.isInitiative = false;
			message.failure = false;
			message.failureDescription = "";
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectAspectFailure(string msg) {
			if (!IsUsing) return;
			var message = new CheckerSelectAspectResponseMessage();
			message.selectionOver = false;
			message.isInitiative = false;
			message.failure = true;
			message.failureDescription = msg;
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectAspectOver() {
			if (!IsUsing) return;
			var message = new CheckerSelectAspectResponseMessage();
			message.selectionOver = true;
			message.isInitiative = false;
			message.failure = false;
			message.failureDescription = "";
			_connection.SendMessage(message);
		}

		public void DisplayCheckResult(CheckResult initiative, CheckResult passive, int delta) {
			var message = new CheckerCheckResultMessage();
			message.initiative = initiative;
			message.passive = passive;
			message.delta = delta;
			_connection.SendMessage(message);
		}

		public void UpdateSumPoint(bool isInitiative, int point) {
			if (!IsUsing) return;
			var message = new CheckerUpdateSumPointMessage();
			message.isInitiative = isInitiative;
			message.point = point;
			_connection.SendMessage(message);
		}

		public void DisplayUsingSkill(bool isInitiative, SkillType skillType) {
			if (!IsUsing) return;
			var message = new CheckerDisplayUsingSkillMessage();
			message.isInitiative = isInitiative;
			message.skillTypeID = skillType.ID;
			_connection.SendMessage(message);
		}

		public void DisplayUsingStunt(Character character, Stunt stunt) {
			if (!IsUsing) return;
			var message = new CheckerDisplayUsingStuntMessage();
			message.characterID = character.ID;
			message.stuntID = stunt.ID;
			_connection.SendMessage(message);
		}

		public void DisplayUsingAspect(bool isInitiative, Character aspectOwner, Aspect aspect) {
			if (!IsUsing) return;
			var message = new CheckerDisplayUsingAspectMessage();
			message.isInitiative = isInitiative;
			message.aspectOwnerID = aspectOwner.ID;
			message.aspectID = aspect.ID;
			_connection.SendMessage(message);
		}
	}
}
