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
                        Grid grid = _grids[pushGridObjMessage.row, pushGridObjMessage.col];
                        GridObject gridObject = new GridObject(pushGridObjMessage.objID, pushGridObjMessage.view);
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
                        Grid grid = _grids[removeGridObjMessage.row, removeGridObjMessage.col];
                        for (int i = grid.lowland.Count - 1; i >= 0; --i)
                        {
                            if (grid.lowland[i].id == removeGridObjMessage.objID)
                            {
                                grid.lowland.RemoveAt(i);
                                return;
                            }
                        }
                        for (int i = grid.highland.Count - 1; i >= 0; --i)
                        {
                            if (grid.highland[i].id == removeGridObjMessage.objID)
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
                        BattleSceneRemoveGridObjectMessage removeGridObjMessage = (BattleSceneRemoveGridObjectMessage)message;
                        Grid grid = _grids[removeGridObjMessage.row, removeGridObjMessage.col];
                        SideObject sideObject = new SideObject(addLadderObjMessage.objID, addLadderObjMessage.view);
                        switch (addLadderObjMessage.direction)
                        {
                            case (int)Direction.POSITIVE_ROW:
                                grid.positiveRowLadder = sideObject;
                                _grids[removeGridObjMessage.row + 1, removeGridObjMessage.col].negativeRowLadder = sideObject;
                                break;
                            case (int)Direction.POSITIVE_COL:
                                grid.positiveRowLadder = sideObject;
                                _grids[removeGridObjMessage.row, removeGridObjMessage.col + 1].negativeColLadder = sideObject;
                                break;
                            case (int)Direction.NEGATIVE_ROW:
                                grid.positiveRowLadder = sideObject;
                                _grids[removeGridObjMessage.row - 1, removeGridObjMessage.col].positiveRowLadder = sideObject;
                                break;
                            case (int)Direction.NEGATIVE_COL:
                                grid.positiveRowLadder = sideObject;
                                _grids[removeGridObjMessage.row, removeGridObjMessage.col - 1].positiveColLadder = sideObject;
                                break;
                            default:
                                return;
                        }
                    }
                    break;
                case BattleSceneRemoveLadderObjectMessage.MESSAGE_TYPE:
                    {
                        BattleSceneRemoveLadderObjectMessage removeLadderObjMessage = (BattleSceneRemoveLadderObjectMessage)message;
                        Grid grid = _grids[removeLadderObjMessage.row, removeLadderObjMessage.col];
                        if (grid.positiveRowLadder != null && grid.positiveRowLadder.id == removeLadderObjMessage.objID)
                        {
                            grid.positiveRowLadder = null;
                            _grids[removeLadderObjMessage.row + 1, removeLadderObjMessage.col].negativeRowLadder = null;
                        }
                        else if (grid.positiveColLadder != null && grid.positiveColLadder.id == removeLadderObjMessage.objID)
                        {
                            grid.positiveColLadder = null;
                            _grids[removeLadderObjMessage.row, removeLadderObjMessage.col + 1].negativeColLadder = null;
                        }
                        else if (grid.negativeRowLadder != null && grid.negativeRowLadder.id == removeLadderObjMessage.objID)
                        {
                            grid.negativeRowLadder = null;
                            _grids[removeLadderObjMessage.row - 1, removeLadderObjMessage.col].positiveRowLadder = null;
                        }
                        else if (grid.negativeColLadder != null && grid.negativeColLadder.id == removeLadderObjMessage.objID)
                        {
                            grid.negativeColLadder = null;
                            _grids[removeLadderObjMessage.row, removeLadderObjMessage.col - 1].positiveColLadder = null;
                        }
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
            connectionUpdater.Interval = 1;
            connectionUpdater.Tick += new EventHandler(this.ConnectionUpdate);
            connectionUpdater.Start();
            
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
            _grids[0, 0].highland.Add(new GridObject("", view2));
            _grids[0, 1].lowland.Add(new GridObject("", view2));
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

        private void battleScene_CanvasDrawing(object sender, CanvasDrawingEventArgs e)
        {
            Graphics g = e.g;
            g.Clear(Color.White);
            if (_grids != null)
            {
                for (int i = 0; i < _rows; ++i)
                {
                    for (int j = 0; j < _cols; ++j)
                    {
                        Grid grid = _grids[i, j];
                        foreach (GridObject gridObject in grid.lowland)
                        {
                            PointF[] diamondPoints = new PointF[4]
                            {
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 2) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f)
                            };
                            if (gridObject.view.battle == "地面")
                            {
                                g.FillPolygon(Brushes.LightGray, diamondPoints);
                            }
                            else
                            {
                                g.DrawString(gridObject.view.battle, new Font("宋体", 12), Brushes.Black, new PointF(diamondPoints[0].X - 12, diamondPoints[0].Y + 12));
                            }
                            g.DrawPolygon(Pens.Black, diamondPoints);
                        }
                        foreach (GridObject gridObject in grid.highland)
                        {
                            PointF[] lowlandDiamondPoints = new PointF[4]
                            {
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 2) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f)
                            };
                            PointF[] diamondPoints;
                            if (grid.isMiddleLand)
                            {
                                diamondPoints = new PointF[4]
                                {
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j + 2) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f)
                                };
                            }
                            else
                            {
                                diamondPoints = new PointF[4]
                                {
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j + 1) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j + 2) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j + 1) * DIAMOND_LENGTH / 2.0f)
                                };
                            }
                            if (gridObject.view.battle == "地面")
                            {
                                g.FillPolygon(Brushes.LightGray, diamondPoints);
                            }
                            else
                            {
                                g.DrawString(gridObject.view.battle, new Font("宋体", 12), Brushes.Black, new PointF(diamondPoints[0].X - 12, diamondPoints[0].Y + 12));
                            }
                            g.DrawPolygon(Pens.Red, diamondPoints);
                            for (int k = 0; k < 4; ++k)
                            {
                                g.DrawLine(Pens.Red, diamondPoints[k], lowlandDiamondPoints[k]);
                            }
                        }
                    }
                }
            }
            if (_mouseRow != -1 && _mouseCol != -1)
            {
                int i = _mouseRow, j = _mouseCol;
                Grid grid = _grids[i, j];
                PointF[] diamondPoints;
                if (_mouseHighland)
                {
                    if (grid.isMiddleLand)
                    {
                        diamondPoints = new PointF[4]
                        {
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j + 2) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f)
                        };
                    }
                    else
                    {
                        diamondPoints = new PointF[4]
                        {
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j + 1) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j + 2) * DIAMOND_LENGTH / 2.0f),
                            new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j + 1) * DIAMOND_LENGTH / 2.0f)
                        };
                    }
                }
                else
                {
                    diamondPoints = new PointF[4]
                    {
                        new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j) * DIAMOND_LENGTH / 2.0f),
                        new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f),
                        new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 2) * DIAMOND_LENGTH / 2.0f),
                        new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f)
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
                for (int i = 0; i < _rows; ++i)
                {
                    for (int j = 0; j < _cols; ++j)
                    {
                        Grid grid = _grids[i, j];
                        PointF[] diamondPoints;
                        if (grid.highland.Count > 0)
                        {
                            if (grid.isMiddleLand)
                            {
                                diamondPoints = new PointF[4]
                                {
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j + 2) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 4.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f)
                                };
                            }
                            else
                            {
                                diamondPoints = new PointF[4]
                                {
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j + 1) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j + 2) * DIAMOND_LENGTH / 2.0f),
                                    new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), (i + j + 1) * DIAMOND_LENGTH / 2.0f)
                                };
                            }
                            if (IsPointInPolygon(diamondPoints, new PointF(e.X, e.Y)))
                            {
                                if (!(_mouseRow == i && _mouseCol == j && _mouseHighland == true)) _specifiedGridObject = null;
                                _mouseRow = i; _mouseCol = j;
                                _mouseHighland = true;
                                return;
                            }
                        }
                        if (grid.lowland.Count > 0)
                        {
                            diamondPoints = new PointF[4]
                            {
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i + 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 2) * DIAMOND_LENGTH / 2.0f),
                                new PointF(DIAMOND_LENGTH * _rows / 2.0f * (float)Math.Sqrt(3.0f) + (j - i - 1) * DIAMOND_LENGTH / 2.0f * (float)Math.Sqrt(3.0f), DIAMOND_LENGTH / 2.0f + (i + j + 1) * DIAMOND_LENGTH / 2.0f)
                            };
                            if (IsPointInPolygon(diamondPoints, new PointF(e.X, e.Y)))
                            {
                                if (!(_mouseRow == i && _mouseCol == j && _mouseHighland == false)) _specifiedGridObject = null;
                                _mouseRow = i; _mouseCol = j;
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
            if (!gridObject.actable)
            {

            }
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

    }
}
