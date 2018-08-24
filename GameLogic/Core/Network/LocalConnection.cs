using System;
using System.IO;

namespace GameLogic.Core.Network {
	public class LocalConnection : Connection {
		private MemoryStream _memoryStream;

		public LocalConnection(bool server) {
			_memoryStream = new MemoryStream();
			throw new NotImplementedException();
		}

		public override void AddMessageReceiver(int messageType, IMessageReceiver receiver) {
			throw new NotImplementedException();
		}

		public override bool RemoveMessageReceiver(int messageType, IMessageReceiver receiver) {
			throw new NotImplementedException();
		}

		public override void SendMessage(Message message) {
			throw new NotImplementedException();
		}

		public override void UpdateReceiver() {
			throw new NotImplementedException();
		}
	}
}
