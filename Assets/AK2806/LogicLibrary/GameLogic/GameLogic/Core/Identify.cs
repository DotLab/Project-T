using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.Core
{
    public interface IIdentifiable : IJSContextProvider
    {
        string ID { get; }
    }
    
    public interface IDescribable : IJSContextProvider
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
