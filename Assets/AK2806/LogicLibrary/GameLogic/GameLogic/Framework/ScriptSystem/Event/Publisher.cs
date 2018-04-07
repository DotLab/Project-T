using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework.ScriptSystem.Event
{
    public static class Publisher 
    {
        private static Dictionary<string, List<ITrigger> > subscribers = new Dictionary<string, List<ITrigger> >();

        public static void Publish(Engine engine, IEvent e)
        {
            string[] splitIDs = e.ID.Split(".");
            for (int i = 1; i < splitIDs.Length; ++i)
            {
                splitIDs[i] = splitIDs[i - 1] + "." + splitIDs[i];
            }
            e.ProvideParam(engine);
            foreach (string id in splitIDs)
            {
                List<ITrigger> triggers = subscribers[id];
                if (triggers != null)
                {
                    foreach (ITrigger trigger in triggers)
                    {
                        if (trigger.Active)
                        {
                            trigger.DoAction(engine);
                        }
                    }
                }
            }
        }

        public static void Register(ITrigger trigger)
        {
            if (!subscribers.ContainsKey(trigger.EventID))
            {
                subscribers.Add(trigger.EventID, new List<ITrigger>());
            }
            List<ITrigger> triggers = subscribers[trigger.EventID];
            triggers.Add(trigger);
        }
    }
}
