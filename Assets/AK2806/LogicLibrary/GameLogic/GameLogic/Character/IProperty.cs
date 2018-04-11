using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;

namespace GameLogic.Character
{
    public interface IProperty : IDescribable
    {
        BaseCharacter Belong { get; set; }
    }
}
