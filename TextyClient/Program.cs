using GameUtil.Network;
using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
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
		public static List<string> charactersID = new List<string>();

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			connection.ExceptionCaught += Connection_EventCaught;

			string ip;
			ip = Microsoft.VisualBasic.Interaction.InputBox("输入服务器IP");
			if (ip == "") return;

			string id;
			byte[] verificationCode = { };
			var dialogResult = MessageBox.Show("DM(Yes) or Player(No)?", "Choose", MessageBoxButtons.YesNo);
			if (dialogResult == DialogResult.Yes) {
				verificationCode = new byte[] { 0x00, 0x10, 0x20, 0xAB };
				isDM = true;
				id = "DM";
			} else {
				while (true) {
					string dialogResult2 = Microsoft.VisualBasic.Interaction.InputBox("Player1(输入1) or Player2(输入2) or Player3(输入3)?");
					if (dialogResult2 == "1") {
						verificationCode = new byte[] { 0x00, 0x10, 0x20, 0x3B };
						id = "Player1";
						break;
					} else if (dialogResult2 == "2") {
						verificationCode = new byte[] { 0x00, 0x10, 0x20, 0xC5 };
						id = "Player2";
						break;
					} else if (dialogResult2 == "3") {
						verificationCode = new byte[] { 0x00, 0x11, 0x33, 0xFA };
						id = "Player3";
						break;
					}
				}
			}

			var service = NetworkHelper.StartClient(ip);
			var initializer = new NSInitializer(service);
			if (!initializer.ClientInit(verificationCode, connection)) {
				MessageBox.Show("登陆失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				service.TeardownService();
				return;
			}

			var getCharactersRequest = new GetPlayerCharactersMessage();
			connection.Request(getCharactersRequest, result => {
				var resp = result as PlayerCharactersMessage;
				if (resp != null) {
					charactersID.AddRange(resp.charactersID);
				}
			});

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

			mainForm = new MainForm(isDM);
			mainForm.Text += id;
			mainForm.battleSceneForm.Text += id;
			Application.Run(mainForm);
		}

		private static void Connection_EventCaught(object sender, NetworkfExceptionCaughtEventArgs e) {
			MessageBox.Show(e.message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			Application.Exit();
		}
	}
}
