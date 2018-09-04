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
	public partial class CheckForm : Form {
		private bool _dmCheck = false;

		public bool DMCheck { get => _dmCheck; set => _dmCheck = value; }

		public CheckForm() {
			InitializeComponent();
		}

		public void SetMessage(string message) {
			messageLbl.Text = message;
		}

		private void YesBtn_Click(object sender, EventArgs e) {
			SendResult(false);
		}

		private void NoBtn_Click(object sender, EventArgs e) {
			SendResult(true);
		}

		private void SendResult(bool reject) {
			if (_dmCheck) {
				var message = new DMCheckResultMessage();
				message.result = !reject;
				Program.connection.SendMessage(message);
			} else {
				var message = new UserDeterminResultMessage();
				message.result = reject ? 1 : 0;
				Program.connection.SendMessage(message);
			}
			this.Visible = false;
		}

		private void CheckForm_FormClosing(object sender, FormClosingEventArgs e) {
			SendResult(true);
			e.Cancel = true;
		}
	}
}
