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
        protected readonly string _id;
        protected readonly string _actionJS;

        public string ID => _id;
        public string ActionJS => _actionJS;

        public Command(string id, string jscode)
        {
            this._id = id;
            this._actionJS = jscode;
        }

        public void DoAction(Engine engine)
        {
            engine.Execute(this.ActionJS);
        }
    }


}
