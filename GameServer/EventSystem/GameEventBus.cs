using GameServer.Core;
using GameServer.Core.ScriptSystem;
using GameServer.EventSystem.Events;
using System;
using System.Collections.Generic;

namespace GameServer.EventSystem {
	public sealed class GameEventBus : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<GameEventBus> {
			private readonly GameEventBus _outer;

			public JSAPI(GameEventBus outer) {
				_outer = outer;
			}

			public IJSAPI<Trigger> createTrigger(string eventID, Action action) {
				try {
					Trigger trigger = new Trigger(eventID, new Command(action));
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
		private Queue<Event> _waitForPublishing;
		private bool _publishing = false;
		private bool _publishingCachedEvents = false;

		private GameEventBus() {
			_triggerPools = new Dictionary<string, List<Trigger>>();
			_apiObj = new JSAPI(this);
			_waitForAdding = new Queue<Trigger>();
			_waitForRemoving = new Queue<Trigger>();
			_waitForPublishing = new Queue<Event>();
		}

		public void Publish(Event e) {
			if (_publishing) {
				_waitForPublishing.Enqueue(e);
			} else {
				_publishing = true;
				string[] eventIDs = e.NotifyList;
				foreach (string id in eventIDs) {
					if (_triggerPools.TryGetValue(id, out List<Trigger> triggers)) {
						foreach (Trigger trigger in triggers) {
							if (trigger.Active) {
								e.TransmitContext();
								trigger.Notify();
								e.RetrieveContext();
								if (e.Swallowed) return;
							}
						}
					}
				}
				_publishing = false;
				foreach (var trigger in _waitForAdding) {
					this.Register(trigger);
				}
				_waitForAdding.Clear();
				foreach (var trigger in _waitForRemoving) {
					this.Unregister(trigger);
				}
				_waitForRemoving.Clear();
				if (!_publishingCachedEvents) {
					_publishingCachedEvents = true;
					while (_waitForPublishing.Count > 0) {
						var e1 = _waitForPublishing.Dequeue();
						this.Publish(e1);
					}
					_publishingCachedEvents = false;
				}
			}
		}

		public void Register(Trigger trigger) {
			if (_publishing) {
				if (_waitForAdding.Contains(trigger)) throw new ArgumentException("Repeated trigger is registered to the event bus.", nameof(trigger));
				_waitForAdding.Enqueue(trigger);
				return;
			}
			if (!_triggerPools.ContainsKey(trigger.BoundEventID)) {
				_triggerPools.Add(trigger.BoundEventID, new List<Trigger>() { trigger });
			} else {
				List<Trigger> triggers = _triggerPools[trigger.BoundEventID];
				if (triggers.Contains(trigger)) throw new ArgumentException("Repeated trigger is registered to the event bus.", nameof(trigger));
				triggers.Add(trigger);
			}
		}

		public bool Unregister(Trigger trigger) {
			if (_triggerPools.TryGetValue(trigger.BoundEventID, out List<Trigger> triggers)) {
				if (_publishing) {
					if (triggers.Contains(trigger) && !_waitForRemoving.Contains(trigger)) {
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
