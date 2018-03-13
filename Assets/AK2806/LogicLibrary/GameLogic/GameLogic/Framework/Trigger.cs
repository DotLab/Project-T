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
            get;
        }
    }

    class Trigger : ITrigger
    {
        private string eventID;
        private string actionJS;
        private bool active;

        public string EventID
        {
            get
            {
                return this.eventID;
            }
            set
            {
                this.eventID = value;
            }
        }

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

        public void Action(Engine engine)
        {
            throw new NotImplementedException();
        }
    }
}
