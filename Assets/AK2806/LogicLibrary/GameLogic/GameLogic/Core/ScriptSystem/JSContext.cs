using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.ScriptSystem
{
    public interface JSContext
    {
        object GetContext();
        void SetContext(object context);
    }
    
}
