using GameLogic.Utilities.ScriptSystem;

namespace GameLogic.EventSystem {
	public abstract class Event : IJSContextProvider {
		protected bool _swallowed = false;

		public bool Swallowed { get => _swallowed; set => _swallowed = value; }

		public void SendContext() {
			IEventInfo eventInfo = (IEventInfo)this.GetContext();
			eventInfo.setSwallowed(_swallowed);
			this.SetContext(eventInfo);
			JSEngineManager.Engine.SynchronizeContext("$__eventArgs__", this);
		}

		public void RetrieveContext() {
			JSEngineManager.Engine.SynchronizeContext("$__eventArgs__", this);
			JSEngineManager.Engine.RemoveContext("$__eventArgs__");
			IEventInfo eventInfo = (IEventInfo)this.GetContext();
			_swallowed = eventInfo.isSwallowed();
		}

		public abstract string[] NotifyList { get; }

		public abstract IJSContext GetContext();
		public abstract void SetContext(IJSContext context);
	}

	public interface IEventInfo : IJSContext {
		void setSwallowed(bool value);
		bool isSwallowed();
	}
}
