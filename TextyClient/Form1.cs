using GameLib.Utilities.Network;
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
	public partial class Form1 : Form, IMessageReceiver {
		public readonly BattleSceneForm battleSceneForm = new BattleSceneForm();
		public readonly StorySceneForm storySceneForm = new StorySceneForm();
		public readonly Timer connectionUpdater = new Timer();

		private readonly DMCheckForm _dmCheckForm = new DMCheckForm();
		private readonly bool _isDM;

		public Form1(bool isDM) {
			InitializeComponent();
			this.WindowState = FormWindowState.Minimized;
			
			connectionUpdater.Interval = 100;
			connectionUpdater.Tick += new EventHandler(this.ConnectionUpdate);
			connectionUpdater.Start();

			_isDM = isDM;
			if (isDM) {
				Program.connection.AddMessageReceiver(DMCheckMessage.MESSAGE_TYPE, this);
			}
		}

		public void MessageReceived(GameLib.Utilities.Network.Message message) {
			if (_isDM) {
				var msg = (DMCheckMessage)message;
				_dmCheckForm.SetMessage(msg.text);
				_dmCheckForm.Visible = true;
				_dmCheckForm.Activate();
			}
		}

		private void ConnectionUpdate(object sender, EventArgs e) {
			Program.connection.UpdateReceiver();
		}

	}
}
