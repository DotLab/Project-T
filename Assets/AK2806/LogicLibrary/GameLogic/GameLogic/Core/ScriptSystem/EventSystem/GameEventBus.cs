using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.ScriptSystem.Event
{
    public sealed class GameEventBus : IJSContextProvider
    {
        #region Javascript API class
        private sealed class API : IJSAPI
        {
            private readonly GameEventBus _outer;

            public API(GameEventBus outer)
            {
                _outer = outer;
            }
            
            public IJSAPI createTrigger(string eventID, string jscode, bool autoReg = true)
            {
                try
                {
                    Trigger trigger = new Trigger(eventID, new Command(jscode), autoReg);
                    return (IJSAPI)trigger.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void publish(IJSAPI e)
            {
                try
                {
                    IEvent originEvent = (IEvent)JSContextHelper.Instance.GetAPIOrigin(e);
                    _outer.Publish(originEvent);
                }
                catch (Exception err)
                {
                    JSEngineManager.Engine.Log(err.Message);
                }
            }

            public void register(IJSAPI trigger)
            {
                try
                {
                    Trigger originEvent = (Trigger)JSContextHelper.Instance.GetAPIOrigin(trigger);
                    _outer.Register(originEvent);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public bool unregister(IJSAPI trigger)
            {
                try
                {
                    Trigger originEvent = (Trigger)JSContextHelper.Instance.GetAPIOrigin(trigger);
                    return _outer.Unregister(originEvent);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public IJSContextProvider Origin(JSContextHelper proof)
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
        private readonly API _apiObj;

        private static readonly GameEventBus _instance = new GameEventBus();

        public static GameEventBus Instance => _instance;

        private readonly Dictionary<string, List<Trigger> > _triggerPools;

        private GameEventBus()
        {
            _triggerPools = new Dictionary<string, List<Trigger> >();
            _apiObj = new API(this);
        }

        public void Publish(IEvent e)
        {
            string[] eventIDs = e.NotifyList;
            e.SendContext(JSEngineManager.Engine);
            foreach (string id in eventIDs)
            {
                List<Trigger> triggers;
                if (_triggerPools.TryGetValue(id, out triggers))
                {
                    foreach (Trigger trigger in triggers)
                    {
                        if (trigger.Active)
                        {
                            trigger.Notify(JSEngineManager.Engine);
                        }
                    }
                }
            }
            e.RetrieveContext(JSEngineManager.Engine);
        }

        public void Register(Trigger trigger)
        {
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));
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

        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
    }
}
