using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework
{
    interface ITrigger : ICommand
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

    class Trigger : ITrigger
    {
        private string eventID;
        private string actionJS;
        private bool active;

        public string EventID => this.eventID;

        public string ActionJS
        {
            get
            {
                return this.actionJS;
            }
            set
            {
                this.actionJS = value;
            }
        }

        public bool Active
        {
            get
            {
                return this.active;
            }
            set
            {
                this.active = value;
            }
        }

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
