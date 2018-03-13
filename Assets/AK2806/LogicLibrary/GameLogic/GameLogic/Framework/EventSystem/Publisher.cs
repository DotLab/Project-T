using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Framework.EventSystem
{
    class Publisher
    {
        private static Publisher instance = new Publisher();

        public static Publisher Instance()
        {
            return instance;
        }

        private List<ITrigger> subscribers;

        public List<ITrigger> Subscribers
        {
            get
            {
                return this.subscribers;
            }
        }

        private Publisher()
        {
            this.subscribers = new List<ITrigger>();
        }

        public void Publish(IEvent e)
        {

        }
    }
}
