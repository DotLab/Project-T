using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework
{
    public interface ITrigger : ICommand
    {
        string EventID
        {
            get;
        }
        bool Active
        {
            get; set;
        }
    }

    public class Trigger : ITrigger
    {
        private string eventID;
        private string actionJS;
        private bool active;

        public string EventID => this.eventID;

        public string ActionJS { get => actionJS; set => actionJS = value; }
        public bool Active { get => active; set => active = value; }

        public Trigger(string eventID)
        {
            this.eventID = eventID;
        }

        public void Action(Engine engine)
        {
            throw new NotImplementedException();
        }
    }
}
