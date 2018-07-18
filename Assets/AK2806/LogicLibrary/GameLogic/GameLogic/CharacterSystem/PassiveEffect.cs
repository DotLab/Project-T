using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;

namespace GameLogic.CharacterSystem
{
    public class PassiveEffect : Trigger
    {


        public PassiveEffect(string boundEventID, Command command) :
            base(boundEventID, command)
        {

        }
    }
}
