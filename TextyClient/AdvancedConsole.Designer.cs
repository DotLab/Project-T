namespace TextyClient
{
    partial class AdvancedConsole
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.gameScene = new TextyClient.GameScene();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // gameScene
            // 
            this.gameScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameScene.Location = new System.Drawing.Point(0, 0);
            this.gameScene.Name = "gameScene";
            this.gameScene.Size = new System.Drawing.Size(150, 129);
            this.gameScene.TabIndex = 0;
            this.gameScene.ViewerRectangleLeft = 0;
            this.gameScene.ViewerRectangleLeftTop = new System.Drawing.Point(0, 0);
            this.gameScene.ViewerRectangleTop = 0;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 129);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(150, 21);
            this.textBox1.TabIndex = 1;
            // 
            // AdvancedConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gameScene);
            this.Controls.Add(this.textBox1);
            this.Name = "AdvancedConsole";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GameScene gameScene;
        private System.Windows.Forms.TextBox textBox1;
    }
}
