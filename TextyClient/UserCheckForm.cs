using GameUtil.Network.ClientMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextyClient {
	public partial class UserCheckForm : Form {
		public int ReturnValue { get; set; }

		public UserCheckForm() {
			InitializeComponent();
		}

		public void SetMessage(string message) {
			messageLbl.Text = message;
		}

		private void YesBtn_Click(object sender, EventArgs e) {
			if (int.TryParse(selectionTbx.Text, out int result)) {
				this.ReturnValue = result;
				this.DialogResult = DialogResult.OK;
				this.Close();
			} else {
				MessageBox.Show("请输入数字", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
