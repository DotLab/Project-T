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

        public string ID { get => id; set => id = value; }
        public string Group { get => group; set => group = value; }

        public abstract void ProvideParam(Engine engine);
    }
}
