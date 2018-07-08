using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.ScriptSystem.Event
{
    public class Trigger
    {
        protected ICommand _command;
        protected string _eventID;
        protected bool _active;
        
        public string BoundEventID => _eventID;
        public ICommand Command { get => _command; set => _command = value; }
        public bool Active { get => _active; set => _active = value; }

        public Trigger(string eventID)
        {
            this._eventID = eventID;
        }

        public virtual void Notify(JSEngine engine)
        {
            throw new NotImplementedException();
        }
    }
}
