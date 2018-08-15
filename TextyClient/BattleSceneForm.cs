using GameLogic.Core;
using GameLogic.Core.Network;
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

namespace TextyClient
{
    public partial class BattleSceneForm : Form, IMessageReceiver
    {
        public enum Direction
        {
            POSITIVE_ROW = 1,
            POSITIVE_COL = 2,
            NEGATIVE_ROW = 4,
            NEGATIVE_COL = 8
        }

        private class GridObject
        {
            public readonly string id;
            public bool highland;
            public readonly CharacterView view;
            public Direction direction;
            public bool actable;
            public bool movable;

            public GridObject(string id, CharacterView view)
            {
                this.id = id;
                this.view = view;
            }

            public override string ToString()
            {
                return view.battle;
            }
        }

        private class SideObject
        {
            public readonly string id;
            public readonly CharacterView view;

            public SideObject(string id, CharacterView view)
            {
                this.id = id;
                this.view = view;
            }

            public override string ToString()
            {
                return view.battle;
            }
        }

        private class Grid
        {
            public readonly List<GridObject> highland;
            public readonly List<GridObject> lowland;
            public SideObject positiveRowLadder;
            public SideObject positiveColLadder;
            public SideObject negativeRowLadder;
            public SideObject negativeColLadder;
            public bool isMiddleLand;

            public Grid()
            {
                highland = new List<GridObject>();
                lowland = new List<GridObject>();
            }
        }
        
        private Grid[,] _grids = null;
        private int _rows = -1;
        private int _cols = -1;
        private const float DIAMOND_LENGTH = 64.0f;
        
        private int _mouseRow = -1;
        private int _mouseCol = -1;
        private bool _mouseHighland = false;
        private GridObject _specifiedGridObject = null;

        private string _operatableID = null;
        private List<GridObject> _actingOrder;
        private bool _inChecking = false;

