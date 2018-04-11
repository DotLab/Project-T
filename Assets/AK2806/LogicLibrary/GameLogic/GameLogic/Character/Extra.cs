using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;
using GameLogic.Framework.ScriptSystem.Event;
using GameLogic.Utilities;

namespace GameLogic.Character
{
    public interface IExtra : IProperty, IIdentifiable
    {
        PropertyList<BaseCharacter> Items { get; }
        List<ITrigger> Triggers { get; }
    }

    public class Extra : IExtra
    {
        protected PropertyList<BaseCharacter> _items;
        protected List<ITrigger> _triggers;
        protected string _id;
        protected string _description;
        protected BaseCharacter _belong;

        public string ID => _id;
        public string Description { get => _description; set => _description = value; }
        public PropertyList<BaseCharacter> Items => _items;
        public List<ITrigger> Triggers => _triggers;
        public BaseCharacter Belong { get => _belong; set => _belong = value; }
    }
}
