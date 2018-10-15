using GameServer.CharacterComponents;
using GameServer.Playground;
using GameServer.Playground.BattleComponent;
using GameServer.Core;
using GameServer.Core.DataSystem;
using GameUtil;
using GameUtil.Network;
using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
using System;
using System.Collections.Generic;
using GameServer.Campaign;

namespace GameServer.Client {
	public class BattleSceneProxy : ClientComponentProxy, IRequestHandler {
		protected bool _canOperate = false;
		protected bool _ignoreOperating = false;

		public override bool IsUsing => CampaignManager.Instance.CurrentContainer == ContainerType.BATTLE;

		protected BattleSceneMovePathInfoMessage CreateMovePathInfoMessage(List<ReachablePlace> reachablePlaces) {
			var ret = new BattleSceneMovePathInfoMessage();
			ret.pathInfo = new BattleSceneMovePathInfoMessage.ReachableGrid[reachablePlaces.Count];
			for (int i = 0; i < reachablePlaces.Count; ++i) {
				var prevPlace = reachablePlaces[i].prevPlace;
				ret.pathInfo[i].prevPlaceIndex = prevPlace != null ? reachablePlaces.IndexOf(prevPlace) : -1;
				ret.pathInfo[i].pos.row = reachablePlaces[i].pos.row;
				ret.pathInfo[i].pos.col = reachablePlaces[i].pos.col;
				ret.pathInfo[i].pos.highland = reachablePlaces[i].pos.highland;
				ret.pathInfo[i].leftMovePoint = reachablePlaces[i].leftMovePoint;
			}
			return ret;
		}

		public Message MakeResponse(Message request) {
			try {
				if (!IsUsing) return null;
				var container = BattleScene.Instance;
				switch (request.MessageType) {
					case BattleSceneGetGridObjectDataMessage.MESSAGE_TYPE: {
							var reqMsg = (BattleSceneGetGridObjectDataMessage)request;
							var resp = new BattleSceneGridObjectDataMessage();
							var gridObject = BattleScene.Instance.FindObject(reqMsg.objID) as GridObject;
							if (gridObject == null) return null;
							resp.objData = StreamableFactory.CreateBattleSceneGridObjData(gridObject);
							return resp;
						}
					case BattleSceneGetLadderObjectDataMessage.MESSAGE_TYPE: {
							var reqMsg = (BattleSceneGetLadderObjectDataMessage)request;
							var resp = new BattleSceneLadderObjectDataMessage();
							var ladderObject = BattleScene.Instance.FindObject(reqMsg.objID) as LadderObject;
							if (ladderObject == null) return null;
							resp.objData = StreamableFactory.CreateBattleSceneLadderObjData(ladderObject);
							return resp;
						}
					case BattleSceneGetMovePathInfoMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) break;
							var list = container.CurrentActable.GetReachablePlaceList();
							var resp = CreateMovePathInfoMessage(list);
							return resp;
						}
					case BattleSceneGetObjectCanExtraMoveMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) break;
							var resp = new BattleSceneCanObjectTakeExtraMoveMessage();
							resp.result = container.CurrentActable.CanTakeExtraMove();
							return resp;
						}
					case BattleSceneGetActionAffectableAreasMessage.MESSAGE_TYPE: {
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) break;
							var reqMsg = (BattleSceneGetActionAffectableAreasMessage)request;
							var resp = new BattleSceneActionAffectableAreasMessage();
							Dictionary<GridPos, List<GridPos>> areas;
							if (reqMsg.isStunt) {
								var stunt = container.CurrentActable.CharacterRef.FindStuntByID(reqMsg.skillTypeOrStuntID);
								areas = container.CurrentActable.GetStuntAffectableAreas(stunt);
							} else {
								var skillType = SkillType.SkillTypes[reqMsg.skillTypeOrStuntID];
								areas = container.CurrentActable.GetSkillAffectableAreas(skillType);
							}
							resp.centers = new GridPos[areas.Count];
							resp.areas = new GridPos[areas.Count][];
							int i = 0;
							foreach (var area in areas) {
								resp.centers[i] = new GridPos() { row = area.Key.row, col = area.Key.col, highland = area.Key.highland };
								var gridList = area.Value;
								int gridCount = area.Value.Count;
								resp.areas[i] = new GridPos[gridCount];
								for (int j = 0; j < gridCount; ++j) {
									resp.areas[i][j] = new GridPos() { row = gridList[j].row, col = gridList[j].col, highland = gridList[j].highland };
								}
								++i;
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
			if (!IsUsing || _ignoreOperating) return;
			var container = BattleScene.Instance;
			var checker = SkillChecker.Instance;
			try {
				switch (message.MessageType) {
					case BattleSceneActableObjectMoveMessage.MESSAGE_TYPE: {
							var msg = (BattleSceneActableObjectMoveMessage)message;
							if (checker.IsChecking) return;
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;
							container.CurrentActable.MoveTo(msg.dst.row, msg.dst.col, msg.dst.highland);
						}
						break;
					case BattleSceneObjectTakeExtraMovePointMessage.MESSAGE_TYPE: {
							if (checker.IsChecking) return;
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;
							container.CurrentActable.TakeExtraMovePoint();
						}
						break;
					case BattleSceneActableObjectDoActionMessage.MESSAGE_TYPE: {
							if (checker.IsChecking) return;
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;
							var msg = (BattleSceneActableObjectDoActionMessage)message;
							if (msg.isStunt) {
								Stunt selectedStunt = container.CurrentActable.CharacterRef.FindStuntByID(msg.skillTypeOrStuntID);
								List<SceneObject> targets = new List<SceneObject>();
								foreach (var msgTarget in msg.targets) {
									var target = container.FindObject(msgTarget);
									if (target != null)
										targets.Add(target);
								}
								if (selectedStunt != null)
									container.CurrentActable.UseStunt(selectedStunt, msg.dstCenter, targets, msg.action);
							} else {
								var skillType = SkillType.SkillTypes[msg.skillTypeOrStuntID];
								List<SceneObject> targets = new List<SceneObject>();
								foreach (var msgTarget in msg.targets) {
									var target = container.FindObject(msgTarget);
									if (target != null)
										targets.Add(target);
								}
								container.CurrentActable.UseSkill(skillType, msg.action, msg.dstCenter, targets);
							}
						}
						break;
					case BattleSceneActableObjectUseStuntDirectlyMessage.MESSAGE_TYPE: {
							if (checker.IsChecking) return;
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;
							var msg = (BattleSceneActableObjectUseStuntDirectlyMessage)message;
							Stunt selectedStunt = container.CurrentActable.CharacterRef.FindStuntByID(msg.stuntID);
							List<SceneObject> targets = new List<SceneObject>();
							foreach (var msgTarget in msg.targets) {
								var target = container.FindObject(msgTarget);
								if (target != null)
									targets.Add(target);
							}
							if (selectedStunt != null)
								container.CurrentActable.UseStunt(selectedStunt, msg.dstCenter, targets, 0);
						}
						break;
					case BattleSceneTurnOverMessage.MESSAGE_TYPE: {
							if (checker.IsChecking) return;
							if (!_canOperate || container.CurrentActable.CharacterRef.Controller != _owner) return;
							container.CurrentTurnOver();
						}
						break;
					default:
						break;
				}
			} catch (Exception e) {
				Logger.WriteLine(e.Message);
			}
		}

