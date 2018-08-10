using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextyClient
{
    public partial class GameScene : UserControl
    {
        private Bitmap _backbuffer = null;
        private Point _viewerRectangleLeftTop = new Point(0, 0);
        
        public Size CanvasSize => _backbuffer.Size;
        public int CanvasWidth => _backbuffer.Width;
        public int CanvasHeight => _backbuffer.Height;

        public int ViewerRectangleLeft { get => _viewerRectangleLeftTop.X; set => _viewerRectangleLeftTop.X = value; }
        public int ViewerRectangleTop { get => _viewerRectangleLeftTop.Y; set => _viewerRectangleLeftTop.Y = value; }
        public Point ViewerRectangleLeftTop { get => _viewerRectangleLeftTop; set => _viewerRectangleLeftTop = value; }

        public event EventHandler<CanvasDrawingEventArgs> CanvasDrawing;
        public event EventHandler<MouseEventArgs> CanvasMouseMove;
        public event EventHandler<MouseEventArgs> CanvasMouseUp;
        public event EventHandler<MouseEventArgs> CanvasMouseDown;
        public event EventHandler<MouseEventArgs> CanvasMouseClick;

        public GameScene()
        {
            InitializeComponent();
            
            this.SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.DoubleBuffer, true);

            Timer GameTimer = new Timer();
            GameTimer.Interval = 16;
            GameTimer.Tick += new EventHandler(this.GameTimer_Tick);
            GameTimer.Start();
        }
        
        public void InitCanvas(Size size)
        {
            if (_backbuffer != null)
                _backbuffer.Dispose();
            _backbuffer = new Bitmap(size.Width, size.Height);
        }

        private void GameScene_Paint(object sender, PaintEventArgs e)
        {
            if (_backbuffer != null)
            {
                e.Graphics.DrawImage(_backbuffer, new Rectangle(0, 0, this.Width, this.Height), new Rectangle(_viewerRectangleLeftTop, this.Size), GraphicsUnit.Pixel);
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (_backbuffer != null)
            {
                using (var g = Graphics.FromImage(_backbuffer))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    CanvasDrawingEventArgs args = new CanvasDrawingEventArgs();
                    args.g = g;
                    args.canvasSize = _backbuffer.Size;
                    this.CanvasDrawing?.Invoke(this, args);
                }

                Invalidate();
            }
        }

        private void GameScene_MouseMove(object sender, MouseEventArgs e)
        {
            this.CanvasMouseMove?.Invoke(this, new MouseEventArgs(e.Button, e.Clicks, _viewerRectangleLeftTop.X + e.X, _viewerRectangleLeftTop.Y + e.Y, e.Delta));
        }

        private void GameScene_MouseUp(object sender, MouseEventArgs e)
        {
            this.CanvasMouseUp?.Invoke(this, new MouseEventArgs(e.Button, e.Clicks, _viewerRectangleLeftTop.X + e.X, _viewerRectangleLeftTop.Y + e.Y, e.Delta));
        }

        private void GameScene_MouseDown(object sender, MouseEventArgs e)
        {
            this.CanvasMouseDown?.Invoke(this, new MouseEventArgs(e.Button, e.Clicks, _viewerRectangleLeftTop.X + e.X, _viewerRectangleLeftTop.Y + e.Y, e.Delta));
        }

        private void GameScene_MouseClick(object sender, MouseEventArgs e)
        {
            this.CanvasMouseClick?.Invoke(this, new MouseEventArgs(e.Button, e.Clicks, _viewerRectangleLeftTop.X + e.X, _viewerRectangleLeftTop.Y + e.Y, e.Delta));
        }
    }

    public class CanvasDrawingEventArgs : EventArgs
    {
        public Graphics g;
        public Size canvasSize;
    }
}
