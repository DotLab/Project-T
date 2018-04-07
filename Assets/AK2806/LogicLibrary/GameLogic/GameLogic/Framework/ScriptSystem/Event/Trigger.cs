using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework.ScriptSystem.Event
{
    public interface ITrigger : ICommand
    {
        string EventID { get; }
        bool Active { get; set; }
    }

    public class Trigger : ITrigger
    {
        protected string _eventID;
        protected string _actionJS;
        protected bool _active;

        public string EventID => this._eventID;

        public string ActionJS { get => _actionJS; set => _actionJS = value; }
        public bool Active { get => _active; set => _active = value; }

        public Trigger(string eventID)
        {
            this._eventID = eventID;
        }

        public void DoAction(Engine engine)
        {
            throw new NotImplementedException();
        }
    }
}
