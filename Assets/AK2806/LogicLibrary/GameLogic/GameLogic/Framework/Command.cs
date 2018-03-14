using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework
{
    interface ICommand
    {
        void Action(Engine engine);
    }
    
    class Command : ICommand
    {
        private string actionJS;

        public string ActionJS { get => actionJS; set => actionJS = value; }

        public void Action(Engine engine)
        {
            throw new NotImplementedException();
        }
    }


}
