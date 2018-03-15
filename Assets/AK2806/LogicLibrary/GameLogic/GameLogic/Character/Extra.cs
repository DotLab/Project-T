using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;
using GameLogic.Utilities;

namespace GameLogic.Character
{
    public interface IExtra : IProperty, IGroupable
    {
        PropertyList<BaseCharacter> Items
        {
            get;
        }

        ITrigger Trigger
        {
            get; set;
        }
    }

    public class Extra : IExtra
    {
        private PropertyList<BaseCharacter> items;
        private ITrigger trigger;
        private string id;
        private string group;
        private string description;
        private BaseCharacter belong;

        public string ID { get => id; set => id = value; }
        public string Group { get => group; set => group = value; }
        public string Description { get => description; set => description = value; }
        public PropertyList<BaseCharacter> Items => items;
        public ITrigger Trigger { get => trigger; set => trigger = value; }
        public BaseCharacter Belong { get => belong; set => belong = value; }
    }
}
