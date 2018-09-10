using GameServer.Core.ScriptSystem;

namespace GameServer.EventSystem {
	public abstract class Event : IJSContextProvider {
		protected bool _swallowed = false;

		public bool Swallowed { get => _swallowed; set => _swallowed = value; }

		public void TransmitContext() {
			IEventInfo eventInfo = (IEventInfo)this.GetContext();
			eventInfo.Swallowed = _swallowed;
			this.SetContext(eventInfo);
			JSEngineManager.Engine.SynchronizeContext("$__eventArgs__", this);
		}

		public void RetrieveContext() {
			JSEngineManager.Engine.SynchronizeContext("$__eventArgs__", this);
			JSEngineManager.Engine.RemoveContext("$__eventArgs__");
			IEventInfo eventInfo = (IEventInfo)this.GetContext();
			_swallowed = eventInfo.Swallowed;
		}

		public abstract string[] NotifyList { get; }

		public abstract IJSContext GetContext();
		public abstract void SetContext(IJSContext context);
	}

	public interface IEventInfo : IJSContext {
		bool Swallowed { get; set; }
	}
}
