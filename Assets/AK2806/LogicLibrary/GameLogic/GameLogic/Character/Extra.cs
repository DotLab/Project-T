using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;
using GameLogic.Utilities;

namespace GameLogic.Character
{
    public interface IExtra : IProperty, IGroupable
    {
        PropertyList<BaseCharacter> Items { get; }
        List<ITrigger> Triggers { get; }
    }

    public class Extra : IExtra
    {
        private PropertyList<BaseCharacter> items;
        private List<ITrigger> triggers;
        private string id;
        private string group;
        private string description;
        private BaseCharacter belong;

        public string ID { get => id; set => id = value; }
        public string Group { get => group; set => group = value; }
        public string Description { get => description; set => description = value; }
        public PropertyList<BaseCharacter> Items => items;
        public List<ITrigger> Triggers => triggers;
        public BaseCharacter Belong { get => belong; set => belong = value; }
    }
}
