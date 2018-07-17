using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.ScriptSystem
{
    public class Command
    {
        protected readonly string _actionJS;

        public string ActionJS => _actionJS;

        public Command(string jscode)
        {
            _actionJS = jscode ?? throw new ArgumentNullException(nameof(jscode));
        }

        public virtual void DoAction(JSEngine engine)
        {
            engine.Execute(_actionJS);
        }

    }
    
}
