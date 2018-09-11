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
	public partial class DMCheckForm : Form {
		public DMCheckForm() {
			InitializeComponent();
		}

		public void SetMessage(string message) {
			messageLbl.Text = message;
		}

		private void YesBtn_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Yes;
			this.Close();
		}

		private void NoBtn_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.No;
			this.Close();
		}
	}
}
