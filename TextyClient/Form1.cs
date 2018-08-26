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
	public partial class Form1 : Form {
		public BattleSceneForm battleSceneForm = new BattleSceneForm();
		public StorySceneForm storySceneForm = new StorySceneForm();
		public Timer connectionUpdater = new Timer();

		public Form1() {
			InitializeComponent();
			this.WindowState = FormWindowState.Minimized;
			
			connectionUpdater.Interval = 100;
			connectionUpdater.Tick += new EventHandler(this.ConnectionUpdate);
			connectionUpdater.Start();
		}

		private void ConnectionUpdate(object sender, EventArgs e) {
			Program.connection.UpdateReceiver();
		}

	}
}
