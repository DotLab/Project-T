using GameLogic.Core.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bit = Networkf.BitHelper;

namespace GameLogic.Core.Network {
	public sealed class NetworkfMessage : Networkf.Message {
		private readonly Message _innerMessage;

		public Message InnerMessage => _innerMessage;

		public static Networkf.Message ParseMessage(byte[] buf, ref int i) {
			int type = ReadMessageType(buf, ref i);
			var stream = new BitDataInputStream(buf, i);
			var message = Message.New(type);
			message.ReadFrom(stream);
			i = stream.i;
			return new NetworkfMessage(message);
		}

		public NetworkfMessage(Message message) :
			base(message.MessageType) {
			_innerMessage = message;
		}

		public override void WriteTo(byte[] buf, ref int i) {
			var stream = new BitDataOutputStream(buf, i);
			_innerMessage.WriteTo(stream);
			i = stream.i;
		}
	}

	public sealed class NetworkfConnection : Connection {
		private volatile Networkf.NetworkService _service = null;
		private readonly List<Message> _sendingMsgCache = new List<Message>();
		private readonly List<Message> _receivedMsgCache = new List<Message>();
		private readonly Dictionary<int, List<IMessageReceiver>> _messageReceiverDict = new Dictionary<int, List<IMessageReceiver>>();

		public NetworkfConnection() {
			Task.Run((Action)SendCachedMessage);
		}

		public bool HasAppliedService() {
			return _service != null;
		}

		public void ApplyService(Networkf.NetworkService service) {
			if (_service != null) return;
			service.parseMessage = NetworkfMessage.ParseMessage;
			service.OnMessageReceived += OnMessageReceived;
			service.OnServiceTeardown += OnServiceTeardown;
			_service = service;
		}

		public override void AddMessageReceiver(int messageType, IMessageReceiver receiver) {
			if (_messageReceiverDict.ContainsKey(messageType)) {
				var receivers = _messageReceiverDict[messageType];
				if (!receivers.Contains(receiver)) receivers.Add(receiver);
			} else {
				_messageReceiverDict.Add(messageType, new List<IMessageReceiver>() { receiver });
			}
		}

		public override bool RemoveMessageReceiver(int messageType, IMessageReceiver receiver) {
			if (_messageReceiverDict.ContainsKey(messageType)) {
				return _messageReceiverDict[messageType].Remove(receiver);
			}
			return false;
		}

		public override void SendMessage(Message message) {
			if (_service != null) {
				lock (_sendingMsgCache) {
					_sendingMsgCache.Add(message);
					Monitor.Pulse(_sendingMsgCache);
				}
			} else {
				var identifiedMsg = message as IdentifiedMessage;
				if (identifiedMsg != null) {
					if (_messageReceiverDict.TryGetValue(IdentifiedMessage.MESSAGE_TYPE, out List<IMessageReceiver> receivers)) {
						foreach (var receiver in receivers) {
							receiver.MessageReceived(new IdentifiedMessage() { resp = true, guid = identifiedMsg.guid, innerMessage = null });
						}
					}
				}
			}
		}

		public override void UpdateReceiver() {
			lock (_receivedMsgCache) {
				foreach (var message in _receivedMsgCache) {
					if (_messageReceiverDict.ContainsKey(message.MessageType)) {
						foreach (var receiver in _messageReceiverDict[message.MessageType]) {
							receiver.MessageReceived(message);
						}
					}
				}
				_receivedMsgCache.Clear();
			}
		}

		private void SendCachedMessage() {
			while (true) {
				Message message;
				lock (_sendingMsgCache) {
					while (_sendingMsgCache.Count <= 0) Monitor.Wait(_sendingMsgCache);
					message = _sendingMsgCache[0];
					_sendingMsgCache.RemoveAt(0);
				}
				var service = _service;
				if (service != null) {
					int sendingResult = service.SendMessage(new NetworkfMessage(message));
					if (sendingResult == -1) {
						var eventArgs = new NetworkEventCaughtEventArgs() {
							message = "A network error occured during sending data."
						};
						OnEventCaught(eventArgs);
					}
				}
			}
		}

		private void OnMessageReceived(int id, Networkf.Message message) {
			lock (_receivedMsgCache) {
				_receivedMsgCache.Add(((NetworkfMessage)message).InnerMessage);
			}
		}

