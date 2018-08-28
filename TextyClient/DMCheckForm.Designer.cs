namespace TextyClient {
	partial class DMCheckForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.messageLbl = new System.Windows.Forms.Label();
			this.YesBtn = new System.Windows.Forms.Button();
			this.NoBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// messageLbl
			// 
			this.messageLbl.AutoSize = true;
			this.messageLbl.Location = new System.Drawing.Point(60, 41);
			this.messageLbl.Name = "messageLbl";
			this.messageLbl.Size = new System.Drawing.Size(0, 12);
			this.messageLbl.TabIndex = 0;
			// 
			// YesBtn
			// 
			this.YesBtn.Location = new System.Drawing.Point(141, 83);
			this.YesBtn.Name = "YesBtn";
			this.YesBtn.Size = new System.Drawing.Size(75, 23);
			this.YesBtn.TabIndex = 1;
			this.YesBtn.Text = "是";
			this.YesBtn.UseVisualStyleBackColor = true;
			this.YesBtn.Click += new System.EventHandler(this.YesBtn_Click);
			// 
			// NoBtn
			// 
			this.NoBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.NoBtn.Location = new System.Drawing.Point(271, 83);
			this.NoBtn.Name = "NoBtn";
			this.NoBtn.Size = new System.Drawing.Size(75, 23);
			this.NoBtn.TabIndex = 2;
			this.NoBtn.Text = "否";
			this.NoBtn.UseVisualStyleBackColor = true;
			this.NoBtn.Click += new System.EventHandler(this.NoBtn_Click);
			// 
			// DMCheckForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(495, 127);
			this.Controls.Add(this.NoBtn);
			this.Controls.Add(this.YesBtn);
			this.Controls.Add(this.messageLbl);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DMCheckForm";
			this.Text = "DM确认";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DMCheckForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label messageLbl;
		private System.Windows.Forms.Button YesBtn;
		private System.Windows.Forms.Button NoBtn;
	}
}