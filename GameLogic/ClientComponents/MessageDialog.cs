using GameLogic.Core.Network;
using GameLogic.Utilities;

namespace GameLogic.ClientComponents {
	public class MessageDialog : ClientComponent {
		public override void MessageReceived(Message message) { }

		public MessageDialog(Connection connection, User owner) :
			base(connection, owner) {

		}

		public void Show(string text) {

		}

	}
}
