using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Framework
{
    interface IIdentifiable
    {
        string ID
        {
            get; set;
        }
    }

    interface IGroupable : IIdentifiable
    {
        string Group
        {
            get; set;
        }
    }
}
