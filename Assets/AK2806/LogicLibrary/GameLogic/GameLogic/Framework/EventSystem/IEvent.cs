using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework.EventSystem
{
    public interface IEvent : IGroupable
    {
        void ProvideParam(Engine engine);
    }

    public abstract class Event : IEvent
    {
        protected string _id;
        protected string _group;

        public string ID { get => _id; set => _id = value; }
        public string Group { get => _group; set => _group = value; }
        
        public abstract void ProvideParam(Engine engine);
    }
}