		private void OnServiceTeardown() {
			var service = _service;
			_service = null;
			service.parseMessage = null;
			service.OnMessageReceived -= OnMessageReceived;
			service.OnServiceTeardown -= OnServiceTeardown;
			var eventArgs = new NetworkEventCaughtEventArgs() {
				message = "Connection is closed."
			};
			OnEventCaught(eventArgs);
		}
	}

	public sealed class BitDataOutputStream : IDataOutputStream {
		public readonly byte[] bytes;
		public int start, i;

		public BitDataOutputStream(byte[] bytes, int i = 0) {
			this.bytes = bytes;
			this.start = this.i = i;
		}

		public void WriteBoolean(bool val) {
			Bit.WriteUInt8(bytes, ref i, (byte)(val ? 1 : 0));
		}

		public void WriteByte(byte val) {
			Bit.WriteUInt8(bytes, ref i, val);
		}

		public void WriteInt32(int val) {
			Bit.WriteInt32(bytes, ref i, val);
		}

		public void WriteSingle(float val) {
			Bit.WriteSingle(bytes, ref i, val);
		}

		public void WriteString(string val) {
			Bit.WriteUInt16(bytes, ref i, (ushort)Bit.GetStringByteCount(val));
			Bit.WriteString(bytes, ref i, val);
		}
	}

	public sealed class BitDataInputStream : IDataInputStream {
		public readonly byte[] bytes;
		public int start, i;

		public BitDataInputStream(byte[] bytes, int i = 0) {
			this.bytes = bytes;
			this.start = this.i = i;
		}

		public bool ReadBoolean() {
			byte val = Bit.ReadUInt8(bytes, ref i);
			return val != 0;
		}

		public byte ReadByte() {
			return Bit.ReadUInt8(bytes, ref i);
		}

		public int ReadInt32() {
			return Bit.ReadInt32(bytes, ref i);
		}

		public float ReadSingle() {
			return Bit.ReadSingle(bytes, ref i);
		}

		public string ReadString() {
			int byteCount = Bit.ReadUInt16(bytes, ref i);
			return Bit.ReadString(bytes, ref i, byteCount);
		}
	}

	public sealed class NSInitializer : IMessageReceiver {
		private sealed class SerRequireVerifyMessage : Networkf.Message {
			public const int KType = 0;
			public SerRequireVerifyMessage() : base(KType) { }
		}

		private sealed class CltVerifyMessage : Networkf.Message {
			public const int KType = 1;

			public byte[] verificationCode;

			public CltVerifyMessage() : base(KType) { }

			public CltVerifyMessage(byte[] buf, ref int i) : base(KType) {
				int byteCount = Bit.ReadInt32(buf, ref i);
				verificationCode = new byte[byteCount];
				for (int j = 0; j < byteCount; ++j) {
					verificationCode[j] = Bit.ReadUInt8(buf, ref i);
				}
			}

			public override void WriteTo(byte[] buf, ref int i) {
				Bit.WriteInt32(buf, ref i, verificationCode.Length);
				foreach (byte b in verificationCode) {
					Bit.WriteUInt8(buf, ref i, b);
				}
			}
		}

		private sealed class SerVerifyResultMessage : Networkf.Message {
			public const int KType = 2;

			public bool result;

			public SerVerifyResultMessage() : base(KType) { }

			public SerVerifyResultMessage(byte[] buf, ref int i) : base(KType) {
				byte val = Bit.ReadUInt8(buf, ref i);
				result = val != 0;
			}

			public override void WriteTo(byte[] buf, ref int i) {
				Bit.WriteUInt8(buf, ref i, (byte)(result ? 1 : 0));
			}
		}

		private sealed class CltReadyMessage : Networkf.Message {
			public const int KType = 3;
			public CltReadyMessage() : base(KType) { }
		}

		private static Networkf.Message ParseInitMessage(byte[] buf, ref int i) {
			var type = Networkf.Message.ReadMessageType(buf, ref i);
			switch (type) {
				case SerRequireVerifyMessage.KType:
					return new SerRequireVerifyMessage();
				case CltVerifyMessage.KType:
					return new CltVerifyMessage(buf, ref i);
				case SerVerifyResultMessage.KType:
					return new SerVerifyResultMessage(buf, ref i);
				case CltReadyMessage.KType:
					return new CltReadyMessage();
				default:
					throw new NotImplementedException();
			}
		}

		private readonly Networkf.NetworkService _service;
		private readonly object mtx = new object();
		private readonly int _retryCount;
		private readonly int _retryDelay;

