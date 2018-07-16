using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.ScriptSystem.Event
{
    public class Trigger : IJSContextProvider
    {
        private sealed class API : IJSAPI
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

        private readonly API _apiObj;

        protected ICommand _command;
        protected string _boundEventID;
        protected bool _active;
        
        public string BoundEventID => _boundEventID;
        public ICommand Command { get => _command; set => _command = value ?? throw new ArgumentNullException(nameof(Command)); }
        public bool Active { get => _active; set => _active = value; }

        public Trigger(string boundEventID, ICommand command, bool autoReg = true)
        {
            _boundEventID = boundEventID ?? throw new ArgumentNullException(nameof(boundEventID));
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _active = true;
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

        public virtual void Notify(JSEngine engine)
        {
            _command.DoAction(engine);
        }

        public virtual object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
    }
}
