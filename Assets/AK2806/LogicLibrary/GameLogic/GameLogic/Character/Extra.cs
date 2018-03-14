using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;
using GameLogic.Utilities;

namespace GameLogic.Character
{
    interface IExtra : IProperty, IGroupable
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

    class Extra : IExtra
    {
        private PropertyList<BaseCharacter> items;
        private ITrigger trigger;
        private string id;
        private string group;
        private string description;
        private BaseCharacter belong;

        public PropertyList<BaseCharacter> Items
        {
            get
            {
                return this.items;
            }
        }

        public ITrigger Trigger
        {
            get
            {
                return this.trigger;
            }
            set
            {
                this.trigger = value;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public BaseCharacter Belong
        {
            get
            {
                return this.belong;
            }
            set
            {
                this.belong = value;
            }
        }

        public string Group
        {
            get
            {
                return this.group;
            }
            set
            {
                this.group = value;
            }
        }

        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

    }
}
