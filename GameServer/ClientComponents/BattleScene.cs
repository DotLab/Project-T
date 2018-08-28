using GameLib.CharacterSystem;
using GameLib.Container;
using GameLib.Container.BattleComponent;
using GameLib.Core;
using GameLib.Core.DataSystem;
using GameLib.Utilities;
using GameLib.Utilities.Network;
using GameLib.Utilities.Network.ClientMessages;
using GameLib.Utilities.Network.ServerMessages;
using GameLib.Utilities.Network.Streamable;
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
					case BattleSceneGetGridObjectDataMessage.MESSAGE_TYPE: {
							var reqMsg = (BattleSceneGetGridObjectDataMessage)request;
							var resp = new BattleSceneGridObjectDataMessage();
							var gridObject = BattleSceneContainer.Instance.FindObject(reqMsg.objID) as GridObject;
							if (gridObject == null) throw new InvalidOperationException("Invalid object id.");
							resp.objData = StreamableFactory.CreateBattleSceneGridObjData(gridObject);
							return resp;
						}
					case BattleSceneGetLadderObjectDataMessage.MESSAGE_TYPE: {
							var reqMsg = (BattleSceneGetLadderObjectDataMessage)request;
							var resp = new BattleSceneLadderObjectDataMessage();
							var ladderObject = BattleSceneContainer.Instance.FindObject(reqMsg.objID) as LadderObject;
							if (ladderObject == null) throw new InvalidOperationException("Invalid object id.");
							resp.objData = StreamableFactory.CreateBattleSceneLadderObjData(ladderObject);
							return resp;
						}
					case BattleSceneGetActableObjectMovePathInfoMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) break;
							var list = container.CurrentActable.GetReachablePlaceList();
							var resp = CreateMovePathInfoMessage(list);
							return resp;
						}
					case BattleSceneGetCanExtraMoveMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) break;
							var resp = new BattleSceneCanTakeExtraMoveMessage();
							resp.result = container.CurrentActable.CanTakeExtraMove();
							return resp;
						}
					case BattleSceneGetInitiativeUsableSkillOrStuntMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) break;
							var reqMsg = (BattleSceneGetInitiativeUsableSkillOrStuntMessage)request;
							if (reqMsg.stunt) {
								var resp = new BattleSceneObjectUsableStuntListMessage();
								var candoList = new List<Stunt>();
								if (container.CurrentActable.CharacterRef != null) {
									foreach (var stunt in container.CurrentActable.CharacterRef.Stunts) {
										if (container.CurrentActable.CanUseSkillOrStunt(stunt.SkillProperty, reqMsg.action)) {
											candoList.Add(stunt);
										}
									}
								}
								resp.stunts = new CharacterPropertyDescription[candoList.Count];
								for (int i = 0; i < candoList.Count; ++i) {
									resp.stunts[i] = StreamableFactory.CreateCharacterPropertyDescription(candoList[i]);
								}
								return resp;
							} else {
								var resp = new BattleSceneObjectUsableSkillListMessage();
								var candoList = new List<SkillType>();
								foreach (var skillType in SkillType.SkillTypes) {
									if (container.CurrentActable.CanUseSkillOrStunt(container.CurrentActable.CharacterRef.GetSkillProperty(skillType.Value), reqMsg.action)) {
										candoList.Add(skillType.Value);
									}
								}
								resp.skillTypes = new SkillTypeDescription[candoList.Count];
								for (int i = 0; i < candoList.Count; ++i) {
									resp.skillTypes[i] = StreamableFactory.CreateSkillTypeDescription(candoList[i]);
								}
								return resp;
							}
						}
					case BattleSceneGetPassiveUsableSkillOrStuntMessage.MESSAGE_TYPE: {
							if (!container.IsChecking || container.CurrentPassive.CharacterRef.Controller != _owner) break;
							var reqMsg = (BattleSceneGetPassiveUsableSkillOrStuntMessage)request;
							if (reqMsg.stunt) {
								var resp = new BattleSceneObjectUsableStuntListMessage();
								var candoList = new List<Stunt>();
								if (container.CurrentPassive.CharacterRef.Stunts != null) {
									foreach (var stunt in container.CurrentPassive.CharacterRef.Stunts) {
										if (container.CanCurrentPassiveUseSkillOrStunt(stunt.SkillProperty)) {
											candoList.Add(stunt);
										}
									}
								}
								resp.stunts = new CharacterPropertyDescription[candoList.Count];
								for (int i = 0; i < candoList.Count; ++i) {
									resp.stunts[i] = StreamableFactory.CreateCharacterPropertyDescription(candoList[i]);
								}
								return resp;
							} else {
								var resp = new BattleSceneObjectUsableSkillListMessage();
								var candoList = new List<SkillType>();
								foreach (var skillType in SkillType.SkillTypes) {
									if (container.CanCurrentPassiveUseSkillOrStunt(container.CurrentActable.CharacterRef.GetSkillProperty(skillType.Value))) {
										candoList.Add(skillType.Value);
									}
								}
								resp.skillTypes = new SkillTypeDescription[candoList.Count];
								for (int i = 0; i < candoList.Count; ++i) {
									resp.skillTypes[i] = StreamableFactory.CreateSkillTypeDescription(candoList[i]);
								}
								return resp;
							}
						}
					default:
						break;
				}
				return null;
			} catch (Exception e) {
				Logger.WriteLine(e.Message);
				throw e;
				return null;
			}
		}

		public override void MessageReceived(Message message) {
			if (!_isUsing) return;
			var container = BattleSceneContainer.Instance;
			try {
				switch (message.MessageType) {
					case BattleSceneActableObjectMoveMessage.MESSAGE_TYPE: {
							var msg = (BattleSceneActableObjectMoveMessage)message;
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;
							container.CurrentActable.MoveTo(msg.dstRow, msg.dstCol, msg.dstHighland);
						}
						break;
					case BattleSceneTakeExtraMovePointMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;
							container.CurrentActable.TakeExtraMove();
						}
						break;
					case BattleSceneActableObjectDoActionMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;
							var msg = (BattleSceneActableObjectDoActionMessage)message;
							if (msg.isStunt) {
								Stunt selectedStunt = null;
								foreach (var stunt in container.CurrentActable.CharacterRef.Stunts) {
									if (stunt.ID == msg.skillTypeOrStuntID) {
										selectedStunt = stunt;
										break;
									}
								}
								List<SceneObject> targets = new List<SceneObject>();
								foreach (var msgTarget in msg.targets) {
									var target = container.FindObject(msgTarget);
									if (target != null)
										targets.Add(target);
								}
								if (selectedStunt != null)
									container.CurrentActable.UseStunt(selectedStunt, msg.action, msg.dstRow, msg.dstCol, targets);
							} else {
								var skillType = SkillType.SkillTypes[msg.skillTypeOrStuntID];
								List<SceneObject> targets = new List<SceneObject>();
								foreach (var msgTarget in msg.targets) {
									var target = container.FindObject(msgTarget);
									if (target != null)
										targets.Add(target);
								}
								container.CurrentActable.UseSkill(skillType, msg.action, msg.dstRow, msg.dstCol, targets);
							}
						}
						break;
					case BattleSceneActableObjectDoSpecialActionMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;

						}
						break;
					case BattleSceneSetSkipSelectAspectMessage.MESSAGE_TYPE: {
							var msg = (BattleSceneSetSkipSelectAspectMessage)message;
							_skipSelectAspect = msg.val;
						}
						break;
					case CheckerSkillSelectedMessage.MESSAGE_TYPE: {
							var msg = (CheckerSkillSelectedMessage)message;
							if (!container.IsChecking) return;
							if (SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL_OR_STUNT
								&& container.CurrentPassive.CharacterRef.Controller == _owner) {
								var selectedSkillType = SkillType.SkillTypes[msg.skillTypeID];
								container.CurrentPassiveUseSkill(selectedSkillType);
							}
						}
						break;
					case CheckerStuntSelectedMessage.MESSAGE_TYPE: {
							var msg = (CheckerStuntSelectedMessage)message;
							if (!container.IsChecking) return;
							if (SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL_OR_STUNT
								&& container.CurrentPassive.CharacterRef.Controller == _owner) {
								foreach (var stunt in container.CurrentPassive.CharacterRef.Stunts) {
									if (stunt.ID == msg.stuntID) {
										container.CurrentPassiveUseStunt(stunt);
										break;
									}
								}
							}
						}
						break;
					case CheckerAspectSelectedMessage.MESSAGE_TYPE: {
							var msg = (CheckerAspectSelectedMessage)message;
							if (!container.IsChecking) return;
							if (SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_ASPECT
								&& container.Initiative.CharacterRef.Controller == _owner) {
								var gridObj = container.FindObject(msg.characterID);
								foreach (var aspect in gridObj.CharacterRef.Aspects) {
									if (aspect.ID == msg.aspectID) {
										container.InitiativeSelectAspect(aspect, msg.reroll);
										break;
									}
								}
							} else if (SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_ASPECT
								  && container.CurrentPassive.CharacterRef.Controller == _owner) {
								var gridObj = container.FindObject(msg.characterID);
								foreach (var aspect in gridObj.CharacterRef.Aspects) {
									if (aspect.ID == msg.aspectID) {
										container.CurrentPassiveSelectAspect(aspect, msg.reroll);
										break;
									}
								}
							}
						}
						break;
					case SelectAspectOverMessage.MESSAGE_TYPE: {
							var msg = (SelectAspectOverMessage)message;
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
					case BattleSceneTurnOverMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;
							container.CurrentTurnOver();
						}
						break;
					default:
						break;
				}
			} catch (Exception e) {
				Logger.WriteLine(e.Message);
				throw e;
			}
		}

		protected BattleScene(Connection connection, User owner) :
			base(connection, owner) {
			connection.SetRequestHandler(BattleSceneGetGridObjectDataMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(BattleSceneGetLadderObjectDataMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(BattleSceneGetActableObjectMovePathInfoMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(BattleSceneGetCanExtraMoveMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(BattleSceneGetInitiativeUsableSkillOrStuntMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(BattleSceneGetPassiveUsableSkillOrStuntMessage.MESSAGE_TYPE, this);

			connection.AddMessageReceiver(BattleSceneActableObjectMoveMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(BattleSceneTakeExtraMovePointMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(BattleSceneActableObjectDoActionMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(BattleSceneActableObjectDoSpecialActionMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(BattleSceneSetSkipSelectAspectMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(CheckerSkillSelectedMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(CheckerStuntSelectedMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(CheckerAspectSelectedMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(SelectAspectOverMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(BattleSceneTurnOverMessage.MESSAGE_TYPE, this);
		}

		public void SynchronizeData() {
			BattleSceneContainer.Instance.ClientSynchronizeData(this);
		}

		public void SynchronizeState() {
			BattleSceneContainer.Instance.ClientSynchronizeState(this);
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

		public void UpdateGridInfo(Grid grid) {
			var message = new BattleSceneUpdateGridInfoMessage();
			message.row = grid.PosRow;
			message.col = grid.PosCol;
			message.isMiddleLand = grid.IsMiddleLand;
			_connection.SendMessage(message);
		}

		public void PushGridObject(GridObject gridObject) {
			var message = new BattleScenePushGridObjectMessage();
			message.view = gridObject.CharacterRef.View;
			message.objData = StreamableFactory.CreateBattleSceneGridObjData(gridObject);
			_connection.SendMessage(message);
		}

		public void RemoveGridObject(GridObject gridObject) {
			var message = new BattleSceneRemoveGridObjectMessage();
			message.gridObj = StreamableFactory.CreateBattleSceneObj(gridObject);
			_connection.SendMessage(message);
		}

		public void AddLadderObject(LadderObject ladderObject) {
			var message = new BattleSceneAddLadderObjectMessage();
			message.objData = StreamableFactory.CreateBattleSceneLadderObjData(ladderObject);
			message.view = ladderObject.CharacterRef.View;
			_connection.SendMessage(message);
		}

		public void RemoveLadderObject(LadderObject ladderObject) {
			var message = new BattleSceneRemoveLadderObjectMessage();
			message.ladderObj = StreamableFactory.CreateBattleSceneObj(ladderObject);
			_connection.SendMessage(message);
		}

		public void DisplayDicePoints(User who, int[] dicePoints) {
			if (!_isUsing) return;
			var message = new DisplayDicePointsMessage();
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
			message.direction = direction;
			message.stairway = stairway;
			_connection.SendMessage(message);
		}

		public void DisplayTakeExtraMovePoint(ActableGridObject actable, SkillType usingSkillType) {
			if (!_isUsing) return;
			var message = new BattleSceneDisplayTakeExtraMovePointMessage();
			message.obj = StreamableFactory.CreateBattleSceneObj(actable);
			message.moveSkillType = StreamableFactory.CreateSkillTypeDescription(usingSkillType);
			message.newMovePoint = actable.MovePoint;
			_connection.SendMessage(message);
		}

		public void NotifyPassiveSelectSkillOrStunt(CharacterAction action, SceneObject passive, SceneObject initiative, SkillType initiativeSkillType) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.PASSIVE_SKILL_OR_STUNT) return;
			var message = new BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage();
			message.passiveObj = StreamableFactory.CreateBattleSceneObj(passive);
			message.initiativeObj = StreamableFactory.CreateBattleSceneObj(initiative);
			message.initiativeSkillType = StreamableFactory.CreateSkillTypeDescription(initiativeSkillType);
			message.action = action;
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

		public void NotifyInitiativeSelectAspect() {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.INITIATIVE_ASPECT) return;
			var message = new BattleSceneCheckerNotifySelectAspectMessage();
			message.isInitiative = true;
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

		public void NotifyPassiveSelectAspect() {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking || SkillChecker.Instance.State != SkillChecker.CheckerState.PASSIVE_ASPECT) return;
			var message = new BattleSceneCheckerNotifySelectAspectMessage();
			message.isInitiative = false;
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

		public void UpdateSumPoint(SceneObject sceneObject, int point) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking) return;
			var message = new BattleSceneCheckerUpdateSumPointMessage();
			message.obj = StreamableFactory.CreateBattleSceneObj(sceneObject);
			message.point = point;
			_connection.SendMessage(message);
		}

		public void DisplaySkillReady(SceneObject sceneObject, SkillType skillType, bool bigone) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking) return;
			var message = new BattleSceneCheckerDisplaySkillReadyMessage();
			message.obj = StreamableFactory.CreateBattleSceneObj(sceneObject);
			message.skillTypeID = skillType.ID;
			message.bigone = bigone;
			_connection.SendMessage(message);
		}

		public void DisplayUsingAspect(SceneObject userSceneObject, SceneObject ownerSceneObject, Aspect aspect) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking) return;
			var message = new BattleSceneCheckerDisplayUsingAspectMessage();
			message.userObj = StreamableFactory.CreateBattleSceneObj(userSceneObject);
			message.aspectOwnerObj = StreamableFactory.CreateBattleSceneObj(ownerSceneObject);
			message.aspectID = aspect.ID;
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeSelectSkillOrStuntComplete() {
			if (!_isUsing) return;
			var message = new CheckerSelectSkillOrStuntCompleteMessage();
			message.isInitiative = true;
			message.failure = false;
			message.extraMessage = "";
			_connection.SendMessage(message);
		}

		public void NotifyInitiativeSelectSkillOrStuntFailure(string msg) {
			if (!_isUsing) return;
			var message = new CheckerSelectSkillOrStuntCompleteMessage();
			message.isInitiative = true;
			message.failure = true;
			message.extraMessage = msg;
			_connection.SendMessage(message);
		}

		public void UpdateActionPoint(ActableGridObject actable) {
			if (!_isUsing) return;
			var message = new BattleSceneUpdateActionPointMessage();
			message.obj = StreamableFactory.CreateBattleSceneObj(actable);
			message.newActionPoint = actable.ActionPoint;
			_connection.SendMessage(message);
		}
		
		public void UpdateMovePoint(ActableGridObject actable) {
			if (!_isUsing) return;
			var message = new BattleSceneUpdateMovePointMessage();
			message.obj = StreamableFactory.CreateBattleSceneObj(actable);
			message.newMovePoint = actable.MovePoint;
			_connection.SendMessage(message);
		}

		public void StartCheck(SceneObject initiative, SkillType initiativeSkillType, CharacterAction action, IEnumerable<SceneObject> targets) {
			if (!_isUsing) return;
			var message = new BattleSceneStartCheckMessage();
			message.initiativeObj = StreamableFactory.CreateBattleSceneObj(initiative);
			message.initiativeSkillType = StreamableFactory.CreateSkillTypeDescription(initiativeSkillType);
			message.action = action;
			int count = 0;
			foreach (var target in targets) ++count;
			message.targets = new BattleSceneObj[count];
			int i = 0;
			foreach (var target in targets) {
				message.targets[i++] = StreamableFactory.CreateBattleSceneObj(target);
			}
			_connection.SendMessage(message);
		}

		public void CheckNextone(SceneObject nextone) {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking) return;
			var message = new BattleSceneCheckNextoneMessage();
			message.nextone = StreamableFactory.CreateBattleSceneObj(nextone);
			_connection.SendMessage(message);
		}

		public void EndCheck() {
			if (!_isUsing || !BattleSceneContainer.Instance.IsChecking) return;
			var message = new BattleSceneEndCheckMessage();
			_connection.SendMessage(message);
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
