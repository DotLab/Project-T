using System;
using System.Collections.Generic;
using System.Text;
using Jint;

namespace GameLogic.Framework.ScriptSystem
{
    public interface ICommand : IIdentifiable
    {
        void DoAction(Engine engine);
    }
    
    public class Command : ICommand
    {
        protected string _id;
        protected string _actionJS;

        public string ID => _id;
        public string ActionJS { get => _actionJS; set => _actionJS = value; }

        public void DoAction(Engine engine)
        {
            engine.Execute(this.ActionJS);
        }
    }


}
