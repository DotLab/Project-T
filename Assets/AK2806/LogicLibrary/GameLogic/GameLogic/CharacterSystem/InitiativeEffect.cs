using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.CharacterSystem
{
    public class InitiativeEffect : Command
    {
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
