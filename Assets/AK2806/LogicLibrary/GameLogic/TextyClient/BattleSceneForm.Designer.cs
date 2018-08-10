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
            this.battleSceneMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemMove = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemUseSkillButton = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemUseStunt = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemRoundOver = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemSelectAspect = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewData = new System.Windows.Forms.ToolStripMenuItem();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mouseGridPos = new System.Windows.Forms.Label();
            this.roundInfo = new System.Windows.Forms.Label();
            this.dicePoints = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.roundInfoList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mousePos = new System.Windows.Forms.Label();
            this.battleScene = new TextyClient.GameScene();
            this.groupBox1.SuspendLayout();
            this.battleSceneMenuStrip.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
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
            // battleSceneMenuStrip
            // 
            this.battleSceneMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemMove,
            this.menuItemUseSkillButton,
            this.menuItemUseStunt,
            this.menuItemRoundOver,
            this.toolStripSeparator1,
            this.menuItemSelectAspect,
            this.menuItemViewData});
            this.battleSceneMenuStrip.Name = "battleSceneMenuStrip";
            this.battleSceneMenuStrip.Size = new System.Drawing.Size(149, 142);
            this.battleSceneMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.battleSceneMenuStrip_Opening);
            // 
            // menuItemMove
            // 
            this.menuItemMove.Name = "menuItemMove";
            this.menuItemMove.Size = new System.Drawing.Size(148, 22);
            this.menuItemMove.Text = "移动";
            // 
            // menuItemUseSkillButton
            // 
            this.menuItemUseSkillButton.Name = "menuItemUseSkillButton";
            this.menuItemUseSkillButton.Size = new System.Drawing.Size(148, 22);
            this.menuItemUseSkillButton.Text = "使用技能";
            // 
            // menuItemUseStunt
            // 
            this.menuItemUseStunt.Name = "menuItemUseStunt";
            this.menuItemUseStunt.Size = new System.Drawing.Size(148, 22);
            this.menuItemUseStunt.Text = "使用特技";
            // 
            // menuItemRoundOver
            // 
            this.menuItemRoundOver.Name = "menuItemRoundOver";
            this.menuItemRoundOver.Size = new System.Drawing.Size(148, 22);
            this.menuItemRoundOver.Text = "回合结束";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
            // 
            // menuItemSelectAspect
            // 
            this.menuItemSelectAspect.Name = "menuItemSelectAspect";
            this.menuItemSelectAspect.Size = new System.Drawing.Size(148, 22);
            this.menuItemSelectAspect.Text = "选择特征";
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
            this.groupBox2.Controls.Add(this.mouseGridPos);
            this.groupBox2.Controls.Add(this.roundInfo);
            this.groupBox2.Controls.Add(this.dicePoints);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.roundInfoList);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.mousePos);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox2.Location = new System.Drawing.Point(822, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 527);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            // 
            // mouseGridPos
            // 
            this.mouseGridPos.AutoSize = true;
            this.mouseGridPos.Location = new System.Drawing.Point(6, 29);
            this.mouseGridPos.Name = "mouseGridPos";
            this.mouseGridPos.Size = new System.Drawing.Size(77, 12);
            this.mouseGridPos.TabIndex = 11;
            this.mouseGridPos.Text = "mouseGridPos";
            // 
            // roundInfo
            // 
            this.roundInfo.AutoSize = true;
            this.roundInfo.Location = new System.Drawing.Point(6, 55);
            this.roundInfo.Name = "roundInfo";
            this.roundInfo.Size = new System.Drawing.Size(59, 12);
            this.roundInfo.TabIndex = 10;
            this.roundInfo.Text = "roundInfo";
            // 
            // dicePoints
            // 
            this.dicePoints.AutoSize = true;
            this.dicePoints.Location = new System.Drawing.Point(6, 252);
            this.dicePoints.Name = "dicePoints";
            this.dicePoints.Size = new System.Drawing.Size(65, 12);
            this.dicePoints.TabIndex = 9;
            this.dicePoints.Text = "dicePoints";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 229);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "骰子点数：";
            // 
            // roundInfoList
            // 
            this.roundInfoList.FormattingEnabled = true;
            this.roundInfoList.ItemHeight = 12;
            this.roundInfoList.Location = new System.Drawing.Point(8, 98);
            this.roundInfoList.Name = "roundInfoList";
            this.roundInfoList.Size = new System.Drawing.Size(180, 112);
            this.roundInfoList.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "行动顺序：";
            // 
            // mousePos
            // 
            this.mousePos.AutoSize = true;
            this.mousePos.Location = new System.Drawing.Point(6, 17);
            this.mousePos.Name = "mousePos";
            this.mousePos.Size = new System.Drawing.Size(53, 12);
            this.mousePos.TabIndex = 0;
            this.mousePos.Text = "mousePos";
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
            this.battleScene.CanvasMouseMove += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.battleScene_CanvasMouseMove);
            this.battleScene.SizeChanged += new System.EventHandler(this.battleScene_SizeChanged);
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
        private System.Windows.Forms.Label mousePos;
        private System.Windows.Forms.ListBox roundInfoList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label dicePoints;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label roundInfo;
        private System.Windows.Forms.ContextMenuStrip battleSceneMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuItemMove;
        private System.Windows.Forms.ToolStripMenuItem menuItemUseSkillButton;
        private System.Windows.Forms.ToolStripMenuItem menuItemUseStunt;
        private System.Windows.Forms.ToolStripMenuItem menuItemRoundOver;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemSelectAspect;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewData;
        private System.Windows.Forms.Label mouseGridPos;
    }
}