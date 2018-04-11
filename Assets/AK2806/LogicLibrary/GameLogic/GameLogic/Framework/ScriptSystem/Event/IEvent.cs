using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework.ScriptSystem.Event
{
    public interface IEvent : IIdentifiable
    {
        void ProvideParam(Engine engine);
    }

    public class Event : IEvent
    {
        protected string _id;

        public string ID => _id;
        
        protected Event()
        {
            this._id = "event";
        }

        public virtual void ProvideParam(Engine engine)
        {
            engine.SetValue("$EventID", _id);
        }
    }
}
