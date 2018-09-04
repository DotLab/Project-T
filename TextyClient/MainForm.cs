using GameLib.Utilities.Network;
using GameLib.Utilities.Network.ClientMessages;
using GameLib.Utilities.Network.ServerMessages;
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
		private readonly CheckForm _dmCheckForm = new CheckForm();
		private readonly bool _isDM;

		public AdvancedConsole AdvancedConsole => advancedConsole;

		public MainForm(bool isDM) {
			InitializeComponent();

			_isDM = isDM;
			if (isDM) {
				Program.connection.AddMessageReceiver(UserDeterminMessage.MESSAGE_TYPE, this);
				Program.connection.AddMessageReceiver(DMCheckMessage.MESSAGE_TYPE, this);
			}
		}

		public void MessageReceived(GameLib.Utilities.Network.Message message) {
			switch (message.MessageType) {
				case DMCheckMessage.MESSAGE_TYPE: {
						if (_isDM) {
							var msg = (DMCheckMessage)message;
							_dmCheckForm.SetMessage(msg.text);
							_dmCheckForm.DMCheck = true;
							_dmCheckForm.Visible = true;
							_dmCheckForm.Activate();
						}
					}
					break;
				case UserDeterminMessage.MESSAGE_TYPE: {
						var msg = (UserDeterminMessage)message;
						_dmCheckForm.SetMessage(msg.text);
						_dmCheckForm.DMCheck = false;
						_dmCheckForm.Visible = true;
						_dmCheckForm.Activate();
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
