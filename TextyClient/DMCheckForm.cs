using GameLib.Utilities.Network.ClientMessages;
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

		private void DMCheckForm_FormClosed(object sender, FormClosedEventArgs e) {
			SendResult(true);
		}

		private void YesBtn_Click(object sender, EventArgs e) {
			SendResult(false);
		}

		private void NoBtn_Click(object sender, EventArgs e) {
			SendResult(true);
		}

		private void SendResult(bool reject) {
			var message = new DMCheckResultMessage();
			message.result = !reject;
			Program.connection.SendMessage(message);
		}
	}
}
