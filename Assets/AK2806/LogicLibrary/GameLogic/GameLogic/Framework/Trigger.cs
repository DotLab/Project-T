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

        public string EventID => this.eventID;

        public string ActionJS => this.actionJS;

        public bool Active => this.active;

        public void Action(Engine engine)
        {
            throw new NotImplementedException();
        }
    }
}
