using GameLib.CharacterSystem;
using GameLib.Container;
using GameLib.Container.BattleComponent;
using GameLib.Core;
using GameLib.Core.DataSystem;
using GameLib.Utilities;
using GameLib.Utilities.Network;
using GameLib.Utilities.Network.ClientMessages;
using GameLib.Utilities.Network.ServerMessages;
using System;
using System.Collections.Generic;

namespace GameLib.ClientComponents {
	public class BattleScene : ClientComponent, IRequestHandler {
		protected bool _canOperate = false;
		protected bool _skipSelectAspect = false;
		protected bool _isUsing = false;

		protected BattleSceneMovePathInfoMessage CreateMovePathInfoMessage(List<ReachablePlace> reachablePlaces) {
			var ret = new BattleSceneMovePathInfoMessage();
			ret.pathInfo = new BattleSceneMovePathInfoMessage.ReachableGrid[reachablePlaces.Count];
			for (int i = 0; i < reachablePlaces.Count; ++i) {
				var prevPlace = reachablePlaces[i].prevPlace;
				ret.pathInfo[i].prevPlaceIndex = prevPlace != null ? reachablePlaces.IndexOf(prevPlace) : -1;
				ret.pathInfo[i].row = reachablePlaces[i].row;
				ret.pathInfo[i].col = reachablePlaces[i].col;
				ret.pathInfo[i].highland = reachablePlaces[i].highland;
				ret.pathInfo[i].leftMovePoint = reachablePlaces[i].leftMovePoint;
			}
			return ret;
		}

