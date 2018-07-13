using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.ScriptSystem.Event
{
    public sealed class Publisher : IJSContextProvider
    {
        private static Publisher _instance = new Publisher();

        public static Publisher Instance => _instance;

        private Dictionary<string, List<Trigger>> _subscribers;

        private Publisher()
        {
            _subscribers = new Dictionary<string, List<Trigger>>();
        }

        public void Publish(JSEngine engine, IEvent e)
        {
            string[] splitIDs = e.EventID.Split(".");
            for (int i = 1; i < splitIDs.Length; ++i)
            {
                splitIDs[i] = splitIDs[i - 1] + "." + splitIDs[i];
            }
            e.SendContext(engine);
            foreach (string id in splitIDs)
            {
                List<Trigger> triggers = _subscribers[id];
                if (triggers != null)
                {
                    foreach (Trigger trigger in triggers)
                    {
                        if (trigger != null && trigger.Active)
                        {
                            trigger.Notify(engine);
                        }
                    }
                }
            }
            e.RetrieveContext(engine);
        }

        public void Register(Trigger trigger)
        {
            if (!_subscribers.ContainsKey(trigger.BoundEventID))
            {
                _subscribers.Add(trigger.BoundEventID, new List<Trigger>());
            }
            List<Trigger> triggers = _subscribers[trigger.BoundEventID];
            triggers.Add(trigger);
        }

        public object GetContext()
        {
            throw new NotImplementedException();
        }

        public void SetContext(object context)
        {
            throw new NotImplementedException();
        }
    }
}
