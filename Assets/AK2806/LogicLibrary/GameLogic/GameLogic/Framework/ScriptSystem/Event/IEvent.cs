using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework.ScriptSystem.Event
{
    public interface IEvent
    {
        string ID { get; }
        void ProvideParam(Engine engine);
    }

    public class Event : IEvent
    {
        private const string id = "event";

        public string ID => id;
        
        public virtual void ProvideParam(Engine engine)
        {
            engine.SetValue("$EventID", id);
        }
    }
}