		public Message MakeResponse(Message request) {
			try {
				if (!_isUsing) return null;
				var container = BattleSceneContainer.Instance;
				switch (request.MessageType) {
					case BattleSceneGetActableObjectMovePathInfoMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) break;
							var list = container.CurrentActable.GetReachablePlaceList();
							var resp = CreateMovePathInfoMessage(list);
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
			if (!_isUsing) return;
			var container = BattleSceneContainer.Instance;
			try {
				switch (message.MessageType) {
					case BattleSceneActableObjectMoveMessage.MESSAGE_TYPE: {
							var moveMsg = (BattleSceneActableObjectMoveMessage)message;
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;
							container.CurrentActable.MoveTo(moveMsg.dstRow, moveMsg.dstCol, moveMsg.dstHighland);
						}
						break;
					case BattleSceneSetSkipSelectAspectMessage.MESSAGE_TYPE: {
							var skipAspectMsg = (BattleSceneSetSkipSelectAspectMessage)message;
							_skipSelectAspect = skipAspectMsg.val;
						}
						break;
					case CheckerSkillSelectedMessage.MESSAGE_TYPE: {
							var skillSelectedMsg = (CheckerSkillSelectedMessage)message;
							if (!container.IsChecking) return;
							if (SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL_OR_STUNT
								&& container.CurrentPassive.CharacterRef.Controller == _owner) {
								var selectedSkillType = SkillType.SkillTypes[skillSelectedMsg.skillTypeID];
								container.CurrentPassiveUseSkill(selectedSkillType, false, false);
							}
						}
						break;
					case CheckerStuntSelectedMessage.MESSAGE_TYPE: {
							var stuntSelectedMsg = (CheckerStuntSelectedMessage)message;
							if (!container.IsChecking) return;
							if (SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL_OR_STUNT
								&& container.CurrentPassive.CharacterRef.Controller == _owner) {
								foreach (var stunt in container.CurrentPassive.CharacterRef.Stunts) {
									if (stunt.ID == stuntSelectedMsg.stuntID) {
										container.CurrentPassiveUseStunt(stunt);
										break;
									}
								}
							}
						}
						break;
					case CheckerAspectSelectedMessage.MESSAGE_TYPE: {
							var aspectSelectedMsg = (CheckerAspectSelectedMessage)message;
							if (!container.IsChecking) return;
							if (SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_ASPECT
								&& container.Initiative.CharacterRef.Controller == _owner) {
								var gridObj = container.FindGridObject(aspectSelectedMsg.characterID);
								foreach (var aspect in gridObj.CharacterRef.Aspects) {
									if (aspect.ID == aspectSelectedMsg.aspectID) {
										container.InitiativeSelectAspect(aspect, aspectSelectedMsg.reroll);
										break;
									}
								}
							} else if (SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_ASPECT
								  && container.CurrentPassive.CharacterRef.Controller == _owner) {
								var gridObj = container.FindGridObject(aspectSelectedMsg.characterID);
								foreach (var aspect in gridObj.CharacterRef.Aspects) {
									if (aspect.ID == aspectSelectedMsg.aspectID) {
										container.CurrentPassiveSelectAspect(aspect, aspectSelectedMsg.reroll);
										break;
									}
								}
							}
						}
						break;
					case SelectAspectOverMessage.MESSAGE_TYPE: {
							var aspectOverMsg = (SelectAspectOverMessage)message;
							if (!container.IsChecking) return;
							if (SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_ASPECT
								&& container.Initiative.CharacterRef.Controller == _owner) {
								container.InitiativeAspectSelectionOver();
							} else if (SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_ASPECT
								  && container.CurrentPassive.CharacterRef.Controller == _owner) {
								container.CurrentPassiveAspectSelectionOver();
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

		protected BattleScene(Connection connection, User owner) :
			base(connection, owner) {
			connection.SetRequestHandler(BattleSceneGetActableObjectMovePathInfoMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(CheckerSkillSelectedMessage.MESSAGE_TYPE, this);
		}

		public void Open() {
			if (_isUsing) return;
			_isUsing = true;
		}

		public void Close() {
			if (!_isUsing) return;
			_isUsing = false;
		}

		public void Reset(int rows, int cols) {
			BattleSceneResetMessage message = new BattleSceneResetMessage();
			message.rows = rows;
			message.cols = cols;
			_connection.SendMessage(message);
		}

		public void PushGridObject(GridObject gridObject) {
			BattleScenePushGridObjectMessage message = new BattleScenePushGridObjectMessage();
			message.view = gridObject.CharacterRef.View;
			message.objData = StreamableFactory.CreateBattleSceneGridObjData(gridObject);
			_connection.SendMessage(message);
		}

		public void RemoveGridObject(GridObject gridObject) {
			BattleSceneRemoveGridObjectMessage message = new BattleSceneRemoveGridObjectMessage();
			message.gridObj = StreamableFactory.CreateBattleSceneObj(gridObject);
			_connection.SendMessage(message);
		}

		public void AddLadderObject(LadderObject ladderObject) {
			BattleSceneAddLadderObjectMessage message = new BattleSceneAddLadderObjectMessage();
			message.objData = StreamableFactory.CreateBattleSceneLadderObjData(ladderObject);
			message.view = ladderObject.CharacterRef.View;
			_connection.SendMessage(message);
		}

		public void RemoveLadderObject(LadderObject ladderObject) {
			BattleSceneRemoveLadderObjectMessage message = new BattleSceneRemoveLadderObjectMessage();
			message.ladderObj = StreamableFactory.CreateBattleSceneObj(ladderObject);
			_connection.SendMessage(message);
		}

		public void DisplayDicePoints(User who, int[] dicePoints) {
			if (!_isUsing) return;
			DisplayDicePointsMessage message = new DisplayDicePointsMessage();
			message.dicePoints = dicePoints;
			message.userID = who.Id;
			_connection.SendMessage(message);
		}

		public void SetActingOrder(List<ActableGridObject> actableObjects) {
			if (!_isUsing) return;
			var message = new BattleSceneSetActingOrderMessage();
			message.objOrder = new BattleSceneObj[actableObjects.Count];
			for (int i = 0; i < actableObjects.Count; ++i) {
				message.objOrder[i] = StreamableFactory.CreateBattleSceneObj(actableObjects[i]);
			}
			_connection.SendMessage(message);
		}

		public void ChangeTurn(ActableGridObject actable) {
			if (!_isUsing) return;
			_canOperate = actable.CharacterRef.Controller == _owner;
			var message = new BattleSceneChangeTurnMessage();
			message.canOperate = _canOperate;
			message.gridObj = StreamableFactory.CreateBattleSceneObj(actable);
			_connection.SendMessage(message);
		}

		public void DisplayActableObjectMove(ActableGridObject actable, BattleMapDirection direction, bool stairway) {
			if (!_isUsing) return;
			var message = new BattleSceneDisplayActableObjectMovingMessage();
			message.obj = StreamableFactory.CreateBattleSceneObj(actable);
			message.direction = (int)direction;
			message.stairway = stairway;
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectSkillOrStunt(SkillChecker.CharacterAction action, GridObject passive, GridObject initiative, SkillType initiativeSkillType) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.PASSIVE_SKILL_OR_STUNT) return;
			var message = new BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage();
			message.passiveObj = StreamableFactory.CreateBattleSceneObj(passive);
			message.initiativeObj = StreamableFactory.CreateBattleSceneObj(initiative);
			message.initiativeSkillType = StreamableFactory.CreateSkillTypeDescription(initiativeSkillType);
			message.action = (int)action;
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectSkillOrStuntComplete() {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.PASSIVE_SKILL_OR_STUNT) return;
			var message = new CheckerSelectSkillOrStuntCompleteMessage();
			message.isInitiative = false;
			message.failure = false;
			message.extraMessage = "";
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectSkillOrStuntFailure(string msg) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.PASSIVE_SKILL_OR_STUNT) return;
			var message = new CheckerSelectSkillOrStuntCompleteMessage();
			message.isInitiative = false;
			message.failure = true;
			message.extraMessage = msg;
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeSelectAspect(GridObject initiative) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.INITIATIVE_ASPECT) return;
			var message = new BattleSceneCheckerNotifySelectAspectMessage();
			message.isInitiative = true;
			message.obj = StreamableFactory.CreateBattleSceneObj(initiative);
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeSelectAspectComplete() {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.INITIATIVE_ASPECT) return;
			var message = new CheckerSelectAspectCompleteMessage();
			message.over = false;
			message.isInitiative = true;
			message.failure = false;
			message.extraMessage = "";
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeSelectAspectFailure(string msg) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.INITIATIVE_ASPECT) return;
			var message = new CheckerSelectAspectCompleteMessage();
			message.over = false;
			message.isInitiative = true;
			message.failure = true;
			message.extraMessage = msg;
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeSelectAspectOver() {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.INITIATIVE_ASPECT) return;
			var message = new CheckerSelectAspectCompleteMessage();
			message.over = true;
			message.isInitiative = true;
			message.failure = false;
			message.extraMessage = "";
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectAspect(GridObject passive) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.PASSIVE_ASPECT) return;
			var message = new BattleSceneCheckerNotifySelectAspectMessage();
			message.isInitiative = false;
			message.obj = StreamableFactory.CreateBattleSceneObj(passive);
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectAspectComplete() {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.PASSIVE_ASPECT) return;
			var message = new CheckerSelectAspectCompleteMessage();
			message.over = false;
			message.isInitiative = false;
			message.failure = false;
			message.extraMessage = "";
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectAspectFailure(string msg) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.PASSIVE_ASPECT) return;
			var message = new CheckerSelectAspectCompleteMessage();
			message.over = false;
			message.isInitiative = false;
			message.failure = true;
			message.extraMessage = msg;
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectAspectOver() {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.PASSIVE_ASPECT) return;
			var message = new CheckerSelectAspectCompleteMessage();
			message.over = true;
			message.isInitiative = false;
			message.failure = false;
			message.extraMessage = "";
			_connection.SendMessage(message);
		}

		public void UpdateSumPoint(GridObject gridObject, int point) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking) return;
			var message = new BattleSceneCheckerUpdateSumPointMessage();
			message.obj = StreamableFactory.CreateBattleSceneObj(gridObject);
			message.point = point;
			_connection.SendMessage(message);
		}

