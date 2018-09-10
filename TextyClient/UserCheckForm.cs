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
		public UserCheckForm() {
			InitializeComponent();
		}

		public void SetMessage(string message) {
			messageLbl.Text = message;
		}

		private void YesBtn_Click(object sender, EventArgs e) {
			if (int.TryParse(selectionTbx.Text, out int result)) {
				SendResult(result);
			}
		}
		
		private void SendResult(int result) {
			var message = new UserDeterminResultMessage();
			message.result = result;
			Program.connection.SendMessage(message);
			this.Visible = false;
		}

		private void CheckForm_FormClosing(object sender, FormClosingEventArgs e) {
			e.Cancel = true;
		}
	}
}
