using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem.Events;

namespace GameLogic.EventSystem
{
    public sealed class GameEventBus : IJSContextProvider
    {
        #region Javascript API class
        private sealed class JSAPI : IJSAPI<GameEventBus>
        {
            private readonly GameEventBus _outer;

            public JSAPI(GameEventBus outer)
            {
                _outer = outer;
            }
            
            public IJSAPI<Trigger> createTrigger(string eventID, Action action, bool autoReg = true)
            {
                try
                {
                    Trigger trigger = new Trigger(eventID, new Command(action), autoReg);
                    return (IJSAPI<Trigger>)trigger.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void publishCustomEvent(object message, string[] notifyList)
            {
                try
                {
                    CustomEvent customEvent = new CustomEvent();
                    customEvent.Info = new CustomEvent.EventInfo(message, notifyList);
                    _outer.Publish(customEvent);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void register(IJSAPI<Trigger> trigger)
            {
                try
                {
                    Trigger originEvent = JSContextHelper.Instance.GetAPIOrigin(trigger);
                    _outer.Register(originEvent);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public bool unregister(IJSAPI<Trigger> trigger)
            {
                try
                {
                    Trigger originEvent = JSContextHelper.Instance.GetAPIOrigin(trigger);
                    return _outer.Unregister(originEvent);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public GameEventBus Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly JSAPI _apiObj;

        private static readonly GameEventBus _instance = new GameEventBus();

        public static GameEventBus Instance => _instance;

        private readonly Dictionary<string, List<Trigger>> _triggerPools;

        private GameEventBus()
        {
            _triggerPools = new Dictionary<string, List<Trigger>>();
            _apiObj = new JSAPI(this);
        }

        public void Publish(Event e)
        {
            string[] eventIDs = e.NotifyList;
            foreach (string id in eventIDs)
            {
                if (_triggerPools.TryGetValue(id, out List<Trigger> triggers))
                {
                    foreach (Trigger trigger in triggers)
                    {
                        if (trigger.Active)
                        {
                            e.SendContext();
                            trigger.Notify();
                            e.RetrieveContext();
                        }
                        if (e.Swallowed) return;
                    }
                }
            }
        }

        public void Register(Trigger trigger)
        {
            if (!_triggerPools.ContainsKey(trigger.BoundEventID))
            {
                _triggerPools.Add(trigger.BoundEventID, new List<Trigger>());
            }
            List<Trigger> triggers = _triggerPools[trigger.BoundEventID];
            if (triggers.Contains(trigger)) throw new ArgumentException("Repeated trigger is registered to the event bus.", nameof(trigger));
            triggers.Add(trigger);
        }

        public bool Unregister(Trigger trigger)
        {
            List<Trigger> triggers;
            if (_triggerPools.TryGetValue(trigger.BoundEventID, out triggers))
            {
                return triggers.Remove(trigger);
            }
            return false;
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