		public void DisplaySkillReady(GridObject gridObject, SkillType skillType, bool bigone) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking) return;
			var message = new BattleSceneCheckerDisplaySkillReadyMessage();
			message.obj = StreamableFactory.CreateBattleSceneObj(gridObject);
			message.skillTypeID = skillType.ID;
			message.bigone = bigone;
			_connection.SendMessage(message);
		}

		public void DisplayUsingAspect(GridObject userGridObject, GridObject ownerGridObject, Aspect aspect) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking) return;
			var message = new BattleSceneCheckerDisplayUsingAspectMessage();
			message.userObj = StreamableFactory.CreateBattleSceneObj(userGridObject);
			message.aspectOwnerObj = StreamableFactory.CreateBattleSceneObj(ownerGridObject);
			message.aspectID = aspect.ID;
			_connection.SendMessage(message);
		}
		
		public void GridObjectSkillEffect(GridObject gridObject, SkillType skillType, bool bigone) {
			if (!_isUsing) return;

		}

	}

	public sealed class DMBattleScene : BattleScene {
		public DMBattleScene(Connection connection, User owner) :
			base(connection, owner) {

		}
	}

	public sealed class PlayerBattleScene : BattleScene {
		public PlayerBattleScene(Connection connection, User owner) :
			base(connection, owner) {

		}
	}

}
