namespace TextyClient
{
    partial class SceneForm
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
			this.gridObjectSelectionListBox = new System.Windows.Forms.ListBox();
			this.battleScene = new TextyClient.GameScene();
			this.battleSceneMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemMove = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemExtraMovePoint = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemCreateAspect = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAttack = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemSpecialAction = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRoundOver = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemConfirmGrid = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemViewData = new System.Windows.Forms.ToolStripMenuItem();
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.confirmTargetBtn = new System.Windows.Forms.Button();
			this.selectAspectOverBtn = new System.Windows.Forms.Button();
			this.selectionTypeGroupPanel = new System.Windows.Forms.Panel();
			this.stuntRbn = new System.Windows.Forms.RadioButton();
			this.skillRbn = new System.Windows.Forms.RadioButton();
			this.selectionTypeLbl = new System.Windows.Forms.Label();
			this.skipAspectSelectionCbx = new System.Windows.Forms.CheckBox();
			this.confirmSelectionBtn = new System.Windows.Forms.Button();
			this.selectionListBox = new System.Windows.Forms.ListBox();
			this.leftActionPointLbl = new System.Windows.Forms.Label();
			this.leftMovePointLbl = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.selectedGridObjectLbl = new System.Windows.Forms.Label();
			this.mouseGridPosLbl = new System.Windows.Forms.Label();
			this.roundInfoLbl = new System.Windows.Forms.Label();
			this.dicePointsLbl = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.roundInfoListBox = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.battleScenePanel = new System.Windows.Forms.Panel();
			this.storyScenePanel = new System.Windows.Forms.Panel();
			this.groupBox1.SuspendLayout();
			this.battleSceneMenuStrip.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.selectionTypeGroupPanel.SuspendLayout();
			this.battleScenePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.gridObjectSelectionListBox);
			this.groupBox1.Controls.Add(this.battleScene);
			this.groupBox1.Controls.Add(this.hScrollBar);
			this.groupBox1.Controls.Add(this.vScrollBar);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(822, 598);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			// 
			// gridObjectSelectionListBox
			// 
			this.gridObjectSelectionListBox.FormattingEnabled = true;
			this.gridObjectSelectionListBox.ItemHeight = 12;
			this.gridObjectSelectionListBox.Location = new System.Drawing.Point(0, 0);
			this.gridObjectSelectionListBox.Name = "gridObjectSelectionListBox";
			this.gridObjectSelectionListBox.Size = new System.Drawing.Size(174, 88);
			this.gridObjectSelectionListBox.TabIndex = 9;
			this.gridObjectSelectionListBox.Visible = false;
			this.gridObjectSelectionListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gridObjectSelectionList_MouseDoubleClick);
			// 
			// battleScene
			// 
			this.battleScene.ContextMenuStrip = this.battleSceneMenuStrip;
			this.battleScene.Dock = System.Windows.Forms.DockStyle.Fill;
			this.battleScene.Location = new System.Drawing.Point(3, 17);
			this.battleScene.Name = "battleScene";
			this.battleScene.Size = new System.Drawing.Size(799, 561);
			this.battleScene.TabIndex = 6;
			this.battleScene.UpdateInterval = 100;
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
            this.menuItemExtraMovePoint,
            this.menuItemCreateAspect,
            this.menuItemAttack,
            this.menuItemSpecialAction,
            this.menuItemRoundOver,
            this.toolStripSeparator1,
            this.menuItemConfirmGrid,
            this.menuItemViewData});
			this.battleSceneMenuStrip.Name = "battleSceneMenuStrip";
			this.battleSceneMenuStrip.Size = new System.Drawing.Size(149, 186);
			this.battleSceneMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.battleSceneMenuStrip_Opening);
			// 
			// menuItemMove
			// 
			this.menuItemMove.Name = "menuItemMove";
			this.menuItemMove.Size = new System.Drawing.Size(148, 22);
			this.menuItemMove.Text = "移动";
			this.menuItemMove.Click += new System.EventHandler(this.menuItemMove_Click);
			// 
			// menuItemExtraMovePoint
			// 
			this.menuItemExtraMovePoint.Name = "menuItemExtraMovePoint";
			this.menuItemExtraMovePoint.Size = new System.Drawing.Size(148, 22);
			this.menuItemExtraMovePoint.Text = "额外移动点数";
			this.menuItemExtraMovePoint.Click += new System.EventHandler(this.menuItemExtraMovePoint_Click);
			// 
			// menuItemCreateAspect
			// 
			this.menuItemCreateAspect.Name = "menuItemCreateAspect";
			this.menuItemCreateAspect.Size = new System.Drawing.Size(148, 22);
			this.menuItemCreateAspect.Text = "创造优势";
			this.menuItemCreateAspect.Click += new System.EventHandler(this.menuItemCreateAspect_Click);
			// 
			// menuItemAttack
			// 
			this.menuItemAttack.Name = "menuItemAttack";
			this.menuItemAttack.Size = new System.Drawing.Size(148, 22);
			this.menuItemAttack.Text = "攻击";
			this.menuItemAttack.Click += new System.EventHandler(this.menuItemAttack_Click);
			// 
			// menuItemSpecialAction
			// 
			this.menuItemSpecialAction.Name = "menuItemSpecialAction";
			this.menuItemSpecialAction.Size = new System.Drawing.Size(148, 22);
			this.menuItemSpecialAction.Text = "特殊行动";
			this.menuItemSpecialAction.Click += new System.EventHandler(this.menuItemSpecialAction_Click);
			// 
			// menuItemRoundOver
			// 
			this.menuItemRoundOver.Name = "menuItemRoundOver";
			this.menuItemRoundOver.Size = new System.Drawing.Size(148, 22);
			this.menuItemRoundOver.Text = "回合结束";
			this.menuItemRoundOver.Click += new System.EventHandler(this.menuItemRoundOver_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
			// 
			// menuItemConfirmGrid
			// 
			this.menuItemConfirmGrid.Name = "menuItemConfirmGrid";
			this.menuItemConfirmGrid.Size = new System.Drawing.Size(148, 22);
			this.menuItemConfirmGrid.Text = "确认位置";
			this.menuItemConfirmGrid.Click += new System.EventHandler(this.menuItemConfirmGrid_Click);
			// 
			// menuItemViewData
			// 
			this.menuItemViewData.Name = "menuItemViewData";
			this.menuItemViewData.Size = new System.Drawing.Size(148, 22);
			this.menuItemViewData.Text = "查看角色信息";
			this.menuItemViewData.Click += new System.EventHandler(this.menuItemViewData_Click);
			// 
			// hScrollBar
			// 
			this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar.Location = new System.Drawing.Point(3, 578);
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
			this.vScrollBar.Size = new System.Drawing.Size(17, 578);
			this.vScrollBar.TabIndex = 7;
			this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.confirmTargetBtn);
			this.groupBox2.Controls.Add(this.selectAspectOverBtn);
			this.groupBox2.Controls.Add(this.selectionTypeGroupPanel);
			this.groupBox2.Controls.Add(this.selectionTypeLbl);
			this.groupBox2.Controls.Add(this.skipAspectSelectionCbx);
			this.groupBox2.Controls.Add(this.confirmSelectionBtn);
			this.groupBox2.Controls.Add(this.selectionListBox);
			this.groupBox2.Controls.Add(this.leftActionPointLbl);
			this.groupBox2.Controls.Add(this.leftMovePointLbl);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.selectedGridObjectLbl);
			this.groupBox2.Controls.Add(this.mouseGridPosLbl);
			this.groupBox2.Controls.Add(this.roundInfoLbl);
			this.groupBox2.Controls.Add(this.dicePointsLbl);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.roundInfoListBox);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupBox2.Location = new System.Drawing.Point(822, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 598);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			// 
			// confirmTargetBtn
			// 
			this.confirmTargetBtn.Location = new System.Drawing.Point(113, 81);
			this.confirmTargetBtn.Name = "confirmTargetBtn";
			this.confirmTargetBtn.Size = new System.Drawing.Size(75, 23);
			this.confirmTargetBtn.TabIndex = 29;
			this.confirmTargetBtn.Text = "确认选择";
			this.confirmTargetBtn.UseVisualStyleBackColor = true;
			// 
			// selectAspectOverBtn
			// 
			this.selectAspectOverBtn.Location = new System.Drawing.Point(91, 532);
			this.selectAspectOverBtn.Name = "selectAspectOverBtn";
			this.selectAspectOverBtn.Size = new System.Drawing.Size(97, 23);
			this.selectAspectOverBtn.TabIndex = 28;
			this.selectAspectOverBtn.Text = "结束特征选择";
			this.selectAspectOverBtn.UseVisualStyleBackColor = true;
			this.selectAspectOverBtn.Visible = false;
			this.selectAspectOverBtn.Click += new System.EventHandler(this.selectAspectOverBtn_Click);
			// 
			// selectionTypeGroupPanel
			// 
			this.selectionTypeGroupPanel.Controls.Add(this.stuntRbn);
			this.selectionTypeGroupPanel.Controls.Add(this.skillRbn);
			this.selectionTypeGroupPanel.Location = new System.Drawing.Point(10, 531);
			this.selectionTypeGroupPanel.Name = "selectionTypeGroupPanel";
			this.selectionTypeGroupPanel.Size = new System.Drawing.Size(178, 24);
			this.selectionTypeGroupPanel.TabIndex = 27;
			this.selectionTypeGroupPanel.Visible = false;
			// 
			// stuntRbn
			// 
			this.stuntRbn.AutoSize = true;
			this.stuntRbn.Location = new System.Drawing.Point(56, 3);
			this.stuntRbn.Name = "stuntRbn";
			this.stuntRbn.Size = new System.Drawing.Size(47, 16);
			this.stuntRbn.TabIndex = 1;
			this.stuntRbn.Text = "特技";
			this.stuntRbn.UseVisualStyleBackColor = true;
			this.stuntRbn.CheckedChanged += new System.EventHandler(this.stuntRbn_CheckedChanged);
			// 
			// skillRbn
			// 
			this.skillRbn.AutoSize = true;
			this.skillRbn.Checked = true;
			this.skillRbn.Location = new System.Drawing.Point(3, 3);
			this.skillRbn.Name = "skillRbn";
			this.skillRbn.Size = new System.Drawing.Size(47, 16);
			this.skillRbn.TabIndex = 0;
			this.skillRbn.TabStop = true;
			this.skillRbn.Text = "技能";
			this.skillRbn.UseVisualStyleBackColor = true;
			this.skillRbn.CheckedChanged += new System.EventHandler(this.skillRbn_CheckedChanged);
			// 
			// selectionTypeLbl
			// 
			this.selectionTypeLbl.AutoSize = true;
			this.selectionTypeLbl.Location = new System.Drawing.Point(9, 381);
			this.selectionTypeLbl.Name = "selectionTypeLbl";
			this.selectionTypeLbl.Size = new System.Drawing.Size(83, 12);
			this.selectionTypeLbl.TabIndex = 26;
			this.selectionTypeLbl.Text = "selectionType";
			// 
			// skipAspectSelectionCbx
			// 
			this.skipAspectSelectionCbx.AutoSize = true;
			this.skipAspectSelectionCbx.Location = new System.Drawing.Point(11, 506);
			this.skipAspectSelectionCbx.Name = "skipAspectSelectionCbx";
			this.skipAspectSelectionCbx.Size = new System.Drawing.Size(96, 16);
			this.skipAspectSelectionCbx.TabIndex = 25;
			this.skipAspectSelectionCbx.Text = "跳过特征选择";
			this.skipAspectSelectionCbx.UseVisualStyleBackColor = true;
			this.skipAspectSelectionCbx.CheckedChanged += new System.EventHandler(this.skipAspectSelectionCbx_CheckedChanged);
			// 
			// confirmSelectionBtn
			// 
			this.confirmSelectionBtn.Location = new System.Drawing.Point(113, 502);
			this.confirmSelectionBtn.Name = "confirmSelectionBtn";
			this.confirmSelectionBtn.Size = new System.Drawing.Size(75, 23);
			this.confirmSelectionBtn.TabIndex = 24;
			this.confirmSelectionBtn.Text = "确认";
			this.confirmSelectionBtn.UseVisualStyleBackColor = true;
			this.confirmSelectionBtn.Click += new System.EventHandler(this.confirmSelectionBtn_Click);
			// 
			// selectionListBox
			// 
			this.selectionListBox.FormattingEnabled = true;
			this.selectionListBox.ItemHeight = 12;
			this.selectionListBox.Location = new System.Drawing.Point(10, 396);
			this.selectionListBox.Name = "selectionListBox";
			this.selectionListBox.Size = new System.Drawing.Size(178, 100);
			this.selectionListBox.TabIndex = 23;
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
			this.label5.Size = new System.Drawing.Size(77, 12);
			this.label5.TabIndex = 20;
			this.label5.Text = "当前选择对象";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 340);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(77, 12);
			this.label4.TabIndex = 19;
			this.label4.Text = "你剩余行动点";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 319);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 12);
			this.label1.TabIndex = 18;
			this.label1.Text = "你剩余移动点";
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
			this.label3.Size = new System.Drawing.Size(53, 12);
			this.label3.TabIndex = 8;
			this.label3.Text = "骰子点数";
			// 
			// roundInfoListBox
			// 
			this.roundInfoListBox.FormattingEnabled = true;
			this.roundInfoListBox.ItemHeight = 12;
			this.roundInfoListBox.Location = new System.Drawing.Point(8, 163);
			this.roundInfoListBox.Name = "roundInfoListBox";
			this.roundInfoListBox.Size = new System.Drawing.Size(180, 148);
			this.roundInfoListBox.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 145);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "行动顺序";
			// 
			// battleScenePanel
			// 
			this.battleScenePanel.Controls.Add(this.groupBox1);
			this.battleScenePanel.Controls.Add(this.groupBox2);
			this.battleScenePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.battleScenePanel.Location = new System.Drawing.Point(0, 0);
			this.battleScenePanel.Name = "battleScenePanel";
			this.battleScenePanel.Size = new System.Drawing.Size(1022, 598);
			this.battleScenePanel.TabIndex = 10;
			// 
			// storyScenePanel
			// 
			this.storyScenePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.storyScenePanel.Location = new System.Drawing.Point(0, 0);
			this.storyScenePanel.Name = "storyScenePanel";
			this.storyScenePanel.Size = new System.Drawing.Size(1022, 598);
			this.storyScenePanel.TabIndex = 10;
			// 
			// SceneForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1022, 598);
			this.Controls.Add(this.battleScenePanel);
			this.Controls.Add(this.storyScenePanel);
			this.Name = "SceneForm";
			this.Text = "SceneForm";
			this.Load += new System.EventHandler(this.BattleSceneForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.battleSceneMenuStrip.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.selectionTypeGroupPanel.ResumeLayout(false);
			this.selectionTypeGroupPanel.PerformLayout();
			this.battleScenePanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private GameScene battleScene;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox roundInfoListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label dicePointsLbl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label roundInfoLbl;
        private System.Windows.Forms.ContextMenuStrip battleSceneMenuStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewData;
        private System.Windows.Forms.Label mouseGridPosLbl;
        private System.Windows.Forms.ListBox gridObjectSelectionListBox;
        private System.Windows.Forms.Label selectedGridObjectLbl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label leftActionPointLbl;
        private System.Windows.Forms.Label leftMovePointLbl;
        private System.Windows.Forms.ToolStripMenuItem menuItemMove;
        private System.Windows.Forms.ToolStripMenuItem menuItemCreateAspect;
        private System.Windows.Forms.ToolStripMenuItem menuItemSpecialAction;
        private System.Windows.Forms.ToolStripMenuItem menuItemConfirmGrid;
        private System.Windows.Forms.ToolStripMenuItem menuItemRoundOver;
        private System.Windows.Forms.Button confirmSelectionBtn;
        private System.Windows.Forms.ListBox selectionListBox;
        private System.Windows.Forms.CheckBox skipAspectSelectionCbx;
        private System.Windows.Forms.Label selectionTypeLbl;
        private System.Windows.Forms.Panel selectionTypeGroupPanel;
        private System.Windows.Forms.RadioButton stuntRbn;
        private System.Windows.Forms.RadioButton skillRbn;
        private System.Windows.Forms.ToolStripMenuItem menuItemAttack;
        private System.Windows.Forms.Button selectAspectOverBtn;
        private System.Windows.Forms.ToolStripMenuItem menuItemExtraMovePoint;
        private System.Windows.Forms.Button confirmTargetBtn;
		private System.Windows.Forms.Panel battleScenePanel;
		private System.Windows.Forms.Panel storyScenePanel;
	}
}