		private bool _hasReceivedServerHello = false;
		private bool _hasReceivedServerResult = false;
		private bool _verificationResult = false;
		private bool _hasReceivedServerReady = false;

		private byte[] _verificationCode = null;
		private bool _hasReceivedClientReady = false;

		public NSInitializer(Networkf.NetworkService service, int retryCount = 10, int retryDelay = 1000) {
			_service = service;
			service.parseMessage = ParseInitMessage;
			service.OnMessageReceived += OnRawMessageReceived;
			_retryCount = retryCount;
			_retryDelay = retryDelay;
		}

		private void ReleaseService() {
			_service.parseMessage = null;
			_service.OnMessageReceived -= OnRawMessageReceived;
		}

		private void Log(string str) {
			Console.WriteLine(str);
		}

		public void OnRawMessageReceived(int id, Networkf.Message message) {
			lock (mtx) {
				switch (message.type) {
					case SerRequireVerifyMessage.KType:
						_hasReceivedServerHello = true;
						break;
					case CltVerifyMessage.KType: {
							var cltMsg = (CltVerifyMessage)message;
							_verificationCode = cltMsg.verificationCode;
						}
						break;
					case SerVerifyResultMessage.KType: {
							var serMsg = (SerVerifyResultMessage)message;
							_verificationResult = serMsg.result;
							_hasReceivedServerResult = true;
						}
						break;
					case CltReadyMessage.KType:
						_hasReceivedClientReady = true;
						break;
					default:
						throw new NotImplementedException();
				}
				Monitor.PulseAll(mtx);
			}
		}

		public void MessageReceived(Message message) {
			var serReadyMsg = message as ServerReadyMessage;
			if (serReadyMsg != null) _hasReceivedServerReady = true;
		}

		public bool ClientInit(byte[] verificationCode, NetworkfConnection applyTo) {
			Log("waiting server hello...");
			lock (mtx) {
				while (!_hasReceivedServerHello) Monitor.Wait(mtx);
			}
			Log("server hello received");
			var verifyMsg = new CltVerifyMessage();
			verifyMsg.verificationCode = verificationCode;
			_service.SendMessage(verifyMsg);
			Log("waiting verification result...");
			lock (mtx) {
				while (!_hasReceivedServerResult) Monitor.Wait(mtx);
			}
			Log("verification result received");
			if (_verificationResult) {
				ReleaseService();
				applyTo.ApplyService(_service);
				applyTo.AddMessageReceiver(ServerReadyMessage.MESSAGE_TYPE, this);
				_service.SendMessage(new CltReadyMessage());
				Log("client ready has sent");
				Log("waiting server ready...");
				while (!_hasReceivedServerReady) {
					Thread.Sleep(100);
					applyTo.UpdateReceiver();
				}
				Log("server ready received");
				applyTo.RemoveMessageReceiver(ServerReadyMessage.MESSAGE_TYPE, this);
				return true;
			} else return false;
		}

		public byte[] ServerRequireClientVerify() {
			var serMsg = new SerRequireVerifyMessage();
			int tryCount = 0;
			Log("waiting client verify...");
			lock (mtx) {
				while (_verificationCode == null && tryCount < _retryCount) {
					_service.SendMessage(serMsg);
					++tryCount;
					Monitor.Wait(mtx, _retryDelay);
				}
			}
			if (_verificationCode == null) {
				Log("broken client");
				ReleaseService();
				_service.TeardownService();
				return null;
			}
			Log("client verification received");
			return _verificationCode;
		}

		public void ServerApplyConnection(NetworkfConnection applyTo) {
			var serMsg = new SerVerifyResultMessage();
			serMsg.result = applyTo != null;
			_service.SendMessage(serMsg);
			Log("verification result has sent");
			if (applyTo != null) {
				int tryCount = 0;
				Log("waiting client ready...");
				lock (mtx) {
					while (!_hasReceivedClientReady && tryCount < _retryCount) {
						Monitor.Wait(mtx, _retryDelay);
						++tryCount;
					}
				}
				if (!_hasReceivedClientReady) {
					Log("broken client");
					ReleaseService();
					_service.TeardownService();
					return;
				}
				Log("client ready received");
				ReleaseService();
				applyTo.ApplyService(_service);
				applyTo.SendMessage(new ServerReadyMessage());
			} else {
				ReleaseService();
				_service.TeardownService();
			}
		}
	}
}
