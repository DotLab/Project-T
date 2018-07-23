using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;

namespace GameLogic.CharacterSystem
{
    public sealed class PassiveEffect : Trigger, IExtraProperty
    {
        #region Javascript API class
        private new class API : Trigger.API, IJSAPI<PassiveEffect>
        {
            private readonly PassiveEffect _outer;

            public API(PassiveEffect outer) :
                base(outer)
            {
                _outer = outer;
            }

            public IJSAPI<Extra> getBelongExtra()
            {
                try
                {
                    if (_outer.Belong != null) return (IJSAPI<Extra>)_outer.Belong.GetContext();
                    else return null;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            PassiveEffect IJSAPI<PassiveEffect>.Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly API _apiObj;

        private Extra _belong = null;
        
        public PassiveEffect(string boundEventID, Command command) :
            base(boundEventID, command)
        {
            _apiObj = new API(this);
        }

        public override void Notify()
        {
            JSEngineManager.Engine.SynchronizeContext("$__belongExtra__", _belong);
            base.Notify();
            JSEngineManager.Engine.RemoveContext("$__belongExtra__");
        }

        public Extra Belong { get => _belong; set => _belong = value; }
        
        public override IJSContext GetContext()
        {
            return _apiObj;
        }

    }
}
