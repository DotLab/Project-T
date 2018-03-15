using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Framework
{
    public interface IIdentifiable
    {
        string ID
        {
            get; set;
        }
    }

    public interface IGroupable : IIdentifiable
    {
        string Group
        {
            get; set;
        }
    }
}
