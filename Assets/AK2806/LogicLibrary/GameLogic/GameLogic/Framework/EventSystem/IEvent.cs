using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework.EventSystem
{
    interface IEvent : IGroupable
    {
        void ProvideParam(Engine engine);
    }

    abstract class Event : IEvent
    {
        private string id;
        private string group;

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

        public abstract void ProvideParam(Engine engine);
    }
}
