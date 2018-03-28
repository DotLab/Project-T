using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Framework.EventSystem
{
    public sealed class Publisher 
    {
        private static Publisher instance = new Publisher();

        public static Publisher Instance() => instance;

        private List<ITrigger> _subscribers;

        public List<ITrigger> Subscribers => this._subscribers;

        private Publisher()
        {
            this._subscribers = new List<ITrigger>();
        }

        public void Publish(IEvent e)
        {

        }
    }
}
