using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework
{
    public interface ICommand
    {
        void Action(Engine engine);
    }
    
    public class Command : ICommand
    {
        private string _actionJS;

        public string ActionJS { get => _actionJS; set => _actionJS = value; }

        public void Action(Engine engine)
        {
            engine.Execute(this.ActionJS);
        }
    }


}
