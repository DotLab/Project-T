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
        protected ICommand _command;
        protected string _eventID;
        protected bool _active;

        public string ID => _command.ID;
        public string EventID => _eventID;
        public ICommand Command { get => _command; set => _command = value; }
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
