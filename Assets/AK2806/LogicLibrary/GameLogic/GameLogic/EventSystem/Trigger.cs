using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.EventSystem
{
    public class Trigger : IJSContextProvider
    {
        #region Javascript API class
        protected class API : IJSAPI
        {
            private readonly Trigger _outer;

            public API(Trigger outer)
            {
                _outer = outer;
            }

            public string getBoundEventID()
            {
                try
                {
                    return _outer.BoundEventID;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void setActive(bool value)
            {
                try
                {
                    _outer.Active = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public bool isActive()
            {
                try
                {
                    return _outer.Active;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public void register()
            {
                try
                {
                    _outer.Register();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void unregister()
            {
                try
                {
                    _outer.Unregister();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public IJSContextProvider Origin(JSContextHelper proof)
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

        protected Command _command;
        protected string _boundEventID;
        protected bool _active;
        
        public string BoundEventID => _boundEventID;
        public Command Command { get => _command; set => _command = value ?? throw new ArgumentNullException(nameof(value)); }
        public bool Active { get => _active; set => _active = value; }

        public Trigger(string boundEventID, Command command, bool autoReg = true)
        {
            _boundEventID = boundEventID ?? throw new ArgumentNullException(nameof(boundEventID));
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _active = true;
            _apiObj = new API(this);
            if (autoReg) this.Register();
        }
        
        public void Register()
        {
            GameEventBus.Instance.Register(this);
        }

        public void Unregister()
        {
            GameEventBus.Instance.Unregister(this);
        }

        public virtual void Notify()
        {
            _command.DoAction();
        }

        public virtual IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
