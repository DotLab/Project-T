using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.Core
{
    public interface IIdentifiable : JSContext
    {
        string ID { get; }
    }
    
    public interface IDescribable
    {
        string Description { get; set; }
    }
}
