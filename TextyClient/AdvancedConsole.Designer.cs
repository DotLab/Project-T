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
			this.inputTbx = new System.Windows.Forms.TextBox();
			this.outputTbx = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// inputTbx
			// 
			this.inputTbx.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.inputTbx.Location = new System.Drawing.Point(0, 129);
			this.inputTbx.Name = "inputTbx";
			this.inputTbx.Size = new System.Drawing.Size(150, 21);
			this.inputTbx.TabIndex = 1;
			this.inputTbx.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputTbx_KeyDown);
			// 
			// outputTbx
			// 
			this.outputTbx.Dock = System.Windows.Forms.DockStyle.Fill;
			this.outputTbx.Location = new System.Drawing.Point(0, 0);
			this.outputTbx.Multiline = true;
			this.outputTbx.Name = "outputTbx";
			this.outputTbx.ReadOnly = true;
			this.outputTbx.Size = new System.Drawing.Size(150, 129);
			this.outputTbx.TabIndex = 2;
			// 
			// AdvancedConsole
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.outputTbx);
			this.Controls.Add(this.inputTbx);
			this.Name = "AdvancedConsole";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox inputTbx;
		private System.Windows.Forms.TextBox outputTbx;
	}
}
