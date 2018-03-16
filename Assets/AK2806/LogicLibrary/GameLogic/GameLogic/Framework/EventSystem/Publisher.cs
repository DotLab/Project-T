using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Framework.EventSystem
{
    public class Publisher
    {
        private static Publisher instance = new Publisher();

        public static Publisher Instance() => instance;

        private List<ITrigger> subscribers;

        public List<ITrigger> Subscribers => this.subscribers;

        private Publisher()
        {
            this.subscribers = new List<ITrigger>();
        }

        public void Publish(IEvent e)
        {

        }
    }
}
