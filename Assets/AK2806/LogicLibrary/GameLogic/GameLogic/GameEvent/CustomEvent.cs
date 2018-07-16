using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.ScriptSystem;
using GameLogic.Core.ScriptSystem.Event;

namespace GameLogic.GameEvent
{
    public class CustomEvent : IEvent
    {
        private struct EventInfo {

        }
        
        private static readonly string[] _idList = {
            "event.custom"
        };

        public virtual string[] NotifyList => _idList;

        public void RetrieveContext(JSEngine engine)
        {
            throw new NotImplementedException();
        }

        public void SendContext(JSEngine engine)
        {
            throw new NotImplementedException();
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
