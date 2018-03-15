using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Character
{
    public interface IProperty
    {
        BaseCharacter Belong
        {
            get; set;
        }
        string Description
        {
            get; set;
        }
    }
}
