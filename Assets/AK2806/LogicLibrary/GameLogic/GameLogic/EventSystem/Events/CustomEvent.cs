using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.EventSystem.Events
{
    public sealed class CustomEvent : Event
    {
        public struct EventInfo : IEventInfo
        {
            public bool swallowed { get; set; }
            public string[] notifyList;
            public object message;

            public EventInfo(object message, string[] notifyList)
            {
                this.message = message;
                this.notifyList = notifyList;
                this.swallowed = false;
            }
        }

        private static readonly string[] _idList = {
            "event.custom"
        };

        private EventInfo _info;
        
        public EventInfo Info { get => _info; set => _info = value; }
        
        public override string[] NotifyList => _info.notifyList ?? _idList;

        public override IJSContext GetContext()
        {
            return _info;
        }

        public override void SetContext(IJSContext context)
        {
            _info = (EventInfo)context;
        }
    }
}
