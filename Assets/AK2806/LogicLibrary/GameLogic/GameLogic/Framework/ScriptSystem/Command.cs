using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework.ScriptSystem
{
    public interface ICommand
    {
        void DoAction(Engine engine);
    }
    
    public class Command : ICommand
    {
        private string _actionJS;

        public string ActionJS { get => _actionJS; set => _actionJS = value; }

        public void DoAction(Engine engine)
        {
            engine.Execute(this.ActionJS);
        }
    }


}
