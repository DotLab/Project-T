namespace TextyClient {
	partial class UserCheckForm {
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
			this.selectionTbx = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// messageLbl
			// 
			this.messageLbl.AutoSize = true;
			this.messageLbl.Location = new System.Drawing.Point(12, 9);
			this.messageLbl.Name = "messageLbl";
			this.messageLbl.Size = new System.Drawing.Size(0, 12);
			this.messageLbl.TabIndex = 0;
			// 
			// YesBtn
			// 
			this.YesBtn.Location = new System.Drawing.Point(403, 128);
			this.YesBtn.Name = "YesBtn";
			this.YesBtn.Size = new System.Drawing.Size(88, 23);
			this.YesBtn.TabIndex = 1;
			this.YesBtn.Text = "确认";
			this.YesBtn.UseVisualStyleBackColor = true;
			this.YesBtn.Click += new System.EventHandler(this.YesBtn_Click);
			// 
			// selectionTbx
			// 
			this.selectionTbx.Location = new System.Drawing.Point(12, 130);
			this.selectionTbx.Name = "selectionTbx";
			this.selectionTbx.Size = new System.Drawing.Size(383, 21);
			this.selectionTbx.TabIndex = 2;
			// 
			// UserCheckForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(503, 163);
			this.ControlBox = false;
			this.Controls.Add(this.selectionTbx);
			this.Controls.Add(this.YesBtn);
			this.Controls.Add(this.messageLbl);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UserCheckForm";
			this.Text = "确认";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label messageLbl;
		private System.Windows.Forms.Button YesBtn;
		private System.Windows.Forms.TextBox selectionTbx;
	}
}