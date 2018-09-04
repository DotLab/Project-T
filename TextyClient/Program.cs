using GameLib.Utilities.Network;
using GameLib.Utilities.Network.ClientMessages;
using GameLib.Utilities.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetworkHelper = Networkf.NetworkHelper;

namespace TextyClient {
	public struct CharacterPropertyInfo {
		public string ownerID;
		public string propertyID;
		public string name;
		public string description;
		public string extraMessage;

		public override string ToString() {
			return name + " " + extraMessage;
		}
	}

	static class Program {
		public static bool isDM = false;
		public static NetworkfConnection connection = new NetworkfConnection();
		public static List<CharacterPropertyInfo> skillTypes = new List<CharacterPropertyInfo>();
		public static MainForm mainForm;
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main() {
			connection.EventCaught += Connection_EventCaught;

			string id;
			byte[] verificationCode = { };
			var dialogResult = MessageBox.Show("DM(Yes) or Player(No)?", "Choose", MessageBoxButtons.YesNo);
			if (dialogResult == DialogResult.Yes) {
				verificationCode = new byte[] { 0x00, 0x10, 0x20, 0xAB };
				isDM = true;
				id = "DM";
			} else {
				var dialogResult2 = MessageBox.Show("Player1(Yes) or Player2(No)?", "Choose", MessageBoxButtons.YesNo);
				if (dialogResult2 == DialogResult.Yes) {
					verificationCode = new byte[] { 0x00, 0x10, 0x20, 0x3B };
					id = "Player1";
				} else {
					verificationCode = new byte[] { 0x00, 0x10, 0x20, 0xC5 };
					id = "Player2";
				}
			}

			var service = NetworkHelper.StartClient("127.0.0.1");
			var initializer = new NSInitializer(service);
			if (!initializer.ClientInit(verificationCode, connection)) {
				MessageBox.Show("登陆失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				service.TeardownService();
				return;
			}
			
			var getAllSkillTypesRequest = new GetSkillTypeListMessage();
			connection.Request(getAllSkillTypesRequest, result => {
				var resp = result as SkillTypeListDataMessage;
				if (resp != null) {
					foreach (var skillType in resp.skillTypes) {
						skillTypes.Add(new CharacterPropertyInfo() {
							name = skillType.name,
							propertyID = skillType.id
						});
					}
				}
			});

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			mainForm = new MainForm(isDM);
			mainForm.Text += id;
			mainForm.battleSceneForm.Text += id;
			Application.Run(mainForm);
		}

		private static void Connection_EventCaught(object sender, NetworkEventCaughtEventArgs e) {
			MessageBox.Show(e.message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			Application.Exit();
		}
	}
}
