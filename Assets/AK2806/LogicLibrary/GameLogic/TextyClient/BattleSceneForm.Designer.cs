namespace TextyClient
{
    partial class BattleSceneForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gridObjectSelectionList = new System.Windows.Forms.ListBox();
            this.battleScene = new TextyClient.GameScene();
            this.battleSceneMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemSelectAspect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemViewData = new System.Windows.Forms.ToolStripMenuItem();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.leftActionPointLbl = new System.Windows.Forms.Label();
            this.leftMovePointLbl = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.roundOverBtn = new System.Windows.Forms.Button();
            this.selectedGridObjectLbl = new System.Windows.Forms.Label();
            this.mouseGridPosLbl = new System.Windows.Forms.Label();
            this.roundInfoLbl = new System.Windows.Forms.Label();
            this.dicePointsLbl = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.roundInfoList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.menuItemConfirm = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMove = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemUseSkill = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemUseStunt = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExtraMovePoint = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.battleSceneMenuStrip.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gridObjectSelectionList);
            this.groupBox1.Controls.Add(this.battleScene);
            this.groupBox1.Controls.Add(this.hScrollBar);
            this.groupBox1.Controls.Add(this.vScrollBar);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(822, 527);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // gridObjectSelectionList
            // 
            this.gridObjectSelectionList.FormattingEnabled = true;
            this.gridObjectSelectionList.ItemHeight = 12;
            this.gridObjectSelectionList.Location = new System.Drawing.Point(0, 0);
            this.gridObjectSelectionList.Name = "gridObjectSelectionList";
            this.gridObjectSelectionList.Size = new System.Drawing.Size(150, 88);
            this.gridObjectSelectionList.TabIndex = 9;
            this.gridObjectSelectionList.Visible = false;
            this.gridObjectSelectionList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gridObjectSelectionList_MouseDoubleClick);
            // 
            // battleScene
            // 
            this.battleScene.ContextMenuStrip = this.battleSceneMenuStrip;
            this.battleScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.battleScene.Location = new System.Drawing.Point(3, 17);
            this.battleScene.Name = "battleScene";
            this.battleScene.Size = new System.Drawing.Size(799, 490);
            this.battleScene.TabIndex = 6;
            this.battleScene.ViewerRectangleLeft = 0;
            this.battleScene.ViewerRectangleLeftTop = new System.Drawing.Point(0, 0);
            this.battleScene.ViewerRectangleTop = 0;
            this.battleScene.CanvasDrawing += new System.EventHandler<TextyClient.CanvasDrawingEventArgs>(this.battleScene_CanvasDrawing);
            this.battleScene.CanvasMouseDown += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.battleScene_CanvasMouseDown);
            this.battleScene.SizeChanged += new System.EventHandler(this.battleScene_SizeChanged);
            this.battleScene.MouseClick += new System.Windows.Forms.MouseEventHandler(this.battleScene_MouseClick);
            this.battleScene.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.battleScene_MouseDoubleClick);
            // 
            // battleSceneMenuStrip
            // 
            this.battleSceneMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemMove,
            this.menuItemUseSkill,
            this.menuItemUseStunt,
            this.menuItemExtraMovePoint,
            this.toolStripSeparator1,
            this.menuItemConfirm,
            this.menuItemSelectAspect,
            this.menuItemViewData});
            this.battleSceneMenuStrip.Name = "battleSceneMenuStrip";
            this.battleSceneMenuStrip.Size = new System.Drawing.Size(149, 164);
            this.battleSceneMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.battleSceneMenuStrip_Opening);
            // 
            // menuItemSelectAspect
            // 
            this.menuItemSelectAspect.Name = "menuItemSelectAspect";
            this.menuItemSelectAspect.Size = new System.Drawing.Size(148, 22);
            this.menuItemSelectAspect.Text = "选择特征";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
            // 
            // menuItemViewData
            // 
            this.menuItemViewData.Name = "menuItemViewData";
            this.menuItemViewData.Size = new System.Drawing.Size(148, 22);
            this.menuItemViewData.Text = "查看角色信息";
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(3, 507);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(799, 17);
            this.hScrollBar.TabIndex = 8;
            this.hScrollBar.ValueChanged += new System.EventHandler(this.hScrollBar_ValueChanged);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(802, 17);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(17, 507);
            this.vScrollBar.TabIndex = 7;
            this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.leftActionPointLbl);
            this.groupBox2.Controls.Add(this.leftMovePointLbl);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.roundOverBtn);
            this.groupBox2.Controls.Add(this.selectedGridObjectLbl);
            this.groupBox2.Controls.Add(this.mouseGridPosLbl);
            this.groupBox2.Controls.Add(this.roundInfoLbl);
            this.groupBox2.Controls.Add(this.dicePointsLbl);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.roundInfoList);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox2.Location = new System.Drawing.Point(822, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 527);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            // 
            // leftActionPointLbl
            // 
            this.leftActionPointLbl.AutoSize = true;
            this.leftActionPointLbl.Location = new System.Drawing.Point(89, 340);
            this.leftActionPointLbl.Name = "leftActionPointLbl";
            this.leftActionPointLbl.Size = new System.Drawing.Size(41, 12);
            this.leftActionPointLbl.TabIndex = 22;
            this.leftActionPointLbl.Text = "label7";
            // 
            // leftMovePointLbl
            // 
            this.leftMovePointLbl.AutoSize = true;
            this.leftMovePointLbl.Location = new System.Drawing.Point(89, 319);
            this.leftMovePointLbl.Name = "leftMovePointLbl";
            this.leftMovePointLbl.Size = new System.Drawing.Size(41, 12);
            this.leftMovePointLbl.TabIndex = 21;
            this.leftMovePointLbl.Text = "label6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 20;
            this.label5.Text = "当前选择对象：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 340);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 19;
            this.label4.Text = "剩余行动点：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 319);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "剩余移动点：";
            // 
            // roundOverBtn
            // 
            this.roundOverBtn.Location = new System.Drawing.Point(113, 355);
            this.roundOverBtn.Name = "roundOverBtn";
            this.roundOverBtn.Size = new System.Drawing.Size(75, 23);
            this.roundOverBtn.TabIndex = 13;
            this.roundOverBtn.Text = "回合结束";
            this.roundOverBtn.UseVisualStyleBackColor = true;
            // 
            // selectedGridObjectLbl
            // 
            this.selectedGridObjectLbl.AutoSize = true;
            this.selectedGridObjectLbl.Location = new System.Drawing.Point(6, 61);
            this.selectedGridObjectLbl.Name = "selectedGridObjectLbl";
            this.selectedGridObjectLbl.Size = new System.Drawing.Size(113, 12);
            this.selectedGridObjectLbl.TabIndex = 12;
            this.selectedGridObjectLbl.Text = "selectedGridObject";
            // 
            // mouseGridPosLbl
            // 
            this.mouseGridPosLbl.AutoSize = true;
            this.mouseGridPosLbl.Location = new System.Drawing.Point(6, 17);
            this.mouseGridPosLbl.Name = "mouseGridPosLbl";
            this.mouseGridPosLbl.Size = new System.Drawing.Size(77, 12);
            this.mouseGridPosLbl.TabIndex = 11;
            this.mouseGridPosLbl.Text = "mouseGridPos";
            // 
            // roundInfoLbl
            // 
            this.roundInfoLbl.AutoSize = true;
            this.roundInfoLbl.Location = new System.Drawing.Point(129, 145);
            this.roundInfoLbl.Name = "roundInfoLbl";
            this.roundInfoLbl.Size = new System.Drawing.Size(59, 12);
            this.roundInfoLbl.TabIndex = 10;
            this.roundInfoLbl.Text = "roundInfo";
            // 
            // dicePointsLbl
            // 
            this.dicePointsLbl.AutoSize = true;
            this.dicePointsLbl.Location = new System.Drawing.Point(6, 111);
            this.dicePointsLbl.Name = "dicePointsLbl";
            this.dicePointsLbl.Size = new System.Drawing.Size(65, 12);
            this.dicePointsLbl.TabIndex = 9;
            this.dicePointsLbl.Text = "dicePoints";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "骰子点数：";
            // 
            // roundInfoList
            // 
            this.roundInfoList.FormattingEnabled = true;
            this.roundInfoList.ItemHeight = 12;
            this.roundInfoList.Location = new System.Drawing.Point(8, 163);
            this.roundInfoList.Name = "roundInfoList";
            this.roundInfoList.Size = new System.Drawing.Size(180, 112);
            this.roundInfoList.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "行动顺序：";
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 16;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // menuItemConfirm
            // 
            this.menuItemConfirm.Name = "menuItemConfirm";
            this.menuItemConfirm.Size = new System.Drawing.Size(148, 22);
            this.menuItemConfirm.Text = "确认选择";
            // 
            // menuItemMove
            // 
            this.menuItemMove.Name = "menuItemMove";
            this.menuItemMove.Size = new System.Drawing.Size(148, 22);
            this.menuItemMove.Text = "移动";
            // 
            // menuItemUseSkill
            // 
            this.menuItemUseSkill.Name = "menuItemUseSkill";
            this.menuItemUseSkill.Size = new System.Drawing.Size(148, 22);
            this.menuItemUseSkill.Text = "使用技能";
            // 
            // menuItemUseStunt
            // 
            this.menuItemUseStunt.Name = "menuItemUseStunt";
            this.menuItemUseStunt.Size = new System.Drawing.Size(148, 22);
            this.menuItemUseStunt.Text = "使用特技";
            // 
            // menuItemExtraMovePoint
            // 
            this.menuItemExtraMovePoint.Name = "menuItemExtraMovePoint";
            this.menuItemExtraMovePoint.Size = new System.Drawing.Size(148, 22);
            this.menuItemExtraMovePoint.Text = "额外移动点数";
            // 
            // BattleSceneForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 527);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "BattleSceneForm";
            this.Text = "BattleSceneForm";
            this.Load += new System.EventHandler(this.BattleSceneForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.battleSceneMenuStrip.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private GameScene battleScene;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox roundInfoList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label dicePointsLbl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label roundInfoLbl;
        private System.Windows.Forms.ContextMenuStrip battleSceneMenuStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemSelectAspect;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewData;
        private System.Windows.Forms.Label mouseGridPosLbl;
        private System.Windows.Forms.ListBox gridObjectSelectionList;
        private System.Windows.Forms.Label selectedGridObjectLbl;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.Button roundOverBtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label leftActionPointLbl;
        private System.Windows.Forms.Label leftMovePointLbl;
        private System.Windows.Forms.ToolStripMenuItem menuItemMove;
        private System.Windows.Forms.ToolStripMenuItem menuItemUseSkill;
        private System.Windows.Forms.ToolStripMenuItem menuItemUseStunt;
        private System.Windows.Forms.ToolStripMenuItem menuItemExtraMovePoint;
        private System.Windows.Forms.ToolStripMenuItem menuItemConfirm;
    }
}