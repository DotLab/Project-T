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

    public abstract class AutogenIdentifiable : IIdentifiable
    {
        private static ulong _autoIncrement = 0L;

        private readonly ulong _thisNumber;

        public abstract string BaseID { get; }

        public string ID => this.BaseID + "_" + _thisNumber;

        public AutogenIdentifiable()
        {
            _thisNumber = _autoIncrement++;
        }

        public abstract IJSContext GetContext();
        public abstract void SetContext(IJSContext context);
    }

    public interface IDescribable
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
