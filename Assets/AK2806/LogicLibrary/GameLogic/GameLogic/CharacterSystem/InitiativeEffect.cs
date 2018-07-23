using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.CharacterSystem
{
    public sealed class InitiativeEffect : Command, IStuntProperty
    {
        #region Javascript API class
        private sealed class API : IJSAPI<InitiativeEffect>
        {
            private readonly InitiativeEffect _outer;

            public API(InitiativeEffect outer)
            {
                _outer = outer;
            }
            
            public IJSAPI<Stunt> getBelongStunt()
            {
                try
                {
                    if (_outer.Belong != null) return (IJSAPI<Stunt>)_outer.Belong.GetContext();
                    else return null;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public bool getDMCheckResult()
            {
                try
                {
                    return _outer.DMCheckResult;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public InitiativeEffect Origin(JSContextHelper proof)
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

        private Stunt _belong = null;

        private bool _dmCheck = false;
        private bool _dmCheckResult = true;

        public bool DMCheck { get => _dmCheck; set => _dmCheck = value; }
        public Stunt Belong { get => _belong; set => _belong = value; }
        public bool DMCheckResult { get => _dmCheck ? _dmCheckResult : true; set { if (_dmCheck) _dmCheckResult = value; } }

        public InitiativeEffect(Action action) : this(false, action, null) { }

        public InitiativeEffect(string jscode) : this(true, null, jscode) { }

        public InitiativeEffect(bool javascript, Action action, string jscode) :
            base(javascript, action, jscode)
        {
            _apiObj = new API(this);
        }

        public override void DoAction()
        {
            JSEngineManager.Engine.SynchronizeContext("$__belongStunt__", _belong);
            base.DoAction();
            JSEngineManager.Engine.RemoveContext("$__belongStunt__");
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
