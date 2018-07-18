using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core
{
    public class Command
    {
        protected readonly bool _javascript;
        protected readonly string _actionJS;
        protected readonly Action _actionCLR;

        public string ActionJS => _actionJS;
        public Action ActionCLR => _actionCLR;
        
        public Command(string jscode) : this(true, null, jscode) { }

        public Command(Action action) : this(false, action, null) { }

        public Command(bool javascript, Action action, string jscode)
        {
            _javascript = javascript;
            if (_javascript)
            {
                _actionCLR = null;
                _actionJS = jscode ?? throw new ArgumentNullException(nameof(jscode));
            }
            else
            {
                _actionCLR = action ?? throw new ArgumentNullException(nameof(action));
                _actionJS = null;
            }
        }

        public virtual void DoAction()
        {
            if (_javascript) JSEngineManager.Engine.Execute(_actionJS);
            else _actionCLR();
        }

    }
    
}
