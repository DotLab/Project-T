using GameUtil;
using GameUtil.Network;
using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TextyClient {
	public partial class BattleSceneForm : Form, IMessageReceiver {
		private abstract class BattleSceneObject {
			public readonly string id;
			public readonly CharacterView view;
			public Color color = Color.Black;

			public abstract bool Actable { get; }
			public abstract bool Movable { get; }

			public BattleSceneObject(string id, CharacterView view) {
				this.id = id;
				this.view = view;
			}

			public override string ToString() {
				return view.battle;
			}
		}

		private class GridObject : BattleSceneObject {
			public BattleMapDirection direction;
			public bool actable;
			public bool movable;

			public override bool Actable => actable;
			public override bool Movable => movable;

			public GridObject(string id, CharacterView view) :
				base(id, view) {

			}
		}

		private class SideObject : BattleSceneObject {
			public override bool Actable => false;
			public override bool Movable => false;

			public SideObject(string id, CharacterView view) :
				base(id, view) {

			}
		}

		private class Grid {
			public readonly List<GridObject> highland;
			public readonly List<GridObject> lowland;
			public SideObject positiveRowLadder;
			public SideObject positiveColLadder;
			public SideObject negativeRowLadder;
			public SideObject negativeColLadder;
			public bool isMiddleLand;

			public readonly int row;
			public readonly int col;

			public List<BattleSceneObject> Selections(bool isHighland) {
				var ret = new List<BattleSceneObject>();
				if (isHighland) {
					ret.AddRange(highland);
				} else {
					ret.AddRange(lowland);
				}
				if (positiveRowLadder != null) ret.Add(positiveRowLadder);
				if (positiveColLadder != null) ret.Add(positiveColLadder);
				if (negativeRowLadder != null) ret.Add(negativeRowLadder);
				if (negativeColLadder != null) ret.Add(negativeColLadder);
				return ret;
			}

			public Grid(int row, int col) {
				this.row = row;
				this.col = col;
				highland = new List<GridObject>();
				lowland = new List<GridObject>();
			}
		}

		private struct ActableObjWithAP {
			public GridObject actable;
			public bool currentActing;

			public override string ToString() {
				return (currentActing ? "→" : "") + actable.ToString();
			}
		}

		private class ReachablePlace {
			public ReachablePlace prevPlace;
			public int row;
			public int col;
			public bool highland;
			public int leftMovePoint;
		}

		private enum PassiveCheckingState {
			IDLE,
			SELECT_SKILL_OR_STUNT,
			SELECT_ASPECT
		}

		private enum InitiativeState {
			ACTING,
			CHECKING_SELECT_ASPECT
		}

		private Grid[,] _grids = null;
		private int _rows = -1;
		private int _cols = -1;
		private const float DIAMOND_LENGTH = 64.0f;

		private int _selectedRow = -1;
		private int _selectedCol = -1;
		private bool _selectedHighland = false;
		private BattleSceneObject _specifiedSelectedObject = null;

		private bool _waitingForUserDetermin = false;

		private BindingList<ActableObjWithAP> _actingOrder = new BindingList<ActableObjWithAP>();
		private string _actingObjID = null;
		private int _actingObjRow = -1;
		private int _actingObjCol = -1;
		private bool _canActing = false;
		private InitiativeState _initiativeState = InitiativeState.ACTING;
		private PassiveCheckingState _checkingStateAsPassive = PassiveCheckingState.IDLE;
		private CharacterAction _checkingAction = 0;
		private readonly BindingList<CharacterPropertyInfo> _selectionList = new BindingList<CharacterPropertyInfo>();
		private readonly BindingList<CharacterPropertyInfo> _selectionList2 = new BindingList<CharacterPropertyInfo>();

		private ReachablePlace[] _movePathInfo = null;
		private ReachablePlace _selectedMoveDst = null;
		private bool _createAspectReady = false;
		private bool _attackReady = false;
		private bool _interactReady = false;
		private bool _isUsingSkillOrStunt = false;
		private string _usingSkillOrStuntID = null;
		private bool _isStunt = false;
		private Dictionary<GridPos, List<GridPos>> _affectableAreas = null;
		private bool _isSelectingAffectCenter = false;
		private GridPos _selectedAffectCenter;
		private int _targetMaxCount = 0;
		private bool _isSelectingTarget = false;
		private readonly BindingList<BattleSceneObject> _targets = new BindingList<BattleSceneObject>();

		private bool _isChecking = false;
		private ReachablePlace _currentCheckPlace = null;
		private GridObject _currentPassiveObj = null;
		private List<ReachablePlace> _targetPlaces = null;

		public void MessageReceived(GameUtil.Network.Message message) {
			var printer = Program.mainForm.AdvancedConsole.TextBoxPrinter;
			switch (message.MessageType) {
				case WaitingForUserDeterminMessage.MESSAGE_TYPE: {
						var msg = (WaitingForUserDeterminMessage)message;
						_waitingForUserDetermin = msg.enabled;
					}
					break;
				case ShowSceneMessage.MESSAGE_TYPE: {
						var msg = (ShowSceneMessage)message;
						if (msg.sceneType == 1) {
							this.Visible = true;
						} else {
							this.Visible = false;
						}
					}
					break;
				case DisplayDicePointsMessage.MESSAGE_TYPE: {
						var msg = (DisplayDicePointsMessage)message;
						string dicePointsLbl = msg.userID + "投出了: ";
						int sum = 0;
						foreach (int point in msg.dicePoints) {
							dicePointsLbl += point + ", ";
							sum += point;
						}
						dicePointsLbl += "共计: " + sum;
						printer.WriteLine(dicePointsLbl);
					}
					break;
				case BattleSceneResetMessage.MESSAGE_TYPE: {
						var msg = (BattleSceneResetMessage)message;
						this.InitGrids(msg.rows, msg.cols);
						_selectedRow = -1;
						_selectedCol = -1;
						_selectedHighland = false;
					}
					break;
				case BattleSceneUpdateGridDataMessage.MESSAGE_TYPE: {
						var msg = (BattleSceneUpdateGridDataMessage)message;
						_grids[msg.row, msg.col].isMiddleLand = msg.isMiddleLand;
					}
					break;
				case BattleScenePushGridObjectMessage.MESSAGE_TYPE: {
						var msg = (BattleScenePushGridObjectMessage)message;
						Grid grid = _grids[msg.objData.obj.row, msg.objData.obj.col];
						GridObject gridObject = new GridObject(msg.objData.obj.id, msg.view);
						gridObject.direction = msg.objData.direction;
						bool actable = gridObject.actable = msg.objData.actable;
						if (actable) {
							gridObject.movable = msg.objData.actableObjData.movable;
						} else {
							gridObject.movable = false;
						}
						if (msg.objData.highland) {
							grid.highland.Add(gridObject);
						} else {
							grid.lowland.Add(gridObject);
						}
						var req = new GetCharacterDataMessage();
						req.characterID = gridObject.id;
						req.dataType = GetCharacterDataMessage.DataType.GROUP;
						Program.connection.Request(req, resp => {
							var result = resp as CharacterGroupDataMessage;
							if (result != null) {
								switch (result.token) {
									case CharacterToken.PLAYER:
										gridObject.color = Color.RoyalBlue;
										break;
									case CharacterToken.FRIENDLY:
										gridObject.color = Color.Green;
										break;
									case CharacterToken.NEUTRAL:
										gridObject.color = Color.Yellow;
										break;
									case CharacterToken.NEUTRAL_HOSTILE:
										gridObject.color = Color.Orange;
										break;
									case CharacterToken.HOSTILE:
										gridObject.color = Color.Red;
										break;
									default:
										break;
								}
							}
						});
					}
					break;
				case BattleSceneRemoveGridObjectMessage.MESSAGE_TYPE: {
						var removeGridObjMessage = (BattleSceneRemoveGridObjectMessage)message;
						Grid grid = _grids[removeGridObjMessage.gridObj.row, removeGridObjMessage.gridObj.col];
						for (int i = grid.lowland.Count - 1; i >= 0; --i) {
							if (grid.lowland[i].id == removeGridObjMessage.gridObj.id) {
								grid.lowland.RemoveAt(i);
								return;
							}
						}
						for (int i = grid.highland.Count - 1; i >= 0; --i) {
							if (grid.highland[i].id == removeGridObjMessage.gridObj.id) {
								grid.highland.RemoveAt(i);
								return;
							}
						}
					}
					break;
				case BattleSceneAddLadderObjectMessage.MESSAGE_TYPE: {
						BattleSceneAddLadderObjectMessage addLadderObjMessage = (BattleSceneAddLadderObjectMessage)message;
						int row = addLadderObjMessage.objData.obj.row;
						int col = addLadderObjMessage.objData.obj.col;
						Grid grid = _grids[row, col];
						SideObject sideObject = new SideObject(addLadderObjMessage.objData.obj.id, addLadderObjMessage.view);
						switch (addLadderObjMessage.objData.direction) {
							case BattleMapDirection.POSITIVE_ROW:
								grid.positiveRowLadder = sideObject;
								_grids[row + 1, col].negativeRowLadder = sideObject;
								break;
							case BattleMapDirection.POSITIVE_COL:
								grid.positiveRowLadder = sideObject;
								_grids[row, col + 1].negativeColLadder = sideObject;
								break;
							case BattleMapDirection.NEGATIVE_ROW:
								grid.positiveRowLadder = sideObject;
								_grids[row - 1, col].positiveRowLadder = sideObject;
								break;
							case BattleMapDirection.NEGATIVE_COL:
								grid.positiveRowLadder = sideObject;
								_grids[row, col - 1].positiveColLadder = sideObject;
								break;
							default:
								return;
						}
					}
					break;
				case BattleSceneRemoveLadderObjectMessage.MESSAGE_TYPE: {
						BattleSceneRemoveLadderObjectMessage removeLadderObjMessage = (BattleSceneRemoveLadderObjectMessage)message;
						Grid grid = _grids[removeLadderObjMessage.ladderObj.row, removeLadderObjMessage.ladderObj.col];
						if (grid.positiveRowLadder != null && grid.positiveRowLadder.id == removeLadderObjMessage.ladderObj.id) {
							grid.positiveRowLadder = null;
							_grids[removeLadderObjMessage.ladderObj.row + 1, removeLadderObjMessage.ladderObj.col].negativeRowLadder = null;
						} else if (grid.positiveColLadder != null && grid.positiveColLadder.id == removeLadderObjMessage.ladderObj.id) {
							grid.positiveColLadder = null;
							_grids[removeLadderObjMessage.ladderObj.row, removeLadderObjMessage.ladderObj.col + 1].negativeColLadder = null;
						} else if (grid.negativeRowLadder != null && grid.negativeRowLadder.id == removeLadderObjMessage.ladderObj.id) {
							grid.negativeRowLadder = null;
							_grids[removeLadderObjMessage.ladderObj.row - 1, removeLadderObjMessage.ladderObj.col].positiveRowLadder = null;
						} else if (grid.negativeColLadder != null && grid.negativeColLadder.id == removeLadderObjMessage.ladderObj.id) {
							grid.negativeColLadder = null;
							_grids[removeLadderObjMessage.ladderObj.row, removeLadderObjMessage.ladderObj.col - 1].positiveColLadder = null;
						}
					}
					break;
				case BattleSceneStartBattleMessage.MESSAGE_TYPE: {
						
					}
					break;
				case BattleSceneUpdateTurnOrderMessage.MESSAGE_TYPE: {
						var orderMessage = (BattleSceneUpdateTurnOrderMessage)message;
						_actingOrder.Clear();
						foreach (var obj in orderMessage.objOrder) {
							var gridObject = GetGridObject(obj.row, obj.col, obj.id, out bool highland);
							var objWithAP = new ActableObjWithAP { actable = gridObject };
							if (_actingObjID == gridObject.id) objWithAP.currentActing = true;
							_actingOrder.Add(objWithAP);
						}
					}
					break;
				case BattleSceneNewTurnMessage.MESSAGE_TYPE: {
						var orderMessage = (BattleSceneNewTurnMessage)message;
						_canActing = orderMessage.canOperate;
						_actingObjID = orderMessage.gridObj.id;
						_actingObjRow = orderMessage.gridObj.row;
						_actingObjCol = orderMessage.gridObj.col;
						if (orderMessage.canOperate) {
							roundInfoPanel.Visible = true;
							roundInfoLbl.Text = "你的回合";
						} else {
							roundInfoPanel.Visible = false;
							roundInfoLbl.Text = GetGridObject(orderMessage.gridObj, out bool highland) + " 的回合";
						}
						for (int i = 0; i < _actingOrder.Count; ++i) {
							if (_actingOrder[i].actable.id == orderMessage.gridObj.id) {
								var val = _actingOrder[i];
								val.currentActing = true;
								_actingOrder[i] = val;
							} else {
								var val = _actingOrder[i];
								val.currentActing = false;
								_actingOrder[i] = val;
							}
						}
						this.BattleSceneLookAtGrid(_actingObjRow, _actingObjCol);
					}
					break;
				case BattleSceneUpdateActionPointMessage.MESSAGE_TYPE: {
						var updateAPMsg = (BattleSceneUpdateActionPointMessage)message;
						if (_canActing && updateAPMsg.obj.id == _actingObjID) {
							actionPointLbl.Text = updateAPMsg.newActionPoint.ToString();
						}
					}
					break;
				case BattleSceneUpdateMovePointMessage.MESSAGE_TYPE: {
						var updateAPMsg = (BattleSceneUpdateMovePointMessage)message;
						if (_canActing && updateAPMsg.obj.id == _actingObjID) {
							movePointLbl.Text = updateAPMsg.newMovePoint.ToString();
						}
					}
					break;
				case BattleSceneDisplayActableObjectMovingMessage.MESSAGE_TYPE: {
						var moveMsg = (BattleSceneDisplayActableObjectMovingMessage)message;
						var grid = _grids[moveMsg.obj.row, moveMsg.obj.col];
						GridObject gridObject = null;
						bool highland = false;
						foreach (var lowObj in grid.lowland) {
							if (lowObj.id == moveMsg.obj.id) {
								gridObject = lowObj;
								break;
							}
						}
						if (gridObject == null) {
							foreach (var highObj in grid.highland) {
								if (highObj.id == moveMsg.obj.id) {
									gridObject = highObj;
									highland = true;
									break;
								}
							}
						}
						if (gridObject == null) throw new InvalidOperationException("Invalid message.");
						switch (moveMsg.direction) {
							case BattleMapDirection.POSITIVE_ROW:
								moveMsg.obj.row += 1;
								break;
							case BattleMapDirection.POSITIVE_COL:
								moveMsg.obj.col += 1;
								break;
							case BattleMapDirection.NEGATIVE_ROW:
								moveMsg.obj.row -= 1;
								break;
							case BattleMapDirection.NEGATIVE_COL:
								moveMsg.obj.col -= 1;
								break;
							default:
								throw new InvalidOperationException("Invalid message.");
						}
						MoveGridObjectStack(grid.row, grid.col, highland, moveMsg.obj.row, moveMsg.obj.col, highland ^ moveMsg.stairway);
						if (moveMsg.obj.id == _actingObjID) {
							_actingObjRow = moveMsg.obj.row;
							_actingObjCol = moveMsg.obj.col;
						}
					}
					break;
				case BattleSceneDisplayTakeExtraMovePointMessage.MESSAGE_TYPE: {
						var extraMoveMsg = (BattleSceneDisplayTakeExtraMovePointMessage)message;
						if (_canActing && extraMoveMsg.obj.id == _actingObjID) {
							movePointLbl.Text = extraMoveMsg.newMovePoint.ToString();
						}
					}
					break;
				case BattleSceneDisplayUsingStuntMessage.MESSAGE_TYPE: {
						var msg = (BattleSceneDisplayUsingStuntMessage)message;
						var gridObject = GetGridObject(msg.obj, out bool highland);
						printer.WriteLine(gridObject + " 使用特技 " + msg.stunt.describable.name);
					}
					break;
				case BattleSceneStartCheckMessage.MESSAGE_TYPE: {
						var msg = (BattleSceneStartCheckMessage)message;
						var initiativeObject = GetGridObject(msg.initiativeObj, out bool highland);
						_checkingAction = msg.action;
						_targetPlaces = new List<ReachablePlace>();
						printer.Write(initiativeObject + " 对 ");
						foreach (var target in msg.targets) {
							var passiveObject = GetGridObject(target, out bool targetHighland);
							printer.Write(passiveObject + " ");
							_targetPlaces.Add(new ReachablePlace() { row = target.row, col = target.col, highland = targetHighland });
						}
						printer.WriteLine("使用 " + msg.initiativeSkillType.name + " 进行" + msg.action);
						printer.WriteLine("检定开始");
						_isChecking = true;
					}
					break;
				case CheckerCheckResultMessage.MESSAGE_TYPE: {
						var msg = (CheckerCheckResultMessage)message;
						var initiativeObj = GetGridObject(_actingObjRow, _actingObjCol, _actingObjID, out bool highland);
						printer.WriteLine(initiativeObj + " 结果为 " + msg.initiative + "， " + _currentPassiveObj + " 结果为 " + msg.passive + "，成功度为" + Math.Abs(msg.delta));
					}
					break;
				case BattleSceneCheckNextoneMessage.MESSAGE_TYPE: {
						var msg = (BattleSceneCheckNextoneMessage)message;
						if (_currentCheckPlace != null) _targetPlaces.Remove(_currentCheckPlace);
						var obj = GetGridObject(msg.nextone, out bool highland);
						_currentPassiveObj = obj;
						printer.WriteLine("轮到" + obj + "进行被动检定");
						foreach (var targetPlace in _targetPlaces) {
							if (targetPlace.row == msg.nextone.row && targetPlace.col == msg.nextone.col && targetPlace.highland == highland) {
								_currentCheckPlace = targetPlace;
								break;
							}
						}
					}
					break;
				case BattleSceneEndCheckMessage.MESSAGE_TYPE: {
						if (_currentCheckPlace != null) _targetPlaces.Remove(_currentCheckPlace);
						_checkingAction = 0;
						_currentPassiveObj = null;
						_currentCheckPlace = null;
						_targetPlaces = null;
						printer.WriteLine("检定结束");
						_isChecking = false;
					}
					break;
				case BattleSceneCheckerDisplaySkillReadyMessage.MESSAGE_TYPE: {
						var msg = (BattleSceneCheckerDisplaySkillReadyMessage)message;
						var gridObject = GetGridObject(msg.obj, out bool highland);
						var nameReq = new GetSkillDataMessage() {
							characterID = gridObject.id,
							skillTypeID = msg.skillTypeID
						};
						Program.connection.Request(nameReq, result => {
							var resp = result as SkillDataMessage;
							if (resp != null) {
								printer.WriteLine(gridObject + " 将要使用 " + resp.customName);
							}
						});
					}
					break;
				case BattleSceneCheckerDisplayUsingAspectMessage.MESSAGE_TYPE: {
						var msg = (BattleSceneCheckerDisplayUsingAspectMessage)message;
						var userObj = GetGridObject(msg.userObj, out bool h1);
						var ownerObj = GetGridObject(msg.aspectOwnerObj, out bool h2);
						printer.WriteLine(userObj + " 声明 " + ownerObj + " 的 " + msg.aspect.describable.name + " 是对自己有利的");
					}
					break;
				case BattleSceneCheckerUpdateSumPointMessage.MESSAGE_TYPE: {
						var msg = (BattleSceneCheckerUpdateSumPointMessage)message;
						var gridObject = GetGridObject(msg.obj, out bool highland);
						printer.WriteLine(gridObject + " 的点数总和为：" + msg.point);
					}
					break;
				case BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage.MESSAGE_TYPE: {
						if (_checkingStateAsPassive != PassiveCheckingState.IDLE) return;
						var selectSkillMsg = (BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage)message;
						var getUsableActionRequest = new BattleSceneGetPassiveUsableSkillOrStuntListMessage() { stunt = false };
						var getDirectResistRequest = new GetDirectResistSkillsMessage() {
							actionType = selectSkillMsg.action,
							initiativeSkillTypeID = selectSkillMsg.initiativeSkillType.id
						};
						_selectionList.Clear();
						_selectionList2.Clear();
						selectionTypeGroupPanel.Visible = true;
						skillRbn.Checked = true;
						stuntRbn.Checked = false;
						selectionTypeLbl.Text = "技能选择";
						selectionListBox.DataSource = _selectionList;
						_checkingStateAsPassive = PassiveCheckingState.SELECT_SKILL_OR_STUNT;
						Program.connection.Request(getUsableActionRequest, result => {
							var resp = result as BattleSceneObjectUsableSkillListMessage;
							if (resp != null) {
								Program.connection.Request(getDirectResistRequest, result2 => {
									var resp2 = result2 as DirectResistSkillsListMessage;
									if (resp2 != null) {
										foreach (var skillType in resp.skillTypes) {
											bool contain = false;
											foreach (var match in resp2.skillTypes) {
												if (match.id == skillType.id) {
													_selectionList.Add(new CharacterPropertyInfo() {
														name = skillType.name,
														propertyID = skillType.id
													});
													contain = true;
													break;
												}
											}
											if (!contain) {
												_selectionList.Add(new CharacterPropertyInfo() {
													name = skillType.name,
													propertyID = skillType.id,
													extraMessage = "*"
												});
											}
										}
									}
								});
							}
						});
						getUsableActionRequest.stunt = true;
						Program.connection.Request(getUsableActionRequest, result => {
							var resp = result as BattleSceneObjectUsableStuntListMessage;
							if (resp != null) {
								var getDirectStuntsRequest = new GetDirectResistStuntsMessage() {
									passiveCharacterID = selectSkillMsg.passiveObj.id,
									actionType = selectSkillMsg.action,
									initiativeSkillTypeID = selectSkillMsg.initiativeSkillType.id
								};
								Program.connection.Request(getDirectStuntsRequest, listResult => {
									var listResp = listResult as DirectResistStuntsListMessage;
									if (listResp != null) {
										foreach (var stunt in resp.stunts) {
											bool contain = false;
											foreach (var match in listResp.stunts) {
												if (match.propertyID == stunt.propertyID) {
													_selectionList2.Add(new CharacterPropertyInfo() {
														name = stunt.describable.name,
														description = stunt.describable.description,
														propertyID = stunt.propertyID,
													});
													contain = true;
													break;
												}
											}
											if (!contain) {
												_selectionList2.Add(new CharacterPropertyInfo() {
													name = stunt.describable.name,
													description = stunt.describable.description,
													propertyID = stunt.propertyID,
													extraMessage = "*"
												});
											}
										}
									}
								});
								
							}
						});
						printer.WriteLine("进行被动对抗");
					}
					break;
				case CheckerSelectSkillOrStuntCompleteMessage.MESSAGE_TYPE: {
						var completeMsg = (CheckerSelectSkillOrStuntCompleteMessage)message;
						if (completeMsg.failure) {
							printer.WriteLine("失败" + completeMsg.extraMessage);
							if (_checkingStateAsPassive == PassiveCheckingState.IDLE && _canActing && _initiativeState == InitiativeState.ACTING && completeMsg.isInitiative) {
								_createAspectReady = _attackReady = _interactReady = false;
								menuItemCreateAspect.Text = "创造优势";
								menuItemAttack.Text = "攻击";
								menuItemInteract.Text = "其他动作";
								_selectionList.Clear();
								_selectionList2.Clear();
								selectionTypeLbl.Text = "";
								selectionListBox.DataSource = _selectionList;
								selectionTypeGroupPanel.Visible = false;
								_isUsingSkillOrStunt = false;
								_usingSkillOrStuntID = null;
								_isSelectingTarget = false;
								confirmTargetBtn.Visible = false;
								targetCountLbl.Visible = false;
								_targets.Clear();
								menuItemConfirmGrid.Text = "确认位置";
							}
						} else {
							if (_checkingStateAsPassive == PassiveCheckingState.SELECT_SKILL_OR_STUNT && !completeMsg.isInitiative) {
								_selectionList.Clear();
								_selectionList2.Clear();
								selectionTypeGroupPanel.Visible = false;
								selectionTypeLbl.Text = "";
								selectionListBox.DataSource = _selectionList;
								_checkingStateAsPassive = PassiveCheckingState.IDLE;
							} else if (_checkingStateAsPassive == PassiveCheckingState.IDLE) {
								if (_canActing && _initiativeState == InitiativeState.ACTING && completeMsg.isInitiative) {
									_createAspectReady = _attackReady = _interactReady = false;
									menuItemCreateAspect.Text = "创造优势";
									menuItemAttack.Text = "攻击";
									menuItemInteract.Text = "其他动作";
									_selectionList.Clear();
									_selectionList2.Clear();
									selectionTypeLbl.Text = "";
									selectionListBox.DataSource = _selectionList;
									selectionTypeGroupPanel.Visible = false;
									_isUsingSkillOrStunt = false;
									_usingSkillOrStuntID = null;
									_isSelectingTarget = false;
									confirmTargetBtn.Visible = false;
									targetCountLbl.Visible = false;
									_targets.Clear();
									menuItemConfirmGrid.Text = "确认位置";
								}
							}
						}
					}
					break;
				case BattleSceneCheckerNotifySelectAspectMessage.MESSAGE_TYPE: {
						var selectAspectMsg = (BattleSceneCheckerNotifySelectAspectMessage)message;
						if (_checkingAction == CharacterAction.HINDER) {
							selectAspectMsg.isInitiative = !selectAspectMsg.isInitiative;
						}
						if (selectAspectMsg.isInitiative) {
							if (!_canActing || _initiativeState == InitiativeState.CHECKING_SELECT_ASPECT) return;
							_initiativeState = InitiativeState.CHECKING_SELECT_ASPECT;
						} else {
							if (_checkingStateAsPassive != PassiveCheckingState.IDLE) return;
							_checkingStateAsPassive = PassiveCheckingState.SELECT_ASPECT;
						}
						_selectionList.Clear();
						selectionTypeLbl.Text = "特征选择";
						selectionListBox.DataSource = _selectionList;
						selectAspectOverBtn.Visible = true;
						printer.WriteLine("选择特征");
					}
					break;
				case CheckerSelectAspectCompleteMessage.MESSAGE_TYPE: {
						var completeMsg = (CheckerSelectAspectCompleteMessage)message;
						if (completeMsg.over) {
							if (_checkingAction == CharacterAction.HINDER) {
								completeMsg.isInitiative = !completeMsg.isInitiative;
							}
							if (completeMsg.isInitiative) {
								if (_initiativeState != InitiativeState.CHECKING_SELECT_ASPECT) return;
								_selectionList.Clear();
								selectionTypeLbl.Text = "";
								selectAspectOverBtn.Visible = false;
								_initiativeState = InitiativeState.ACTING;
							} else {
								if (_checkingStateAsPassive != PassiveCheckingState.SELECT_ASPECT) return;
								_selectionList.Clear();
								selectionTypeLbl.Text = "";
								selectAspectOverBtn.Visible = false;
								_checkingStateAsPassive = PassiveCheckingState.IDLE;
							}
						}
						if (completeMsg.failure) {
							printer.WriteLine("失败" + completeMsg.extraMessage);
						}
					}
					break;
				default:
					return;
			}
		}

		public BattleSceneForm() {
			InitializeComponent();

			roundInfoListBox.DataSource = _actingOrder;
			selectionListBox.DataSource = _selectionList;

			Program.connection.AddMessageReceiver(WaitingForUserDeterminMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(ShowSceneMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(DisplayDicePointsMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneResetMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneUpdateGridDataMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleScenePushGridObjectMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneRemoveGridObjectMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneAddLadderObjectMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneRemoveLadderObjectMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneStartBattleMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneUpdateTurnOrderMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneNewTurnMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneUpdateActionPointMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneUpdateMovePointMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneDisplayActableObjectMovingMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneDisplayTakeExtraMovePointMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneDisplayUsingStuntMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneStartCheckMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(CheckerCheckResultMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneCheckNextoneMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneEndCheckMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneCheckerDisplaySkillReadyMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneCheckerDisplayUsingAspectMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneCheckerUpdateSumPointMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(CheckerSelectSkillOrStuntCompleteMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(BattleSceneCheckerNotifySelectAspectMessage.MESSAGE_TYPE, this);
			Program.connection.AddMessageReceiver(CheckerSelectAspectCompleteMessage.MESSAGE_TYPE, this);
		}

		private BattleSceneObject GetSelectedObject() {
			if (_specifiedSelectedObject == null) {
				if (_selectedRow != -1 && _selectedCol != -1) {
					Grid grid = _grids[_selectedRow, _selectedCol];
					List<GridObject> land;
					if (_selectedHighland) land = grid.highland;
					else land = grid.lowland;
					if (land.Count > 0) {
						GridObject gridObject = land[land.Count - 1];
						return gridObject;
					}
				}
				return null;
			} else {
				return _specifiedSelectedObject;
			}
		}

		private GridObject GetGridObject(GameUtil.Network.Streamable.BattleSceneObject messageObj, out bool highland) {
			return this.GetGridObject(messageObj.row, messageObj.col, messageObj.id, out highland);
		}

		private GridObject GetGridObject(int row, int col, string id, out bool highland) {
			Grid grid = _grids[row, col];
			for (int i = grid.lowland.Count - 1; i >= 0; --i) {
				if (grid.lowland[i].id == id) {
					highland = false;
					return grid.lowland[i];
				}
			}
			for (int i = grid.highland.Count - 1; i >= 0; --i) {
				if (grid.highland[i].id == id) {
					highland = true;
					return grid.highland[i];
				}
			}
			highland = false;
			return null;
		}

		private void BattleSceneLookAtGrid(int row, int col) {
			Point pos = new Point((int)(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f)), (int)(DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f));
			int deltaWidth = battleScene.CanvasWidth - battleScene.Width;
			int deltaHeight = battleScene.CanvasHeight - battleScene.Height;
			deltaWidth = deltaWidth >= 0 ? deltaWidth : 0;
			deltaHeight = deltaHeight >= 0 ? deltaHeight : 0;
			if (pos.X < 0) pos.X = 0;
			if (pos.Y < 0) pos.Y = 0;
			if (pos.X > deltaWidth) pos.X = deltaWidth;
			if (pos.Y > deltaHeight) pos.Y = deltaHeight;
			pos.X -= battleScene.Width / 2;
			pos.X = pos.X < 0 ? 0 : pos.X;
			pos.Y -= battleScene.Height / 2;
			pos.Y = pos.Y < 0 ? 0 : pos.Y;
			battleScene.ViewerRectangleLeftTop = pos;
			this.UpdateScrollBar();
			hScrollBar.Value = pos.X;
			vScrollBar.Value = pos.Y;
		}

		private void SelectedObjectChanged(BattleSceneObject oldOne, BattleSceneObject newOne) {
			selectedGridObjectLbl.Text = newOne != null ? newOne.ToString() : "无";

			if (_checkingStateAsPassive == PassiveCheckingState.SELECT_ASPECT || _initiativeState == InitiativeState.CHECKING_SELECT_ASPECT) {
				if (newOne != null) {
					_selectionList.Clear();
					var aspectsRequest = new GetCharacterDataMessage();
					aspectsRequest.characterID = newOne.id;
					aspectsRequest.dataType = GetCharacterDataMessage.DataType.ASPECTS;
					Program.connection.Request(aspectsRequest, result => {
						var resp = result as CharacterPropertiesDescriptionMessage;
						if (resp != null) {
							foreach (var aspect in resp.properties) {
								var newItem = new CharacterPropertyInfo() {
									ownerID = resp.characterID,
									propertyID = aspect.propertyID,
									name = aspect.describable.name,
									description = aspect.describable.description
								};
								_selectionList.Add(newItem);
							}
						}
					});
					aspectsRequest.dataType = GetCharacterDataMessage.DataType.CONSEQUENCES;
					Program.connection.Request(aspectsRequest, result => {
						var resp = result as CharacterPropertiesDescriptionMessage;
						if (resp != null) {
							foreach (var consequence in resp.properties) {
								var newItem = new CharacterPropertyInfo() {
									ownerID = resp.characterID,
									propertyID = consequence.propertyID,
									name = consequence.describable.name,
									description = consequence.describable.description,
									extraMessage = "伤痕"
								};
								_selectionList.Add(newItem);
							}
						}
					});
				}
			}
		}

		private void SelectedGridChanged(int oldRow, int oldCol, bool oldHighland, int newRow, int newCol, bool newHighland) {
			var oldSelectedObject = GetSelectedObject();
			_specifiedSelectedObject = null;
			var gridObject = GetSelectedObject();
			SelectedObjectChanged(oldSelectedObject, gridObject);
			mouseGridPosLbl.Text = "Row:" + newRow.ToString() + ", Col:" + newCol.ToString() + ", Highland:" + newHighland.ToString();

			if (_movePathInfo != null) {
				_selectedMoveDst = null;
				foreach (var place in _movePathInfo) {
					if (place.row == newRow && place.col == newCol && place.highland == newHighland) {
						_selectedMoveDst = place;
						break;
					}
				}
			}

			if (_isUsingSkillOrStunt && !_isSelectingTarget) {
				_isSelectingAffectCenter = false;
				foreach (var area in _affectableAreas) {
					if (area.Key.row == newRow && area.Key.col == newCol && area.Key.highland == newHighland) {
						_selectedAffectCenter = new GridPos() { row = newRow, col = newCol, highland = newHighland };
						_isSelectingAffectCenter = true;
						break;
					}
				}
			}
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e) {
			battleScene.ViewerRectangleTop = vScrollBar.Value;
		}

		private void hScrollBar_ValueChanged(object sender, EventArgs e) {
			battleScene.ViewerRectangleLeft = hScrollBar.Value;
		}

		private void BattleSceneForm_Load(object sender, EventArgs e) {
			this.UpdateScrollBar();
		}

		private void UpdateScrollBar() {
			int deltaWidth = battleScene.CanvasWidth - battleScene.Width;
			int deltaHeight = battleScene.CanvasHeight - battleScene.Height;
			hScrollBar.Maximum = deltaWidth >= 0 ? deltaWidth : 0;
			vScrollBar.Maximum = deltaHeight >= 0 ? deltaHeight : 0;
		}

		private void InitGrids(int rows, int cols) {
			_rows = rows; _cols = cols;
			_grids = new Grid[rows, cols];
			for (int i = 0; i < rows; ++i) {
				for (int j = 0; j < cols; ++j) {
					_grids[i, j] = new Grid(i, j);
				}
			}
			battleScene.InitCanvas(new Size((int)(DIAMOND_LENGTH / 2.0f * Math.Sqrt(3.0f) * (rows + cols)), (int)(DIAMOND_LENGTH / 2.0f + DIAMOND_LENGTH / 2.0f * (rows + cols))));
		}

		private void MoveGridObjectStack(int srcRow, int srcCol, bool srcHighland, int dstRow, int dstCol, bool dstHighland, int count = 1) {
			Grid srcGrid = _grids[srcRow, srcCol];
			Grid dstGrid = _grids[dstRow, dstCol];
			List<GridObject> srcLand, dstLand;
			if (srcHighland) srcLand = srcGrid.highland;
			else srcLand = srcGrid.lowland;
			if (dstHighland) dstLand = dstGrid.highland;
			else dstLand = dstGrid.lowland;
			int dstInsertIndex = dstLand.Count;
			for (int i = 0; i < count; ++i) {
				int srcLastIndex = srcLand.Count - 1;
				if (srcLastIndex < 0) break;
				GridObject gridObject = srcLand[srcLastIndex];
				dstLand.Insert(dstInsertIndex, gridObject);
				srcLand.RemoveAt(srcLastIndex);
			}
		}

		private static bool IsPointInPolygon(PointF[] polygon, PointF point) {
			bool isInside = false;
			for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++) {
				if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
					(point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)) {
					isInside = !isInside;
				}
			}
			return isInside;
		}

		private void BattleSceneObjectPresent(GridObject gridObject, Graphics g, PointF[] gridPoints) {
			if (gridObject.view.battle == "地面") {
				g.FillPolygon(Brushes.LightGray, gridPoints);
			} else {
				var text = gridObject.ToString();
				var font = new Font("宋体", 12);
				var loc = new PointF(gridPoints[0].X - 12, gridPoints[0].Y + 12);
				if (gridObject.view.battle == "石柱") {
					g.DrawString(text, font, Brushes.Black, loc);
				} else {
					using (var path = GetStringPath(g, text, font, loc)) {
						g.DrawPath(new Pen(gridObject.color, 1), path);
					}
					g.DrawString(text, font, Brushes.White, loc);
				}
			}
		}

		private GraphicsPath GetStringPath(Graphics g, string s, Font font, PointF location) {
			var path = new GraphicsPath();
			float emSize = g.DpiY * font.SizeInPoints / 72.0f;
			path.AddString(s, font.FontFamily, (int)font.Style, emSize, location, new StringFormat());
			return path;
		}

		#region Canvas Drawing
		private void battleScene_CanvasDrawing(object sender, CanvasDrawingEventArgs e) {
			Graphics g = e.g;
			g.Clear(Color.White);
			if (_grids != null) {
				for (int row = 0; row < _rows; ++row) {
					for (int col = 0; col < _cols; ++col) {
						Grid grid = _grids[row, col];
						if (grid.lowland.Count > 0) {
							PointF[] gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
							foreach (GridObject gridObject in grid.lowland) {
								this.BattleSceneObjectPresent(gridObject, g, gridPoints);
							}
							g.DrawPolygon(Pens.Black, gridPoints);
						}
						if (grid.highland.Count > 0) {
							PointF[] lowlandGridPoints = new PointF[4]
								{
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
								};
							PointF[] gridPoints;
							if (grid.isMiddleLand) {
								gridPoints = new PointF[4]
								{
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
								};
							} else {
								gridPoints = new PointF[4]
								{
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
								};
							}
							for (int k = 0; k < 4; ++k) {
								g.DrawLine(Pens.Cyan, gridPoints[k], lowlandGridPoints[k]);
							}
							foreach (GridObject gridObject in grid.highland) {
								this.BattleSceneObjectPresent(gridObject, g, gridPoints);
							}
							g.DrawPolygon(Pens.Cyan, gridPoints);
						}
					}
				}
			}
			if (_isUsingSkillOrStunt && !_isSelectingTarget) {
				var transpRed = new SolidBrush(Color.FromArgb(80, 255, 0, 0));
				foreach (var area in _affectableAreas) {
					var areaGrid = area.Key;
					int row = areaGrid.row;
					int col = areaGrid.col;
					Grid grid = _grids[row, col];
					if (areaGrid.highland) {
						PointF[] gridPoints;
						if (grid.isMiddleLand) {
							gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
						} else {
							gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
						}
						g.FillPolygon(transpRed, gridPoints);
					} else {
						PointF[] gridPoints = new PointF[4] {
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
						};
						g.FillPolygon(transpRed, gridPoints);
					}
				}
			}
			if (_isUsingSkillOrStunt && _isSelectingAffectCenter) {
				var transpYellow = new SolidBrush(Color.FromArgb(80, 255, 255, 0));
				foreach (var areaGrid in _affectableAreas[_selectedAffectCenter]) {
					int row = areaGrid.row;
					int col = areaGrid.col;
					Grid grid = _grids[row, col];
					if (areaGrid.highland) {
						PointF[] gridPoints;
						if (grid.isMiddleLand) {
							gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
						} else {
							gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
						}
						g.FillPolygon(transpYellow, gridPoints);
					} else {
						PointF[] gridPoints = new PointF[4] {
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
						};
						g.FillPolygon(transpYellow, gridPoints);
					}
				}
			}
			if (_movePathInfo != null) {
				foreach (var path in _movePathInfo) {
					int row = path.row;
					int col = path.col;
					var b = new SolidBrush(Color.FromArgb(80, 0, 0, 255));
					if (!path.highland) {
						PointF[] gridPoints = new PointF[4]{
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
						};
						g.FillPolygon(b, gridPoints);
					} else {
						PointF[] gridPoints;
						if (_grids[row, col].isMiddleLand) {
							gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
						} else {
							gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
						}
						g.FillPolygon(b, gridPoints);
					}
				}
				ReachablePlace pathEnd = _selectedMoveDst;
				if (_selectedMoveDst != null) {
					do {
						int row = pathEnd.row;
						int col = pathEnd.col;
						var b = new SolidBrush(Color.FromArgb(200, 0, 255, 0));
						if (!pathEnd.highland) {
							PointF[] gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
							g.FillPolygon(b, gridPoints);
						} else {
							PointF[] gridPoints;
							if (_grids[row, col].isMiddleLand) {
								gridPoints = new PointF[4] {
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
								};
							} else {
								gridPoints = new PointF[4] {
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
								};
							}
							g.FillPolygon(b, gridPoints);
						}
						pathEnd = pathEnd.prevPlace;
					} while (pathEnd != null);
				}
			}
			if (_targetPlaces != null) {
				foreach (var place in _targetPlaces) {
					int row = place.row;
					int col = place.col;
					var b = new SolidBrush(Color.FromArgb(80, 255, 0, 0));
					if (!place.highland) {
						PointF[] gridPoints = new PointF[4]{
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
						};
						g.FillPolygon(b, gridPoints);
					} else {
						PointF[] gridPoints;
						if (_grids[row, col].isMiddleLand) {
							gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
						} else {
							gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
						}
						g.FillPolygon(b, gridPoints);
					}
				}
			}
			if (_currentCheckPlace != null) {
				var b = new SolidBrush(Color.FromArgb(80, 255, 255, 0));
				if (!_currentCheckPlace.highland) {
					PointF[] gridPoints = new PointF[4]{
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (_currentCheckPlace.row + _currentCheckPlace.col) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (_currentCheckPlace.row + _currentCheckPlace.col + 1) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (_currentCheckPlace.row + _currentCheckPlace.col + 2) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (_currentCheckPlace.row + _currentCheckPlace.col + 1) * DIAMOND_LENGTH / 2.0f)
						};
					g.FillPolygon(b, gridPoints);
				} else {
					PointF[] gridPoints;
					if (_grids[_currentCheckPlace.row, _currentCheckPlace.col].isMiddleLand) {
						gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (_currentCheckPlace.row + _currentCheckPlace.col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (_currentCheckPlace.row + _currentCheckPlace.col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (_currentCheckPlace.row + _currentCheckPlace.col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (_currentCheckPlace.row + _currentCheckPlace.col + 1) * DIAMOND_LENGTH / 2.0f)
							};
					} else {
						gridPoints = new PointF[4] {
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (_currentCheckPlace.row + _currentCheckPlace.col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (_currentCheckPlace.row + _currentCheckPlace.col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (_currentCheckPlace.row + _currentCheckPlace.col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (_currentCheckPlace.col - _currentCheckPlace.row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (_currentCheckPlace.row + _currentCheckPlace.col + 1) * DIAMOND_LENGTH / 2.0f)
							};
					}
					g.FillPolygon(b, gridPoints);
				}
			}
			if (_selectedRow != -1 && _selectedCol != -1) {
				int row = _selectedRow, col = _selectedCol;
				Grid grid = _grids[row, col];
				PointF[] diamondPoints;
				if (_selectedHighland) {
					if (grid.isMiddleLand) {
						diamondPoints = new PointF[4]
						{
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
						};
					} else {
						diamondPoints = new PointF[4]
						{
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
						};
					}
				} else {
					diamondPoints = new PointF[4]
					{
						new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
						new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
						new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
						new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
					};
				}
				Pen pen;
				if (_specifiedSelectedObject != null) {
					pen = new Pen(Brushes.Green, 3);
				} else {
					pen = new Pen(Brushes.Blue, 3);
				}
				g.DrawPolygon(pen, diamondPoints);
			}
		}
		#endregion

		#region Canvas Mouse Down
		private void battleScene_CanvasMouseDown(object sender, MouseEventArgs e) {
			if (_grids != null) {
				for (int row = 0; row < _rows; ++row) {
					for (int col = 0; col < _cols; ++col) {
						Grid grid = _grids[row, col];
						PointF[] diamondPoints;
						if (grid.highland.Count > 0) {
							if (grid.isMiddleLand) {
								diamondPoints = new PointF[4]
								{
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
								};
							} else {
								diamondPoints = new PointF[4]
								{
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
									new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
								};
							}
							if (IsPointInPolygon(diamondPoints, new PointF(e.X, e.Y))) {
								if (!(_selectedRow == row && _selectedCol == col && _selectedHighland == true)) {
									int oldRow = _selectedRow, oldCol = _selectedCol;
									bool oldHighland = _selectedHighland;
									_selectedRow = row; _selectedCol = col;
									_selectedHighland = true;
									SelectedGridChanged(oldRow, oldCol, oldHighland, _selectedRow, _selectedCol, _selectedHighland);
								}
								return;
							}
						}
						if (grid.lowland.Count > 0) {
							diamondPoints = new PointF[4]
							{
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
							if (IsPointInPolygon(diamondPoints, new PointF(e.X, e.Y))) {
								if (!(_selectedRow == row && _selectedCol == col && _selectedHighland == false)) {
									int oldRow = _selectedRow, oldCol = _selectedCol;
									bool oldHighland = _selectedHighland;
									_selectedRow = row; _selectedCol = col;
									_selectedHighland = false;
									SelectedGridChanged(oldRow, oldCol, oldHighland, _selectedRow, _selectedCol, _selectedHighland);
								}
								return;
							}
						}
					}
				}
			}
			if (_selectedRow != -1 || _selectedCol != -1) {
				int oldRow = _selectedRow, oldCol = _selectedCol;
				bool oldHighland = _selectedHighland;
				_selectedRow = -1; _selectedCol = -1;
				_selectedHighland = false;
				SelectedGridChanged(oldRow, oldCol, oldHighland, _selectedRow, _selectedCol, _selectedHighland);
			}
		}
		#endregion

		private void battleScene_SizeChanged(object sender, EventArgs e) {
			this.UpdateScrollBar();
		}

		private void battleSceneMenuStrip_Opening(object sender, CancelEventArgs e) {
			BattleSceneObject sceneObject = this.GetSelectedObject();
			if (sceneObject == null) {
				e.Cancel = true;
				return;
			}
			menuItemConfirmGrid.Enabled = false;
			if (!_waitingForUserDetermin && _isUsingSkillOrStunt && _isSelectingAffectCenter) {
				menuItemConfirmGrid.Enabled = true;
			}
			menuItemMove.Enabled = menuItemExtraMovePoint.Enabled = menuItemCreateAspect.Enabled = menuItemAttack.Enabled = menuItemInteract.Enabled = menuItemRoundOver.Enabled = false;
			if (!_isChecking && !_waitingForUserDetermin) {
				if (_movePathInfo != null) {
					menuItemMove.Enabled = menuItemConfirmGrid.Enabled = true;
				} else if (_createAspectReady) {
					menuItemCreateAspect.Enabled = true;
				} else if (_attackReady) {
					menuItemAttack.Enabled = true;
				} else if (_interactReady) {
					menuItemInteract.Enabled = true;
				} else if (_canActing && sceneObject.id == _actingObjID && sceneObject.Actable) {
					menuItemCreateAspect.Enabled = menuItemAttack.Enabled = menuItemInteract.Enabled = menuItemRoundOver.Enabled = true;
					if (sceneObject.Movable) menuItemMove.Enabled = menuItemExtraMovePoint.Enabled = true;
				}
			}
		}

		private void battleScene_MouseDoubleClick(object sender, MouseEventArgs e) {
			if (_selectedRow != -1 && _selectedCol != -1) {
				Grid grid = _grids[_selectedRow, _selectedCol];
				gridObjectSelectionListBox.DataSource = grid.Selections(_selectedHighland);
				gridObjectSelectionListBox.Location = new Point(e.X + battleScene.Left, e.Y + battleScene.Top);
				gridObjectSelectionListBox.Visible = true;
			}
		}

		private void battleScene_MouseClick(object sender, MouseEventArgs e) {
			gridObjectSelectionListBox.Visible = false;
		}

		private void gridObjectSelectionList_MouseDoubleClick(object sender, MouseEventArgs e) {
			var oldSelectedObject = GetSelectedObject();
			_specifiedSelectedObject = (BattleSceneObject)gridObjectSelectionListBox.SelectedValue;
			gridObjectSelectionListBox.Visible = false;
			SelectedObjectChanged(oldSelectedObject, GetSelectedObject());
		}

		private void menuItemMove_Click(object sender, EventArgs e) {
			if (_movePathInfo != null) {
				_movePathInfo = null;
				_selectedMoveDst = null;
				menuItemMove.Text = "移动";
			} else {
				var request = new BattleSceneGetMovePathInfoMessage();
				Program.connection.Request(request, result => {
					var resp = result as BattleSceneMovePathInfoMessage;
					if (resp != null) {
						_movePathInfo = new ReachablePlace[resp.pathInfo.Length];
						for (int i = 0; i < resp.pathInfo.Length; ++i) {
							_movePathInfo[i] = new ReachablePlace();
							_movePathInfo[i].row = resp.pathInfo[i].pos.row;
							_movePathInfo[i].col = resp.pathInfo[i].pos.col;
							_movePathInfo[i].highland = resp.pathInfo[i].pos.highland;
							_movePathInfo[i].leftMovePoint = resp.pathInfo[i].leftMovePoint;
						}
						for (int i = 0; i < resp.pathInfo.Length; ++i) {
							int prevIndex = resp.pathInfo[i].prevPlaceIndex;
							_movePathInfo[i].prevPlace = prevIndex == -1 ? null : _movePathInfo[prevIndex];
						}
						menuItemMove.Text = "取消移动";
					}
				});
			}
		}

		private void menuItemExtraMovePoint_Click(object sender, EventArgs e) {
			var request = new BattleSceneGetCanExtraMoveMessage();
			Program.connection.Request(request, result => {
				var resp = result as BattleSceneCanTakeExtraMoveMessage;
				if (resp != null) {
					if (resp.result) {
						var message = new BattleSceneTakeExtraMovePointMessage();
						Program.connection.SendMessage(message);
					} else {
						Program.mainForm.AdvancedConsole.TextBoxPrinter.WriteLine("行动点不足");
					}
				}
			});
		}

		private void menuItemCreateAspect_Click(object sender, EventArgs e) {
			if (_createAspectReady) {
				_createAspectReady = false;
				_isUsingSkillOrStunt = false;
				menuItemCreateAspect.Text = "创造优势";
				_selectionList.Clear();
				_selectionList2.Clear();
				selectionTypeGroupPanel.Visible = false;
			} else {
				_createAspectReady = true;
				menuItemCreateAspect.Text = "取消创造优势";
				_selectionList.Clear();
				_selectionList2.Clear();
				selectionTypeGroupPanel.Visible = true;
				skillRbn.Checked = true;
				stuntRbn.Checked = false;
				selectionTypeLbl.Text = "技能选择";
				selectionListBox.DataSource = _selectionList;
				var request = new BattleSceneGetInitiativeUsableSkillOrStuntListMessage();
				request.action = CharacterAction.CREATE_ASPECT;
				request.stunt = false;
				Program.connection.Request(request, result => {
					var resp = result as BattleSceneObjectUsableSkillListMessage;
					if (resp != null) {
						foreach (var skillType in resp.skillTypes) {
							_selectionList.Add(new CharacterPropertyInfo() {
								name = skillType.name,
								propertyID = skillType.id
							});
						}
					}
				});
				request.stunt = true;
				Program.connection.Request(request, result => {
					var resp = result as BattleSceneObjectUsableStuntListMessage;
					if (resp != null) {
						foreach (var stunt in resp.stunts) {
							_selectionList2.Add(new CharacterPropertyInfo() {
								name = stunt.describable.name,
								description = stunt.describable.description,
								propertyID = stunt.propertyID
							});
						}
					}
				});
			}
		}

		private void menuItemAttack_Click(object sender, EventArgs e) {
			if (_attackReady) {
				_attackReady = false;
				_isUsingSkillOrStunt = false;
				menuItemAttack.Text = "攻击";
				_selectionList.Clear();
				_selectionList2.Clear();
				selectionTypeGroupPanel.Visible = false;
			} else {
				_attackReady = true;
				menuItemAttack.Text = "取消攻击";
				_selectionList.Clear();
				_selectionList2.Clear();
				selectionTypeGroupPanel.Visible = true;
				skillRbn.Checked = true;
				stuntRbn.Checked = false;
				selectionTypeLbl.Text = "技能选择";
				selectionListBox.DataSource = _selectionList;
				var request = new BattleSceneGetInitiativeUsableSkillOrStuntListMessage();
				request.action = CharacterAction.ATTACK;
				request.stunt = false;
				Program.connection.Request(request, result => {
					var resp = result as BattleSceneObjectUsableSkillListMessage;
					if (resp != null) {
						foreach (var skillType in resp.skillTypes) {
							_selectionList.Add(new CharacterPropertyInfo() {
								name = skillType.name,
								propertyID = skillType.id
							});
						}
					}
				});
				request.stunt = true;
				Program.connection.Request(request, result => {
					var resp = result as BattleSceneObjectUsableStuntListMessage;
					if (resp != null) {
						foreach (var stunt in resp.stunts) {
							_selectionList2.Add(new CharacterPropertyInfo() {
								name = stunt.describable.name,
								description = stunt.describable.description,
								propertyID = stunt.propertyID
							});
						}
					}
				});
			}
		}

		private void menuItemInteract_Click(object sender, EventArgs e) {
			if (_interactReady) {
				_interactReady = false;
				_isUsingSkillOrStunt = false;
				menuItemInteract.Text = "其他动作";
				_selectionList.Clear();
				_selectionList2.Clear();
				selectionTypeGroupPanel.Visible = false;
			} else {
				_interactReady = true;
				menuItemInteract.Text = "取消其他动作";
				_selectionList.Clear();
				_selectionList2.Clear();
				selectionTypeGroupPanel.Visible = true;
				skillRbn.Checked = true;
				stuntRbn.Checked = false;
				selectionTypeLbl.Text = "技能选择";
				selectionListBox.DataSource = _selectionList;
				var request = new BattleSceneGetInitiativeUsableSkillOrStuntListOnInteractMessage();
				request.stunt = false;
				Program.connection.Request(request, result => {
					var resp = result as BattleSceneObjectUsableSkillListOnInteractMessage;
					if (resp != null) {
						foreach (var skillType in resp.skillTypes) {
							_selectionList.Add(new CharacterPropertyInfo() {
								name = skillType.name,
								propertyID = skillType.id
							});
						}
					}
				});
				request.stunt = true;
				Program.connection.Request(request, result => {
					var resp = result as BattleSceneObjectUsableStuntListOnInteractMessage;
					if (resp != null) {
						foreach (var stunt in resp.stunts) {
							_selectionList2.Add(new CharacterPropertyInfo() {
								name = stunt.describable.name,
								description = stunt.describable.description,
								propertyID = stunt.propertyID
							});
						}
					}
				});
			}
		}

		private void menuItemRoundOver_Click(object sender, EventArgs e) {
			var message = new BattleSceneTurnOverMessage();
			Program.connection.SendMessage(message);
		}

		private void menuItemConfirmGrid_Click(object sender, EventArgs e) {
			if (_movePathInfo != null && _selectedMoveDst != null) {
				var message = new BattleSceneActableObjectMoveMessage();
				message.dst.row = _selectedMoveDst.row;
				message.dst.col = _selectedMoveDst.col;
				message.dst.highland = _selectedMoveDst.highland;
				Program.connection.SendMessage(message);
				_movePathInfo = null;
				_selectedMoveDst = null;
				menuItemMove.Text = "移动";
			}
			if (_isUsingSkillOrStunt && _isSelectingAffectCenter) {
				if (_isSelectingTarget) {
					_isSelectingTarget = false;
					_targets.Clear();
					selectionTypeGroupPanel.Visible = true;
					menuItemConfirmGrid.Text = "确认位置";
					if (skillRbn.Checked) {
						selectionTypeLbl.Text = "技能选择";
						selectionListBox.DataSource = _selectionList;
					} else if (stuntRbn.Checked) {
						selectionTypeLbl.Text = "特技选择";
						selectionListBox.DataSource = _selectionList2;
					}
					confirmTargetBtn.Visible = false;
					targetCountLbl.Visible = false;
				} else {
					var request = new BattleSceneGetActionTargetCountMessage();
					request.isStunt = _isStunt;
					request.skillTypeOrStuntID = _usingSkillOrStuntID;
					Program.connection.Request(request, result => {
						var resp = result as BattleSceneActionTargetCountMessage;
						if (resp != null) {
							_targetMaxCount = resp.count;
							_isSelectingTarget = true;
							selectionTypeGroupPanel.Visible = false;
							menuItemConfirmGrid.Text = "取消确认位置";
							selectionTypeLbl.Text = "目标列表";
							selectionListBox.DataSource = _targets;
							confirmTargetBtn.Visible = true;
							targetCountLbl.Visible = true;
							targetCountLbl.Text = "(0/" + _targetMaxCount + ")";
						}
					});

				}
			}
		}

		private void menuItemViewData_Click(object sender, EventArgs e) {
			var selectedObj = GetSelectedObject();
			if (selectedObj == null) return;
			CharacterInfo infoFrm = new CharacterInfo();
			infoFrm.RequestData(selectedObj.id);
			infoFrm.Visible = true;
			infoFrm.Activate();
		}

		private void skipAspectSelectionCbx_CheckedChanged(object sender, EventArgs e) {
			var message = new BattleSceneSetSkipSelectAspectMessage();
			message.val = skipAspectSelectionCbx.Checked;
			Program.connection.SendMessage(message);
		}

		private void confirmSelectionBtn_Click(object sender, EventArgs e) {
			if (_checkingStateAsPassive == PassiveCheckingState.SELECT_SKILL_OR_STUNT) {
				if (skillRbn.Checked) {
					var message = new CheckerSkillSelectedMessage();
					message.skillTypeID = ((CharacterPropertyInfo)selectionListBox.SelectedValue).propertyID;
					Program.connection.SendMessage(message);
				} else if (stuntRbn.Checked) {
					var message = new CheckerStuntSelectedMessage();
					message.stuntID = ((CharacterPropertyInfo)selectionListBox.SelectedValue).propertyID;
					Program.connection.SendMessage(message);
				}
			} else if ((_canActing && _initiativeState == InitiativeState.CHECKING_SELECT_ASPECT) || _checkingStateAsPassive == PassiveCheckingState.SELECT_ASPECT) {
				DialogResult dr = MessageBox.Show("+2（是）还是重投（否）？", "选项", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				var selectedAspect = (CharacterPropertyInfo)selectionListBox.SelectedValue;
				var message = new CheckerAspectSelectedMessage();
				message.characterID = selectedAspect.ownerID;
				message.aspectID = selectedAspect.propertyID;
				message.isConsequence = selectedAspect.extraMessage == "伤痕";
				message.reroll = dr == DialogResult.No;
				Program.connection.SendMessage(message);
			} else if (_canActing && (_createAspectReady || _attackReady || _interactReady)) {
				if (_isUsingSkillOrStunt && _isSelectingTarget && _isSelectingAffectCenter) {
					if (_interactReady) {
						var message = new BattleSceneActableObjectDoInteractMessage();
						message.isStunt = stuntRbn.Checked;
						message.dstCenter.row = _selectedAffectCenter.row;
						message.dstCenter.col = _selectedAffectCenter.col;
						message.skillTypeOrStuntID = _usingSkillOrStuntID;
						message.targets = new string[_targets.Count];
						for (int i = 0; i < _targets.Count; ++i) {
							message.targets[i] = _targets[i].id;
						}
						Program.connection.SendMessage(message);
					} else {
						var message = new BattleSceneActableObjectDoActionMessage();
						if (_createAspectReady) message.action = CharacterAction.CREATE_ASPECT;
						else if (_attackReady) message.action = CharacterAction.ATTACK;
						message.isStunt = stuntRbn.Checked;
						message.dstCenter.row = _selectedAffectCenter.row;
						message.dstCenter.col = _selectedAffectCenter.col;
						message.skillTypeOrStuntID = _usingSkillOrStuntID;
						message.targets = new string[_targets.Count];
						for (int i = 0; i < _targets.Count; ++i) {
							message.targets[i] = _targets[i].id;
						}
						Program.connection.SendMessage(message);
					}
				} else if (skillRbn.Checked) {
					var request = new BattleSceneGetActionAffectableAreasMessage();
					request.isStunt = false;
					request.skillTypeOrStuntID = ((CharacterPropertyInfo)selectionListBox.SelectedValue).propertyID;
					Program.connection.Request(request, result => {
						var resp = result as BattleSceneActionAffectableAreasMessage;
						if (resp != null) {
							_affectableAreas = new Dictionary<GridPos, List<GridPos>>();
							for (int i = 0; i < resp.centers.Length; ++i) {
								_affectableAreas.Add(resp.centers[i], new List<GridPos>(resp.areas[i]));
							}
							_usingSkillOrStuntID = request.skillTypeOrStuntID;
							_isUsingSkillOrStunt = true;
							_isStunt = request.isStunt;
							_isSelectingAffectCenter = false;
						}
					});
				} else if (stuntRbn.Checked) {
					var request = new BattleSceneGetActionAffectableAreasMessage();
					request.isStunt = true;
					request.skillTypeOrStuntID = ((CharacterPropertyInfo)selectionListBox.SelectedValue).propertyID;
					Program.connection.Request(request, result => {
						var resp = result as BattleSceneActionAffectableAreasMessage;
						if (resp != null) {
							_affectableAreas = new Dictionary<GridPos, List<GridPos>>();
							for (int i = 0; i < resp.centers.Length; ++i) {
								_affectableAreas.Add(resp.centers[i], new List<GridPos>(resp.areas[i]));
							}
							_usingSkillOrStuntID = request.skillTypeOrStuntID;
							_isUsingSkillOrStunt = true;
							_isStunt = request.isStunt;
							_isSelectingAffectCenter = false;
						}
					});
				}
			}
		}

		private void confirmTargetBtn_Click(object sender, EventArgs e) {
			if (_targets.Count >= _targetMaxCount) {
				MessageBox.Show("已达到最大目标数" + _targetMaxCount, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			bool isInRange = false;
			foreach (var affectableGrid in _affectableAreas[_selectedAffectCenter]) {
				if (affectableGrid.row == _selectedRow && affectableGrid.col == _selectedCol && affectableGrid.highland == _selectedHighland) {
					isInRange = true;
					break;
				}
			}
			if (!isInRange) return;
			var selectedObject = GetSelectedObject();
			foreach (var target in _targets) {
				if (target.id == selectedObject.id) return;
			}
			if (stuntRbn.Checked) {
				var req = new GetStuntTargetSelectableMessage();
				if (_createAspectReady) req.action = CharacterAction.CREATE_ASPECT;
				else if (_attackReady) req.action = CharacterAction.ATTACK;
				else if (_interactReady) req.isInteract = true;
				req.stuntID = _usingSkillOrStuntID;
				req.targetID = selectedObject.id;
				Program.connection.Request(req, result => {
					var resp = result as StuntTargetSelectableMessage;
					if (resp != null) {
						if (resp.result) {
							_targets.Add(selectedObject);
							targetCountLbl.Text = "(" + _targets.Count + "/" + _targetMaxCount + ")";
						} else {
							Program.mainForm.AdvancedConsole.TextBoxPrinter.WriteLine("这个角色不可作为目标");
						}
					}
				});
			} else {
				_targets.Add(selectedObject);
				targetCountLbl.Text = "(" + _targets.Count + "/" + _targetMaxCount + ")";
			}
		}

		private void stuntRbn_CheckedChanged(object sender, EventArgs e) {
			if (stuntRbn.Checked) {
				selectionTypeLbl.Text = "特技选择";
				selectionListBox.DataSource = _selectionList2;
			}
		}

		private void skillRbn_CheckedChanged(object sender, EventArgs e) {
			if (skillRbn.Checked) {
				selectionTypeLbl.Text = "技能选择";
				selectionListBox.DataSource = _selectionList;
			}
		}

		private void selectAspectOverBtn_Click(object sender, EventArgs e) {
			if ((_canActing && _initiativeState == InitiativeState.CHECKING_SELECT_ASPECT) || _checkingStateAsPassive == PassiveCheckingState.SELECT_ASPECT) {
				var message = new SelectAspectOverMessage();
				Program.connection.SendMessage(message);
			}
		}

		private void BattleSceneForm_FormClosing(object sender, FormClosingEventArgs e) {
			e.Cancel = true;
		}
	}
}
