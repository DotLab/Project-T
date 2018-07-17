using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.EventSystem
{
    public abstract class Event : IJSContextProvider
    {
        protected bool _swallowed;

        public bool Swallowed { get => _swallowed; set => _swallowed = value; }

        public void SendContext(JSEngine engine)
        {
            IEventInfo eventInfo = (IEventInfo)this.GetContext();
            eventInfo.swallowed = _swallowed;
            this.SetContext(eventInfo);
            engine.SynchronizeContext("eventArgs", this);
        }

        public void RetrieveContext(JSEngine engine)
        {
            engine.SynchronizeContext("eventArgs", this);
            engine.RemoveContext("eventArgs");
            IEventInfo eventInfo = (IEventInfo)this.GetContext();
            _swallowed = eventInfo.swallowed;
        }

        public abstract string[] NotifyList { get; }

        public abstract object GetContext();
        public abstract void SetContext(object context);
    }

    public interface IEventInfo
    {
        bool swallowed { get; set; }
    }
}
