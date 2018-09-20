using GameUtil.Network;
using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
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
	public partial class MainForm : Form, IRequestHandler {
		public readonly BattleSceneForm battleSceneForm = new BattleSceneForm();
		private readonly DMCheckForm _dmCheckForm = new DMCheckForm();
		private readonly UserCheckForm _userCheckForm = new UserCheckForm();
		private readonly bool _isDM;

		public AdvancedConsole AdvancedConsole => advancedConsole;

		public MainForm(bool isDM) {
			InitializeComponent();

			Program.connection.SetRequestHandler(UserDeterminMessage.MESSAGE_TYPE, this);

			_isDM = isDM;
			if (isDM) {
				Program.connection.SetRequestHandler(DMCheckMessage.MESSAGE_TYPE, this);
			}
		}

		public GameUtil.Network.Message MakeResponse(GameUtil.Network.Message request) {
			switch (request.MessageType) {
				case DMCheckMessage.MESSAGE_TYPE: {
						if (_isDM) {
							var msg = (DMCheckMessage)request;
							_dmCheckForm.SetMessage(msg.text);
							connectionTimer.Enabled = false;
							var dialogResult = _dmCheckForm.ShowDialog();
							connectionTimer.Enabled = true;
							var resp = new DMCheckResultMessage();
							resp.result = dialogResult == DialogResult.Yes;
							return resp;
						}
					}
					break;
				case UserDeterminMessage.MESSAGE_TYPE: {
						var msg = (UserDeterminMessage)request;
						_userCheckForm.SetMessage(msg.text);
						connectionTimer.Enabled = false;
						_userCheckForm.ShowDialog();
						connectionTimer.Enabled = true;
						var resp = new UserDeterminResultMessage();
						resp.result = _userCheckForm.ReturnValue;
						return resp;
					}
				default:
					break;
			}
			return null;
		}

		private void MainForm_Load(object sender, EventArgs e) {
			connectionTimer.Enabled = true;
			Program.connection.SendMessage(new ClientInitMessage());
		}

		private void connectionTimer_Tick(object sender, EventArgs e) {
			Program.connection.UpdateReceivers();
		}
	}
}
