using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.CharacterSystem
{
    public interface IProperty : IDescribable, IJSContextProvider
    {
        Character Belong { get; set; }
    }
}