		protected BattleSceneProxy(Connection connection, User owner) :
			base(connection, owner) {
			connection.SetRequestHandler(BattleSceneGetGridObjectDataMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(BattleSceneGetLadderObjectDataMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(BattleSceneGetMovePathInfoMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(BattleSceneGetObjectCanExtraMoveMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(BattleSceneGetActionAffectableAreasMessage.MESSAGE_TYPE, this);

			connection.AddMessageReceiver(BattleSceneActableObjectMoveMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(BattleSceneObjectTakeExtraMovePointMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(BattleSceneActableObjectDoActionMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(BattleSceneActableObjectUseStuntDirectlyMessage.MESSAGE_TYPE, this);
			connection.AddMessageReceiver(BattleSceneTurnOverMessage.MESSAGE_TYPE, this);
		}

		public void SynchronizeData() {
			BattleScene.Instance.ClientSynchronizeData(this);
		}

		public void SynchronizeState() {
			BattleScene.Instance.ClientSynchronizeState(this);
		}
		
		public void Reset(int rows, int cols) {
			BattleSceneResetMessage message = new BattleSceneResetMessage();
			message.rows = rows;
			message.cols = cols;
			_connection.SendMessage(message);
		}

		public void UpdateGridData(Grid grid) {
			var message = new BattleSceneGridDataMessage();
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
			message.gridObjID = gridObject.ID;
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
			message.ladderObjID = ladderObject.ID;
			_connection.SendMessage(message);
		}
		
		public void StartBattle() {
			if (!IsUsing) return;
			var message = new BattleSceneStartBattleMessage();
			_connection.SendMessage(message);
		}

		public void UpdateTurnOrder(List<ActableGridObject> actableObjects) {
			if (!IsUsing) return;
			var message = new BattleSceneUpdateTurnOrderMessage();
			message.objsIDOrdered = new string[actableObjects.Count];
			for (int i = 0; i < actableObjects.Count; ++i) {
				message.objsIDOrdered[i] = actableObjects[i].ID;
			}
			_connection.SendMessage(message);
		}

		public void NewTurn(ActableGridObject actable) {
			if (!IsUsing) return;
			_canOperate = actable.CharacterRef.Controller == _owner;
			var message = new BattleSceneNewTurnMessage();
			message.objIDWhoseTurn = actable.ID;
			message.canOperate = _canOperate;
			_connection.SendMessage(message);
		}

		public void DisplayActableObjectMove(ActableGridObject actable, BattleMapDirection direction, bool stairway) {
			if (!IsUsing) return;
			var message = new BattleSceneDisplayObjectMovingMessage();
			message.objID = actable.ID;
			message.direction = direction;
			message.stairway = stairway;
			_connection.SendMessage(message);
		}

		public void DisplayTakeExtraMovePoint(ActableGridObject actable, SkillType usingSkillType) {
			if (!IsUsing) return;
			var message = new BattleSceneDisplayObjectTakeExtraMovePointMessage();
			message.objID = actable.ID;
			message.moveSkillTypeID = usingSkillType.ID;
			message.newMovePoint = actable.MovePoint;
			_connection.SendMessage(message);
		}

		public void UpdateActionPoint(ActableGridObject actable) {
			if (!IsUsing) return;
			var message = new BattleSceneUpdateActionPointMessage();
			message.objID = actable.ID;
			message.newActionPoint = actable.ActionPoint;
			_connection.SendMessage(message);
		}

		public void UpdateMovePoint(ActableGridObject actable) {
			if (!IsUsing) return;
			var message = new BattleSceneUpdateMovePointMessage();
			message.objID = actable.ID;
			message.newMovePoint = actable.MovePoint;
			_connection.SendMessage(message);
		}

		public override void WaitUserDecision(bool enabled) {
			_ignoreOperating = enabled;
		}
	}

	public sealed class DMBattleScene : BattleSceneProxy {
		public DMBattleScene(Connection connection, User owner) :
			base(connection, owner) {

		}
	}

	public sealed class PlayerBattleScene : BattleSceneProxy {
		public PlayerBattleScene(Connection connection, User owner) :
			base(connection, owner) {

		}
	}

}
