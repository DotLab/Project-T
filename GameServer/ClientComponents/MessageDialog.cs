using GameLib.Core;
using GameLib.Utilities.Network;

namespace GameLib.ClientComponents {
	public class MessageDialog : ClientComponent {
		public override void MessageReceived(Message message) { }

		public MessageDialog(Connection connection, User owner) :
			base(connection, owner) {

		}

		public void Show(string text) {

		}

	}
}
