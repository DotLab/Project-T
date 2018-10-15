using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
using GameUtil.Network.Streamable;
using System;
using System.Collections.Generic;

namespace GameUtil.Network {
	public abstract class Message : IStreamable {
		#region Message Creator
		public static Message New(int messageType) {
			switch (messageType) {
				case IdentifiedMessage.MESSAGE_TYPE:
					return new IdentifiedMessage();

				// server message
				

				// client message
				

				default:
					throw new NotImplementedException();
			}
		}
		#endregion

		public abstract void WriteTo(IDataOutputStream stream);
		public abstract void ReadFrom(IDataInputStream stream);

		public abstract int MessageType { get; }
	}

	public sealed class IdentifiedMessage : Message {
		public Message innerMessage;
		public Guid guid;
		public bool resp;

		public const int MESSAGE_TYPE = 0;
		public override int MessageType { get { return MESSAGE_TYPE; } }
		public int InnerMsgType { get { return innerMessage == null ? 0 : innerMessage.MessageType; } }

		public IdentifiedMessage() { }

		public IdentifiedMessage(Message message) {
			this.innerMessage = message;
			this.guid = Guid.NewGuid();
			this.resp = false;
		}

		public override void ReadFrom(IDataInputStream stream) {
			int innerType = stream.ReadInt32();
			if (innerType == 0) {
				innerMessage = null;
			} else {
				innerMessage = New(innerType);
				innerMessage.ReadFrom(stream);
			}
			guid = InputStreamHelper.ReadGuid(stream);
			resp = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(this.InnerMsgType);
			if (innerMessage != null) innerMessage.WriteTo(stream);
			OutputStreamHelper.WriteGuid(stream, guid);
			stream.WriteBoolean(resp);
		}
	}

	public interface IMessageReceiver {
		void MessageReceived(Message message);
	}

	public interface IRequestHandler {
		Message MakeResponse(Message request);
	}

	public abstract class Connection : IMessageReceiver {
		private readonly Dictionary<Guid, Action<Message>> _callbackDict = new Dictionary<Guid, Action<Message>>();
		private readonly Dictionary<int, IRequestHandler> _reqHandlerDict = new Dictionary<int, IRequestHandler>();

		public void MessageReceived(Message message) {
			var identifiedMsg = (IdentifiedMessage)message;
			if (identifiedMsg.resp) {
				Action<Message> callback;
				if (_callbackDict.TryGetValue(identifiedMsg.guid, out callback)) {
					callback(identifiedMsg.innerMessage);
					_callbackDict.Remove(identifiedMsg.guid);
				}
			} else {
				Message resp = null;
				IRequestHandler handler;
				if (_reqHandlerDict.TryGetValue(identifiedMsg.InnerMsgType, out handler)) {
					resp = handler.MakeResponse(identifiedMsg.innerMessage);
				}
				var respWrapper = new IdentifiedMessage() { innerMessage = resp, guid = identifiedMsg.guid, resp = true };
				SendMessage(respWrapper);
			}
		}

		public void Request(Message message, Action<Message> callback) {
			var identifiedMsg = new IdentifiedMessage(message);
			while (_callbackDict.ContainsKey(identifiedMsg.guid)) identifiedMsg.guid = Guid.NewGuid();
			_callbackDict.Add(identifiedMsg.guid, callback);
			SendMessage(identifiedMsg);
		}

		public void SetRequestHandler(int messageType, IRequestHandler handler) {
			_reqHandlerDict[messageType] = handler;
		}

		public Connection() {
			AddMessageReceiver(IdentifiedMessage.MESSAGE_TYPE, this);
		}

		public abstract void SendMessage(Message message);
		public abstract void AddMessageReceiver(int messageType, IMessageReceiver receiver);
		public abstract bool RemoveMessageReceiver(int messageType, IMessageReceiver receiver);
		public abstract void FlushReceivingBuffer();
		public abstract bool Available();
	}
}