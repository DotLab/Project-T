using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.EventSystem
{
    public abstract class Event : IJSContextProvider
    {
        protected bool _swallowed = false;

        public bool Swallowed { get => _swallowed; set => _swallowed = value; }

        public void SendContext(JSEngine engine)
        {
            IEventInfo eventInfo = (IEventInfo)this.GetContext();
            eventInfo.swallowed = _swallowed;
            this.SetContext(eventInfo);
            engine.SynchronizeContext("$__eventArgs__", this);
        }

        public void RetrieveContext(JSEngine engine)
        {
            engine.SynchronizeContext("$__eventArgs__", this);
            engine.RemoveContext("$__eventArgs__");
            IEventInfo eventInfo = (IEventInfo)this.GetContext();
            _swallowed = eventInfo.swallowed;
        }

        public abstract string[] NotifyList { get; }

        public abstract IJSContext GetContext();
        public abstract void SetContext(IJSContext context);
    }

    public interface IEventInfo : IJSContext
    {
        bool swallowed { get; set; }
    }
}
