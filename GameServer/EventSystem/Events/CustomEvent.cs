using GameServer.Core.ScriptSystem;

namespace GameServer.EventSystem.Events {
	public sealed class CustomEvent : Event {
		public struct EventInfo : IEventInfo {
			public bool swallowed;
			public string[] notifyList;
			public object message;

			public bool Swallowed { get => swallowed; set => swallowed = value; }

			public EventInfo(object message, string[] notifyList) {
				this.message = message;
				this.notifyList = notifyList;
				this.swallowed = false;
			}
		}

		private static readonly string[] _idList = {
			"event.custom"
		};

		public override string[] NotifyList => _info.notifyList ?? _idList;

		private EventInfo _info;

		public EventInfo Info { get => _info; set => _info = value; }

		public override IJSContext GetContext() {
			return _info;
		}

		public override void SetContext(IJSContext context) {
			_info = (EventInfo)context;
		}
	}
}