        public void MessageReceived(ulong timestamp, GameLogic.Core.Network.Message message)
        {
            switch (message.MessageType)
            {
                case BattleSceneResetMessage.MESSAGE_TYPE:
                    {
                        BattleSceneResetMessage resetMessage = (BattleSceneResetMessage)message;
                        this.InitGrids(resetMessage.rows, resetMessage.cols);
                        _mouseRow = -1;
                        _mouseCol = -1;
                        _mouseHighland = false;
                    }
                    break;
                case BattleScenePushGridObjectMessage.MESSAGE_TYPE:
                    {
                        BattleScenePushGridObjectMessage pushGridObjMessage = (BattleScenePushGridObjectMessage)message;
                        Grid grid = _grids[pushGridObjMessage.gridObj.row, pushGridObjMessage.gridObj.col];
                        GridObject gridObject = new GridObject(pushGridObjMessage.gridObj.objID, pushGridObjMessage.view);
                        gridObject.direction = (Direction)pushGridObjMessage.direction;
                        gridObject.actable = pushGridObjMessage.actable;
                        gridObject.movable = pushGridObjMessage.movable;
                        if (pushGridObjMessage.highland)
                        {
                            grid.highland.Add(gridObject);
                        }
                        else
                        {
                            grid.lowland.Add(gridObject);
                        }
                    }
                    break;
                case BattleSceneRemoveGridObjectMessage.MESSAGE_TYPE:
                    {
                        BattleSceneRemoveGridObjectMessage removeGridObjMessage = (BattleSceneRemoveGridObjectMessage)message;
                        Grid grid = _grids[removeGridObjMessage.gridObj.row, removeGridObjMessage.gridObj.col];
                        for (int i = grid.lowland.Count - 1; i >= 0; --i)
                        {
                            if (grid.lowland[i].id == removeGridObjMessage.gridObj.objID)
                            {
                                grid.lowland.RemoveAt(i);
                                return;
                            }
                        }
                        for (int i = grid.highland.Count - 1; i >= 0; --i)
                        {
                            if (grid.highland[i].id == removeGridObjMessage.gridObj.objID)
                            {
                                grid.highland.RemoveAt(i);
                                return;
                            }
                        }
                    }
                    break;
                case BattleSceneAddLadderObjectMessage.MESSAGE_TYPE:
                    {
                        BattleSceneAddLadderObjectMessage addLadderObjMessage = (BattleSceneAddLadderObjectMessage)message;
                        Grid grid = _grids[addLadderObjMessage.ladderObj.row, addLadderObjMessage.ladderObj.col];
                        SideObject sideObject = new SideObject(addLadderObjMessage.ladderObj.objID, addLadderObjMessage.view);
                        switch (addLadderObjMessage.direction)
                        {
                            case (int)Direction.POSITIVE_ROW:
                                grid.positiveRowLadder = sideObject;
                                _grids[addLadderObjMessage.ladderObj.row + 1, addLadderObjMessage.ladderObj.col].negativeRowLadder = sideObject;
                                break;
                            case (int)Direction.POSITIVE_COL:
                                grid.positiveRowLadder = sideObject;
                                _grids[addLadderObjMessage.ladderObj.row, addLadderObjMessage.ladderObj.col + 1].negativeColLadder = sideObject;
                                break;
                            case (int)Direction.NEGATIVE_ROW:
                                grid.positiveRowLadder = sideObject;
                                _grids[addLadderObjMessage.ladderObj.row - 1, addLadderObjMessage.ladderObj.col].positiveRowLadder = sideObject;
                                break;
                            case (int)Direction.NEGATIVE_COL:
                                grid.positiveRowLadder = sideObject;
                                _grids[addLadderObjMessage.ladderObj.row, addLadderObjMessage.ladderObj.col - 1].positiveColLadder = sideObject;
                                break;
                            default:
                                return;
                        }
                    }
                    break;
                case BattleSceneRemoveLadderObjectMessage.MESSAGE_TYPE:
                    {
                        BattleSceneRemoveLadderObjectMessage removeLadderObjMessage = (BattleSceneRemoveLadderObjectMessage)message;
                        Grid grid = _grids[removeLadderObjMessage.ladderObj.row, removeLadderObjMessage.ladderObj.col];
                        if (grid.positiveRowLadder != null && grid.positiveRowLadder.id == removeLadderObjMessage.ladderObj.objID)
                        {
                            grid.positiveRowLadder = null;
                            _grids[removeLadderObjMessage.ladderObj.row + 1, removeLadderObjMessage.ladderObj.col].negativeRowLadder = null;
                        }
                        else if (grid.positiveColLadder != null && grid.positiveColLadder.id == removeLadderObjMessage.ladderObj.objID)
                        {
                            grid.positiveColLadder = null;
                            _grids[removeLadderObjMessage.ladderObj.row, removeLadderObjMessage.ladderObj.col + 1].negativeColLadder = null;
                        }
                        else if (grid.negativeRowLadder != null && grid.negativeRowLadder.id == removeLadderObjMessage.ladderObj.objID)
                        {
                            grid.negativeRowLadder = null;
                            _grids[removeLadderObjMessage.ladderObj.row - 1, removeLadderObjMessage.ladderObj.col].positiveRowLadder = null;
                        }
                        else if (grid.negativeColLadder != null && grid.negativeColLadder.id == removeLadderObjMessage.ladderObj.objID)
                        {
                            grid.negativeColLadder = null;
                            _grids[removeLadderObjMessage.ladderObj.row, removeLadderObjMessage.ladderObj.col - 1].positiveColLadder = null;
                        }
                    }
                    break;
                case BattleSceneSetActingOrderMessage.MESSAGE_TYPE:
                    {
                        var orderMessage = (BattleSceneSetActingOrderMessage)message;
                        _actingOrder.Clear();
                        _actingOrder.Add(null);
                        foreach (var msgActableObj in orderMessage.objsOrder)
                        {
                            GridObject gridObject = this.GetGridObject(msgActableObj.row, msgActableObj.col, msgActableObj.objID);
                            _actingOrder.Add(gridObject);
                        }
                    }
                    break;
                case BattleSceneNextTurnMessage.MESSAGE_TYPE:
                    {
                        var orderMessage = (BattleSceneNextTurnMessage)message;
                        if (orderMessage.canOperate)
                        {
                            roundInfoLbl.Text = "你的回合";
                            _operatableID = orderMessage.gridObj.objID;
                        }
                        else
                        {
                            roundInfoLbl.Text = _actingOrder[0].ToString() + " 的回合";
                            _operatableID = null;
                        }
                        if (_actingOrder.Count > 0) _actingOrder.RemoveAt(0);
                        this.BattleSceneLookAtGrid(orderMessage.gridObj.row, orderMessage.gridObj.col);
                    }
                    break;
                default:
                    return;
            }
        }

        public BattleSceneForm()
        {
            InitializeComponent();

            Timer connectionUpdater = new Timer();
            connectionUpdater.Interval = 10;
            connectionUpdater.Tick += new EventHandler(this.ConnectionUpdate);
            connectionUpdater.Start();

            _actingOrder = new List<GridObject>();
            roundInfoList.DataSource = _actingOrder;
            //Program.connection.AddMessageReceiver(DisplayDicePointsMessage.MESSAGE_TYPE, this);
        }

        private void ConnectionUpdate(object sender, EventArgs e)
        {
            //Program.connection.UpdateReceiver();
        }
        
