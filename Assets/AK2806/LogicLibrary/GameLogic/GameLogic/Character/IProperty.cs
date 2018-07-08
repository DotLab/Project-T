using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;

namespace GameLogic.Character
{
    public interface IProperty : IDescribable
    {
        ICharacter Belong { get; set; }
    }
}
