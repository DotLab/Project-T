using GameLib.Core;
using GameLib.Core.ScriptSystem;
using GameLib.EventSystem.Events;
using System;
using System.Collections.Generic;

namespace GameLib.EventSystem {
	public sealed class GameEventBus : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<GameEventBus> {
			private readonly GameEventBus _outer;

			public JSAPI(GameEventBus outer) {
				_outer = outer;
			}

			public IJSAPI<Trigger> createTrigger(string eventID, Action action, bool autoReg = true) {
				try {
					Trigger trigger = new Trigger(eventID, new Command(action), autoReg);
					return (IJSAPI<Trigger>)trigger.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void publishCustomEvent(object message, string[] notifyList) {
				try {
					CustomEvent customEvent = new CustomEvent();
					customEvent.Info = new CustomEvent.EventInfo(message, notifyList);
					_outer.Publish(customEvent);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public void register(IJSAPI<Trigger> trigger) {
				try {
					Trigger originEvent = JSContextHelper.Instance.GetAPIOrigin(trigger);
					_outer.Register(originEvent);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public bool unregister(IJSAPI<Trigger> trigger) {
				try {
					Trigger originEvent = JSContextHelper.Instance.GetAPIOrigin(trigger);
					return _outer.Unregister(originEvent);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public GameEventBus Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private static readonly GameEventBus _instance = new GameEventBus();

		public static GameEventBus Instance => _instance;

		private readonly Dictionary<string, List<Trigger>> _triggerPools;

		private Queue<Trigger> _waitForAdding;
		private Queue<Trigger> _waitForRemoving;
		private bool _publishing = false;

		private GameEventBus() {
			_triggerPools = new Dictionary<string, List<Trigger>>();
			_apiObj = new JSAPI(this);
			_waitForAdding = new Queue<Trigger>();
			_waitForRemoving = new Queue<Trigger>();
		}

		public void Publish(Event e) {
			_publishing = true;
			string[] eventIDs = e.NotifyList;
			foreach (string id in eventIDs) {
				if (_triggerPools.TryGetValue(id, out List<Trigger> triggers)) {
					foreach (Trigger trigger in triggers) {
						if (trigger.Active) {
							e.SendContext();
							trigger.Notify();
							e.RetrieveContext();
						}
						if (e.Swallowed) return;
					}
				}
			}
			_publishing = false;
			foreach (Trigger trigger in _waitForAdding) {
				this.Register(trigger);
			}
			_waitForAdding.Clear();
			foreach (Trigger trigger in _waitForRemoving) {
				this.Unregister(trigger);
			}
			_waitForRemoving.Clear();
		}

		public void Register(Trigger trigger) {
			if (_publishing) {
				_waitForAdding.Enqueue(trigger);
				return;
			}
			if (!_triggerPools.ContainsKey(trigger.BoundEventID)) {
				_triggerPools.Add(trigger.BoundEventID, new List<Trigger>());
			}
			List<Trigger> triggers = _triggerPools[trigger.BoundEventID];
			if (triggers.Contains(trigger)) throw new ArgumentException("Repeated trigger is registered to the event bus.", nameof(trigger));
			triggers.Add(trigger);
		}

		public bool Unregister(Trigger trigger) {
			if (_triggerPools.TryGetValue(trigger.BoundEventID, out List<Trigger> triggers)) {
				if (_publishing) {
					if (triggers.Contains(trigger)) {
						_waitForRemoving.Enqueue(trigger);
						return true;
					} else return false;
				} else return triggers.Remove(trigger);
			} else return false;
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}
}
