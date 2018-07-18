using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.CharacterSystem
{
    public class InitiativeEffect : Command
    {
        protected bool _dmCheck = false;

        public bool DMCheck { get => _dmCheck; set => _dmCheck = value; }

        public InitiativeEffect(string jscode) :
            base(jscode)
        {
        }

        public override void DoAction(JSEngine engine)
        {
            
            base.DoAction(engine);
        }
    }
}