        private GridObject GetSelectedGridObject()
        {
            if (_specifiedGridObject == null)
            {
                if (_mouseRow != -1 && _mouseCol != -1)
                {
                    Grid grid = _grids[_mouseRow, _mouseCol];
                    List<GridObject> land;
                    if (_mouseHighland) land = grid.highland;
                    else land = grid.lowland;
                    if (land.Count > 0)
                    {
                        GridObject gridObject = land[land.Count - 1];
                        return gridObject;
                    }
                }
                return null;
            }
            else
            {
                return _specifiedGridObject;
            }
        }

        private GridObject GetGridObject(int row, int col, string id)
        {
            Grid grid = _grids[row, col];
            for (int i = grid.lowland.Count - 1; i >= 0; --i)
            {
                if (grid.lowland[i].id == id) return grid.lowland[i];
            }
            for (int i = grid.highland.Count - 1; i >= 0; --i)
            {
                if (grid.highland[i].id == id) return grid.highland[i];
            }
            return null;
        }

        private void BattleSceneLookAtGrid(int row, int col)
        {
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

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            mouseGridPosLbl.Text = "Row:" + _mouseRow.ToString() + ", Col:" + _mouseCol.ToString() + ", Highland:" + _mouseHighland.ToString();
            GridObject gridObject = this.GetSelectedGridObject();
            selectedGridObjectLbl.Text = gridObject != null ? gridObject.ToString() : "无";
        }

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            battleScene.ViewerRectangleTop = vScrollBar.Value;
        }

        private void hScrollBar_ValueChanged(object sender, EventArgs e)
        {
            battleScene.ViewerRectangleLeft = hScrollBar.Value;
        }

