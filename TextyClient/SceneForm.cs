using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextyClient {
	public partial class SceneForm : Form, IMessageReceiver {
		public enum Direction {
			POSITIVE_ROW = 1,
			POSITIVE_COL = 2,
			NEGATIVE_ROW = 4,
			NEGATIVE_COL = 8
		}

		private class GridObject {
			public readonly string id;
			public readonly CharacterView view;
			public Direction direction;
			public bool actable;
			public bool movable;

			public GridObject(string id, CharacterView view) {
				this.id = id;
				this.view = view;
			}

			public override string ToString() {
				return view.battle;
			}
		}

		private class SideObject {
			public readonly string id;
			public readonly CharacterView view;

			public SideObject(string id, CharacterView view) {
				this.id = id;
				this.view = view;
			}

			public override string ToString() {
				return view.battle;
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
				return currentActing ? "→" : "" + actable.ToString();
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

		private int _mouseRow = -1;
		private int _mouseCol = -1;
		private bool _mouseHighland = false;
		private GridObject _specifiedGridObject = null;

		private List<ActableObjWithAP> _actingOrder = new List<ActableObjWithAP>();
		private string _actingObjID = null;
		private bool _canActing = false;
		private InitiativeState _initiativeState = InitiativeState.ACTING;
		private PassiveCheckingState _checkingStateAsPassive = PassiveCheckingState.IDLE;
		private List<CharacterPropertyInfo> _selectionList = new List<CharacterPropertyInfo>();
		private List<CharacterPropertyInfo> _selectionList2 = new List<CharacterPropertyInfo>();

		private ReachablePlace[] _movePathInfo = null;
		private ReachablePlace _selectedMoveDst = null;

		public void MessageReceived(GameLogic.Core.Network.Message message) {
			switch (message.MessageType) {
				case BattleSceneResetMessage.MESSAGE_TYPE: {
						BattleSceneResetMessage resetMessage = (BattleSceneResetMessage)message;
						this.InitGrids(resetMessage.rows, resetMessage.cols);
						_mouseRow = -1;
						_mouseCol = -1;
						_mouseHighland = false;
					}
					break;
				case BattleScenePushGridObjectMessage.MESSAGE_TYPE: {
						BattleScenePushGridObjectMessage pushGridObjMessage = (BattleScenePushGridObjectMessage)message;
						Grid grid = _grids[pushGridObjMessage.objData.obj.row, pushGridObjMessage.objData.obj.col];
						GridObject gridObject = new GridObject(pushGridObjMessage.objData.obj.id, pushGridObjMessage.view);
						gridObject.direction = (Direction)pushGridObjMessage.objData.direction;
						bool actable = gridObject.actable = pushGridObjMessage.objData.actable;
						if (actable) {
							gridObject.movable = pushGridObjMessage.objData.actableObjData.movable;
						} else {
							gridObject.movable = false;
						}
						if (pushGridObjMessage.objData.highland) {
							grid.highland.Add(gridObject);
						} else {
							grid.lowland.Add(gridObject);
						}
					}
					break;
				case BattleSceneRemoveGridObjectMessage.MESSAGE_TYPE: {
						BattleSceneRemoveGridObjectMessage removeGridObjMessage = (BattleSceneRemoveGridObjectMessage)message;
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
							case (int)Direction.POSITIVE_ROW:
								grid.positiveRowLadder = sideObject;
								_grids[row + 1, col].negativeRowLadder = sideObject;
								break;
							case (int)Direction.POSITIVE_COL:
								grid.positiveRowLadder = sideObject;
								_grids[row, col + 1].negativeColLadder = sideObject;
								break;
							case (int)Direction.NEGATIVE_ROW:
								grid.positiveRowLadder = sideObject;
								_grids[row - 1, col].positiveRowLadder = sideObject;
								break;
							case (int)Direction.NEGATIVE_COL:
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
				case BattleSceneSetActingOrderMessage.MESSAGE_TYPE: {
						var orderMessage = (BattleSceneSetActingOrderMessage)message;
						_actingOrder.Clear();
						foreach (var obj in orderMessage.objOrder) {
							var gridObject = this.GetGridObject(obj.row, obj.col, obj.id);
							var objWithAP = new ActableObjWithAP { actable = gridObject };
							_actingOrder.Add(objWithAP);
						}
					}
					break;
				case BattleSceneChangeTurnMessage.MESSAGE_TYPE: {
						var orderMessage = (BattleSceneChangeTurnMessage)message;
						_canActing = orderMessage.canOperate;
						_actingObjID = orderMessage.gridObj.id;
						if (orderMessage.canOperate) {
							roundInfoLbl.Text = "你的回合";
						} else {
							roundInfoLbl.Text = this.GetGridObject(orderMessage.gridObj) + " 的回合";
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
						this.BattleSceneLookAtGrid(orderMessage.gridObj.row, orderMessage.gridObj.col);
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
						BattleMapDirection direction = (BattleMapDirection)moveMsg.direction;
						switch (direction) {
							case BattleMapDirection.POSITIVE_ROW:
								MoveGridObjectStack(grid.row, grid.col, highland, grid.row + 1, grid.col, highland ^ moveMsg.stairway);
								break;
							case BattleMapDirection.POSITIVE_COL:
								MoveGridObjectStack(grid.row, grid.col, highland, grid.row, grid.col + 1, highland ^ moveMsg.stairway);
								break;
							case BattleMapDirection.NEGATIVE_ROW:
								MoveGridObjectStack(grid.row, grid.col, highland, grid.row - 1, grid.col, highland ^ moveMsg.stairway);
								break;
							case BattleMapDirection.NEGATIVE_COL:
								MoveGridObjectStack(grid.row, grid.col, highland, grid.row, grid.col - 1, highland ^ moveMsg.stairway);
								break;
							default:
								throw new InvalidOperationException("Invalid message.");
						}
					}
					break;
				case BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage.MESSAGE_TYPE: {
						if (_checkingStateAsPassive != PassiveCheckingState.IDLE) return;
						var selectSkillMsg = (BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage)message;
						var getDirectResistRequest = new GetDirectResistSkillsMessage {
							actionType = selectSkillMsg.action,
							initiativeSkillTypeID = selectSkillMsg.initiativeSkillType.id
						};
						var stuntsIDRequest = new GetCharacterDataMessage {
							dataType = GetCharacterDataMessage.DataType.STUNTS,
							characterID = selectSkillMsg.passiveObj.id
						};
						_selectionList.Clear();
						_selectionList2.Clear();
						selectionTypeGroupPanel.Visible = true;
						skillRbn.Checked = true;
						stuntRbn.Checked = false;
						selectionTypeLbl.Text = "对抗技能选择";
						selectionListBox.DataSource = _selectionList;
						_checkingStateAsPassive = PassiveCheckingState.SELECT_SKILL_OR_STUNT;
						Program.connection.Request(getDirectResistRequest, result => {
							var resp = result as DirectResistSkillsDataMessage;
							if (resp != null) {
								foreach (var skillType in resp.skillTypes) {
									_selectionList.Add(new CharacterPropertyInfo() {
										name = skillType.name,
										propertyID = skillType.id
									});
								}
								foreach (var skillType in Program.skillTypes) {
									bool contain = false;
									foreach (var match in resp.skillTypes) {
										if (match.id == skillType.propertyID) {
											contain = true;
											break;
										}
									}
									if (!contain) {
										_selectionList.Add(new CharacterPropertyInfo() {
											name = skillType.name,
											propertyID = skillType.propertyID,
											extraMessage = "*"
										});
									}
								}
							}
						});
						Program.connection.Request(stuntsIDRequest, result => {
							var resp = result as CharacterStuntsDescriptionMessage;
							if (resp != null) {
								foreach (var property in resp.properties) {
									var stuntDataRequest = new GetStuntDataMessage() {
										characterID = resp.characterID,
										stuntID = property.propertyID
									};
									Program.connection.Request(stuntDataRequest, dataResult => {
										var dataResp = dataResult as StuntDataMessage;
										if (dataResp != null) {
											_selectionList2.Add(new CharacterPropertyInfo() {
												name = property.describable.name,
												description = property.describable.description,
												propertyID = property.propertyID,
												extraMessage = dataResp.needDMCheck ? "*" : ""
											});
										}
									});
								}
							}
						});
					}
					break;
				case CheckerSelectSkillOrStuntCompleteMessage.MESSAGE_TYPE: {
						var completeMsg = (CheckerSelectSkillOrStuntCompleteMessage)message;
						if (completeMsg.failure) {
							MessageBox.Show(completeMsg.extraMessage, "失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

								}
							}
						}
					}
					break;
				case BattleSceneCheckerNotifySelectAspectMessage.MESSAGE_TYPE: {
						var selectAspectMsg = (BattleSceneCheckerNotifySelectAspectMessage)message;
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
					}
					break;
				case CheckerSelectAspectCompleteMessage.MESSAGE_TYPE: {
						var completeMsg = (CheckerSelectAspectCompleteMessage)message;
						if (completeMsg.over) {
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
							MessageBox.Show(completeMsg.extraMessage, "失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
					}
					break;
				default:
					return;
			}
		}

		public SceneForm() {
			InitializeComponent();

			Timer connectionUpdater = new Timer();
			connectionUpdater.Interval = 10;
			connectionUpdater.Tick += new EventHandler(this.ConnectionUpdate);
			connectionUpdater.Start();

			roundInfoListBox.DataSource = _actingOrder;
			selectionListBox.DataSource = _selectionList;

			Program.connection.AddMessageReceiver(DisplayDicePointsMessage.MESSAGE_TYPE, this);
		}

		private void ConnectionUpdate(object sender, EventArgs e) {
			Program.connection.UpdateReceiver();
		}

		private GridObject GetSelectedGridObject() {
			if (_specifiedGridObject == null) {
				if (_mouseRow != -1 && _mouseCol != -1) {
					Grid grid = _grids[_mouseRow, _mouseCol];
					List<GridObject> land;
					if (_mouseHighland) land = grid.highland;
					else land = grid.lowland;
					if (land.Count > 0) {
						GridObject gridObject = land[land.Count - 1];
						return gridObject;
					}
				}
				return null;
			} else {
				return _specifiedGridObject;
			}
		}

		private GridObject GetGridObject(BattleSceneObj messageObj) {
			return this.GetGridObject(messageObj.row, messageObj.col, messageObj.id);
		}

		private GridObject GetGridObject(int row, int col, string id) {
			Grid grid = _grids[row, col];
			for (int i = grid.lowland.Count - 1; i >= 0; --i) {
				if (grid.lowland[i].id == id) return grid.lowland[i];
			}
			for (int i = grid.highland.Count - 1; i >= 0; --i) {
				if (grid.highland[i].id == id) return grid.highland[i];
			}
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
			battleScene.ViewerRectangleLeftTop = pos;
			this.UpdateScrollBar();
			hScrollBar.Value = pos.X;
			vScrollBar.Value = pos.Y;
		}

		private void SelectedObjectChanged(GridObject oldOne, GridObject newOne) {
			selectedGridObjectLbl.Text = newOne != null ? newOne.ToString() : "无";

			if (_checkingStateAsPassive == PassiveCheckingState.SELECT_ASPECT || _initiativeState == InitiativeState.CHECKING_SELECT_ASPECT) {
				if (newOne != null) {
					_selectionList.Clear();
					var aspectsRequest = new GetCharacterDataMessage();
					aspectsRequest.characterID = newOne.id;
					aspectsRequest.dataType = GetCharacterDataMessage.DataType.ASPECTS;
					Program.connection.Request(aspectsRequest, result => {
						var resp = (CharacterPropertiesDescriptionMessage)result;
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
						var resp = (CharacterPropertiesDescriptionMessage)result;
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
			var oldSelectedObject = GetSelectedGridObject();
			_specifiedGridObject = null;
			var gridObject = GetSelectedGridObject();
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
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e) {
			battleScene.ViewerRectangleTop = vScrollBar.Value;
		}

		private void hScrollBar_ValueChanged(object sender, EventArgs e) {
			battleScene.ViewerRectangleLeft = hScrollBar.Value;
		}

		private void BattleSceneForm_Load(object sender, EventArgs e) {
			this.InitGrids(24, 24);
			for (int i = 0; i < _rows; ++i) {
				for (int j = 0; j < _cols; ++j) {
					CharacterView view = new CharacterView();
					view.battle = "地面";
					_grids[i, j].lowland.Add(new GridObject("", view));
				}
			}
			CharacterView view2 = new CharacterView();
			view2.battle = "吸血鬼";
			CharacterView view3 = new CharacterView();
			view3.battle = "障碍";
			_grids[0, 0].highland.Add(new GridObject("", view3));
			_grids[0, 0].highland.Add(new GridObject("", view2));
			_grids[0, 0].lowland.Add(new GridObject("", view3));
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
			int lastIndex = dstLand.Count;
			for (int i = srcLand.Count - 1; srcLand.Count - i <= count; --i) {
				GridObject gridObject = srcLand[i];
				dstLand.Insert(lastIndex, gridObject);
				srcLand.RemoveAt(i);
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
			if (gridObject == GetSelectedGridObject()) {
				if (gridObject.view.battle == "地面") {
					g.FillPolygon(Brushes.Red, gridPoints);
				} else if (gridObject.view.battle == "障碍") {
					g.FillPolygon(Brushes.DarkRed, gridPoints);
				} else {
					g.DrawString(gridObject.view.battle, new Font("宋体", 12), Brushes.Red, new PointF(gridPoints[0].X - 12, gridPoints[0].Y + 12));
				}
			} else {
				if (gridObject.view.battle == "地面") {
					g.FillPolygon(Brushes.LightGray, gridPoints);
				} else if (gridObject.view.battle == "障碍") {
					g.FillPolygon(Brushes.DarkGray, gridPoints);
				} else {
					g.DrawString(gridObject.view.battle, new Font("宋体", 12), Brushes.Black, new PointF(gridPoints[0].X - 12, gridPoints[0].Y + 12));
				}
			}
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
							PointF[] gridPoints = new PointF[4]
							{
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
			if (_movePathInfo != null) {
				foreach (var path in _movePathInfo) {
					int row = path.row;
					int col = path.col;
					if (!path.highland) {
						PointF[] gridPoints = new PointF[4]
						{
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
								new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
						};
						g.FillPolygon(Brushes.Blue, gridPoints);
					} else {
						PointF[] gridPoints;
						if (_grids[row, col].isMiddleLand) {
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
						g.FillPolygon(Brushes.Blue, gridPoints);
					}
				}
				ReachablePlace pathEnd = _selectedMoveDst;
				if (_selectedMoveDst != null) {
					do {
						int row = pathEnd.row;
						int col = pathEnd.col;
						if (!pathEnd.highland) {
							PointF[] gridPoints = new PointF[4]
							{
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
							new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
							};
							g.FillPolygon(Brushes.Green, gridPoints);
						} else {
							PointF[] gridPoints;
							if (_grids[row, col].isMiddleLand) {
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
							g.FillPolygon(Brushes.Green, gridPoints);
						}
						pathEnd = pathEnd.prevPlace;
					} while (pathEnd != null);
				}
			}
			if (_mouseRow != -1 && _mouseCol != -1) {
				int row = _mouseRow, col = _mouseCol;
				Grid grid = _grids[row, col];
				PointF[] diamondPoints;
				if (_mouseHighland) {
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
				if (_specifiedGridObject != null) {
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
								if (!(_mouseRow == row && _mouseCol == col && _mouseHighland == true)) {
									int oldRow = _mouseRow, oldCol = _mouseCol;
									bool oldHighland = _mouseHighland;
									_mouseRow = row; _mouseCol = col;
									_mouseHighland = true;
									SelectedGridChanged(oldRow, oldCol, oldHighland, _mouseRow, _mouseCol, _mouseHighland);
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
								if (!(_mouseRow == row && _mouseCol == col && _mouseHighland == false)) {
									int oldRow = _mouseRow, oldCol = _mouseCol;
									bool oldHighland = _mouseHighland;
									_mouseRow = row; _mouseCol = col;
									_mouseHighland = false;
									SelectedGridChanged(oldRow, oldCol, oldHighland, _mouseRow, _mouseCol, _mouseHighland);
								}
								return;
							}
						}
					}
				}
			}
			if (_mouseRow != -1 || _mouseCol != -1) {
				int oldRow = _mouseRow, oldCol = _mouseCol;
				bool oldHighland = _mouseHighland;
				_mouseRow = -1; _mouseCol = -1;
				_mouseHighland = false;
				SelectedGridChanged(oldRow, oldCol, oldHighland, _mouseRow, _mouseCol, _mouseHighland);
			}
		}
		#endregion

		private void battleScene_SizeChanged(object sender, EventArgs e) {
			this.UpdateScrollBar();
		}

		private void battleSceneMenuStrip_Opening(object sender, CancelEventArgs e) {
			GridObject gridObject = this.GetSelectedGridObject();
			if (gridObject == null) {
				e.Cancel = true;
				return;
			}
			menuItemConfirmGrid.Enabled = false;
			menuItemMove.Enabled = menuItemExtraMovePoint.Enabled = menuItemCreateAspect.Enabled = menuItemAttack.Enabled = menuItemSpecialAction.Enabled = menuItemRoundOver.Enabled = false;
			if (_movePathInfo != null && _selectedMoveDst != null) {
				menuItemMove.Enabled = menuItemConfirmGrid.Enabled = true;
			} else if (_canActing && gridObject.id == _actingObjID && gridObject.actable) {
				menuItemCreateAspect.Enabled = menuItemAttack.Enabled = menuItemSpecialAction.Enabled = menuItemRoundOver.Enabled = true;
				if (gridObject.movable) menuItemMove.Enabled = menuItemExtraMovePoint.Enabled = true;
			}
		}

		private void battleScene_MouseDoubleClick(object sender, MouseEventArgs e) {
			if (_mouseRow != -1 && _mouseCol != -1) {
				Grid grid = _grids[_mouseRow, _mouseCol];
				if (_mouseHighland) gridObjectSelectionListBox.DataSource = grid.highland;
				else gridObjectSelectionListBox.DataSource = grid.lowland;
				gridObjectSelectionListBox.Location = new Point(e.X + battleScene.Left, e.Y + battleScene.Top);
				gridObjectSelectionListBox.Visible = true;
			}
		}

		private void battleScene_MouseClick(object sender, MouseEventArgs e) {
			gridObjectSelectionListBox.Visible = false;
		}

		private void gridObjectSelectionList_MouseDoubleClick(object sender, MouseEventArgs e) {
			var oldSelectedObject = GetSelectedGridObject();
			_specifiedGridObject = (GridObject)gridObjectSelectionListBox.SelectedValue;
			gridObjectSelectionListBox.Visible = false;
			SelectedObjectChanged(oldSelectedObject, GetSelectedGridObject());
		}

		private void menuItemMove_Click(object sender, EventArgs e) {
			if (_movePathInfo != null) {
				_movePathInfo = null;
				_selectedMoveDst = null;
				menuItemMove.Text = "移动";
			} else {
				var message = new BattleSceneGetActableObjectMovePathInfoMessage();
				Program.connection.Request(message, result => {
					var resp = (BattleSceneMovePathInfoMessage)result;
					if (resp != null) {
						_movePathInfo = new ReachablePlace[resp.pathInfo.Length];
						for (int i = 0; i < resp.pathInfo.Length; ++i) {
							_movePathInfo[i] = new ReachablePlace();
							_movePathInfo[i].row = resp.pathInfo[i].row;
							_movePathInfo[i].col = resp.pathInfo[i].col;
							_movePathInfo[i].highland = resp.pathInfo[i].highland;
							_movePathInfo[i].leftMovePoint = resp.pathInfo[i].leftMovePoint;
						}
						for (int i = 0; i < resp.pathInfo.Length; ++i) {
							_movePathInfo[i].prevPlace = _movePathInfo[resp.pathInfo[i].prevPlaceIndex];
						}
						menuItemMove.Text = "取消移动";
					}
				});
			}
		}

		private void menuItemExtraMovePoint_Click(object sender, EventArgs e) {
			
		}

		private void menuItemCreateAspect_Click(object sender, EventArgs e) {

		}

		private void menuItemAttack_Click(object sender, EventArgs e) {

		}

		private void menuItemSpecialAction_Click(object sender, EventArgs e) {

		}

		private void menuItemRoundOver_Click(object sender, EventArgs e) {

		}

		private void menuItemConfirmGrid_Click(object sender, EventArgs e) {
			if (_movePathInfo != null && _selectedMoveDst != null) {
				var message = new BattleSceneActableObjectMoveMessage();
				message.dstRow = _selectedMoveDst.row;
				message.dstCol = _selectedMoveDst.col;
				message.dstHighland = _selectedMoveDst.highland;
				Program.connection.SendMessage(message);
				_movePathInfo = null;
				_selectedMoveDst = null;
				menuItemMove.Text = "移动";
			}

		}

		private void menuItemViewData_Click(object sender, EventArgs e) {

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
				DialogResult dr = MessageBox.Show("+2（是）还是重投（否）？", "选项", MessageBoxButtons.YesNo);
				var selectedAspect = (CharacterPropertyInfo)selectionListBox.SelectedValue;
				var message = new CheckerAspectSelectedMessage();
				message.characterID = selectedAspect.ownerID;
				message.aspectID = selectedAspect.propertyID;
				message.reroll = dr == DialogResult.No;
				Program.connection.SendMessage(message);
			} else {
				/*
                if (skillRbn.Checked)
                {
                    var message = new SkillSelectedMessage();
                    message.skillTypeID = ((CharacterPropertyInfo)selectionListBox.SelectedValue).propertyID;
                    Program.connection.SendMessage(message);
                }
                else if (stuntRbn.Checked)
                {
                    var message = new StuntSelectedMessage();
                    message.stuntID = ((CharacterPropertyInfo)selectionListBox.SelectedValue).propertyID;
                    Program.connection.SendMessage(message);
                }
                */
			}
		}

		private void stuntRbn_CheckedChanged(object sender, EventArgs e) {
			if (stuntRbn.Checked) {
				selectionTypeLbl.Text = "对抗特技选择";
				selectionListBox.DataSource = _selectionList2;
			}
		}

		private void skillRbn_CheckedChanged(object sender, EventArgs e) {
			if (skillRbn.Checked) {
				selectionTypeLbl.Text = "对抗技能选择";
				selectionListBox.DataSource = _selectionList;
			}
		}

		private void selectAspectOverBtn_Click(object sender, EventArgs e) {
			if ((_canActing && _initiativeState == InitiativeState.CHECKING_SELECT_ASPECT) || _checkingStateAsPassive == PassiveCheckingState.SELECT_ASPECT) {
				var message = new SelectAspectOverMessage();
				Program.connection.SendMessage(message);
			}
		}
	}
}
