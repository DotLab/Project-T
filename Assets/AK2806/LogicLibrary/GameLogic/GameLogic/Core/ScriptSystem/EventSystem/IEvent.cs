using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.ScriptSystem.Event
{
    public interface IEvent : IJSContextProvider
    {
        string[] NotifyList { get; }
        void SendContext(JSEngine engine);
        void RetrieveContext(JSEngine engine);
    }
}
