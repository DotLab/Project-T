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
	public partial class MainForm : Form, IMessageReceiver {
		public readonly BattleSceneForm battleSceneForm = new BattleSceneForm();
		private readonly DMCheckForm _dmCheckForm = new DMCheckForm();
		private readonly UserCheckForm _userCheckForm = new UserCheckForm();
		private readonly bool _isDM;

		public AdvancedConsole AdvancedConsole => advancedConsole;

		public MainForm(bool isDM) {
			InitializeComponent();

			Program.connection.AddMessageReceiver(UserDeterminMessage.MESSAGE_TYPE, this);

			_isDM = isDM;
			if (isDM) {
				Program.connection.AddMessageReceiver(DMCheckMessage.MESSAGE_TYPE, this);
			}
		}

		public void MessageReceived(GameUtil.Network.Message message) {
			switch (message.MessageType) {
				case DMCheckMessage.MESSAGE_TYPE: {
						if (_isDM) {
							var msg = (DMCheckMessage)message;
							_dmCheckForm.SetMessage(msg.text);
							_dmCheckForm.Visible = true;
							_dmCheckForm.Activate();
						}
					}
					break;
				case UserDeterminMessage.MESSAGE_TYPE: {
						var msg = (UserDeterminMessage)message;
						_userCheckForm.SetMessage(msg.text);
						_userCheckForm.Visible = true;
						_userCheckForm.Activate();
					}
					break;
				default:
					break;
			}
			
		}
		
		private void MainForm_Load(object sender, EventArgs e) {
			connectionTimer.Enabled = true;
			Program.connection.SendMessage(new ClientInitMessage());
		}

		private void connectionTimer_Tick(object sender, EventArgs e) {
			Program.connection.UpdateReceiver();
		}
	}
}
