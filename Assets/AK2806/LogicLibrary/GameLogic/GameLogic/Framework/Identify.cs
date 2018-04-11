using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Framework
{
    public interface IIdentifiable
    {
        string ID { get; }
    }
    
    public interface IDescribable
    {
        string Description { get; set; }
    }
}