        private void BattleSceneForm_Load(object sender, EventArgs e)
        {
            this.InitGrids(24, 24);
            for (int i = 0; i < _rows; ++i)
            {
                for (int j = 0; j < _cols; ++j)
                {
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

        private void UpdateScrollBar()
        {
            int deltaWidth = battleScene.CanvasWidth - battleScene.Width;
            int deltaHeight = battleScene.CanvasHeight - battleScene.Height;
            hScrollBar.Maximum = deltaWidth >= 0 ? deltaWidth : 0;
            vScrollBar.Maximum = deltaHeight >= 0 ? deltaHeight : 0;
        }
        
        private void InitGrids(int rows, int cols)
        {
            _rows = rows; _cols = cols;
            _grids = new Grid[rows, cols];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    _grids[i, j] = new Grid();
                }
            }
            battleScene.InitCanvas(new Size((int)(DIAMOND_LENGTH / 2.0f * Math.Sqrt(3.0f) * (rows + cols)), (int)(DIAMOND_LENGTH / 2.0f + DIAMOND_LENGTH / 2.0f * (rows + cols))));
        }

        private bool IsPointInPolygon(PointF[] polygon, PointF point)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                    (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        private void BattleSceneObjectPresent(GridObject gridObject, Graphics g, PointF[] gridPoints)
        {
            if (gridObject.view.battle == "地面")
            {
                g.FillPolygon(Brushes.LightGray, gridPoints);
            }
            else if (gridObject.view.battle == "障碍")
            {
                g.FillPolygon(Brushes.DarkGray, gridPoints);
            }
            else
            {
                g.DrawString(gridObject.view.battle, new Font("宋体", 12), Brushes.Black, new PointF(gridPoints[0].X - 12, gridPoints[0].Y + 12));
            }
        }

        private void battleScene_CanvasDrawing(object sender, CanvasDrawingEventArgs e)
        {
            Graphics g = e.g;
            g.Clear(Color.White);
            if (_grids != null)
            {
                for (int row = 0; row < _rows; ++row)
                {
                    for (int col = 0; col < _cols; ++col)
                    {
                        Grid grid = _grids[row, col];
                        if (grid.lowland.Count > 0)
                        {
                            PointF[] gridPoints = new PointF[4]
                            {
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
                            };
                            foreach (GridObject gridObject in grid.lowland)
                            {
                                this.BattleSceneObjectPresent(gridObject, g, gridPoints);
                            }
                            g.DrawPolygon(Pens.Black, gridPoints);
                        }
                        if (grid.highland.Count > 0)
                        {
                            PointF[] lowlandGridPoints = new PointF[4]
                                {
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
                                };
                            PointF[] gridPoints;
                            if (grid.isMiddleLand)
                            {
                                gridPoints = new PointF[4]
                                {
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
                                };
                            }
                            else
                            {
                                gridPoints = new PointF[4]
                                {
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
                                };
                            }
                            for (int k = 0; k < 4; ++k)
                            {
                                g.DrawLine(Pens.Red, gridPoints[k], lowlandGridPoints[k]);
                            }
                            foreach (GridObject gridObject in grid.highland)
                            {
                                this.BattleSceneObjectPresent(gridObject, g, gridPoints);
                            }
                            g.DrawPolygon(Pens.Red, gridPoints);
                        }
                    }
                }
            }
            if (_mouseRow != -1 && _mouseCol != -1)
            {
                int row = _mouseRow, col = _mouseCol;
                Grid grid = _grids[row, col];
                PointF[] diamondPoints;
                if (_mouseHighland)
                {
                    if (grid.isMiddleLand)
                    {
                        diamondPoints = new PointF[4]
                        {
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
                        };
                    }
                    else
                    {
                        diamondPoints = new PointF[4]
                        {
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
                        };
                    }
                }
                else
                {
                    diamondPoints = new PointF[4]
                    {
                        new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
                        new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
                        new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
                        new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
                    };
                }
                Pen pen;
                if (_specifiedGridObject != null)
                {
                    pen = new Pen(Brushes.Green, 3);
                }
                else
                {
                    pen = new Pen(Brushes.Blue, 3);
                }
                g.DrawPolygon(pen, diamondPoints);
            }
        }

        private void battleScene_SizeChanged(object sender, EventArgs e)
        {
            this.UpdateScrollBar();
        }
        
        private void battleScene_CanvasMouseDown(object sender, MouseEventArgs e)
        {
            if (_grids != null)
            {
                for (int row = 0; row < _rows; ++row)
                {
                    for (int col = 0; col < _cols; ++col)
                    {
                        Grid grid = _grids[row, col];
                        PointF[] diamondPoints;
                        if (grid.highland.Count > 0)
                        {
                            if (grid.isMiddleLand)
                            {
                                diamondPoints = new PointF[4]
                                {
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
                                };
                            }
                            else
                            {
                                diamondPoints = new PointF[4]
                                {
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 2) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (row + col + 1) * DIAMOND_LENGTH / 2.0f)
                                };
                            }
                            if (IsPointInPolygon(diamondPoints, new PointF(e.X, e.Y)))
                            {
                                if (!(_mouseRow == row && _mouseCol == col && _mouseHighland == true)) _specifiedGridObject = null;
                                _mouseRow = row; _mouseCol = col;
                                _mouseHighland = true;
                                return;
                            }
                        }
                        if (grid.lowland.Count > 0)
                        {
                            diamondPoints = new PointF[4]
                            {
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 2) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (col - row - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (row + col + 1) * DIAMOND_LENGTH / 2.0f)
                            };
                            if (IsPointInPolygon(diamondPoints, new PointF(e.X, e.Y)))
                            {
                                if (!(_mouseRow == row && _mouseCol == col && _mouseHighland == false)) _specifiedGridObject = null;
                                _mouseRow = row; _mouseCol = col;
                                _mouseHighland = false;
                                return;
                            }
                        }
                    }
                }
            }
            _specifiedGridObject = null;
            _mouseRow = -1; _mouseCol = -1;
            _mouseHighland = false;
        }

        private void battleSceneMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            GridObject gridObject = this.GetSelectedGridObject();
            if (gridObject == null)
            {
                e.Cancel = true;
                return;
            }
            menuItemMove.Enabled = menuItemUseSkill.Enabled = menuItemUseStunt.Enabled = menuItemExtraMovePoint.Enabled = menuItemRoundOver.Enabled = false;
            menuItemConfirm.Enabled = menuItemSelectAspect.Enabled = false;
            if (gridObject.id == _operatableID && gridObject.actable)
            {
                menuItemUseSkill.Enabled = menuItemUseStunt.Enabled = true;
                if (gridObject.movable) menuItemMove.Enabled = menuItemExtraMovePoint.Enabled = true;
            }
            if (_inChecking) menuItemSelectAspect.Enabled = true;
            
        }

        private void battleScene_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_mouseRow != -1 && _mouseCol != -1)
            {
                Grid grid = _grids[_mouseRow, _mouseCol];
                if (_mouseHighland) gridObjectSelectionList.DataSource = grid.highland;
                else gridObjectSelectionList.DataSource = grid.lowland;
                gridObjectSelectionList.Location = new Point(e.X + battleScene.Left, e.Y + battleScene.Top);
                gridObjectSelectionList.Visible = true;
            }
        }

        private void battleScene_MouseClick(object sender, MouseEventArgs e)
        {
            gridObjectSelectionList.Visible = false;
        }

        private void gridObjectSelectionList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _specifiedGridObject = (GridObject)gridObjectSelectionList.SelectedValue;
            gridObjectSelectionList.Visible = false;
        }

        private void menuItemMove_Click(object sender, EventArgs e)
        {

        }

        private void menuItemUseSkill_Click(object sender, EventArgs e)
        {

        }

        private void menuItemUseStunt_Click(object sender, EventArgs e)
        {

        }

        private void menuItemExtraMovePoint_Click(object sender, EventArgs e)
        {

        }

        private void menuItemRoundOver_Click(object sender, EventArgs e)
        {

        }
    }
}
