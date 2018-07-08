using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.ScriptSystem.Event
{
    public interface IEvent
    {
        void SendContext(JSEngine engine);
        void RetrieveContext(JSEngine engine);
        string EventID { get; }
    }
    
}
