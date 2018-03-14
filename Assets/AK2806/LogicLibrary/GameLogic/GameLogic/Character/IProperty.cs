using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Character
{
    interface IProperty
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
