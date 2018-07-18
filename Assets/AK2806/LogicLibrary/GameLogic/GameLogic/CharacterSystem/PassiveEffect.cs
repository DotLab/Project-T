using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;

namespace GameLogic.CharacterSystem
{
    public class PassiveEffect : Trigger, IExtraProperty
    {
        protected Extra _belong;

        public PassiveEffect(string boundEventID, Command command) :
            base(boundEventID, command)
        {

        }

        public override void Notify(JSEngine engine)
        {
            JSEngineManager.Engine.SynchronizeContext("$__belongExtra__", _belong);
            base.Notify(engine);
            JSEngineManager.Engine.RemoveContext("$__belongExtra__");
        }

        public Extra Belong { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
