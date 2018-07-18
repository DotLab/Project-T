using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;

namespace GameLogic.CharacterSystem
{

    public class Extra : IProperty
    {
        #region Javascript API class
        private sealed class API : IJSAPI
        {
            private readonly Extra _outer;

            public API(Extra outer)
            {
                _outer = outer;
            }
            
            public IJSAPI getItem()
            {
                try
                {
                    return (IJSAPI)_outer.Item.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI getBelong()
            {
                try
                {
                    if (_outer.Belong != null) return (IJSAPI)_outer.Belong.GetContext();
                    else return null;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void setInitiativeEffect(string jscode)
            {
                try
                {
                    if (jscode == null) _outer.Command = null;
                    else _outer.Command = new Command(jscode);
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
        
        protected Character _belong = null;
        protected bool _dmCheck = false;
        protected readonly Character _item;
        protected readonly List<Trigger> _triggers;
        protected Command _command = null;

        public Extra(Character item)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _triggers = new List<Trigger>();
            _apiObj = new API(this);
        }

        public string Name { get => _item.Name; set => _item.Name = value; }
        public string Description { get => _item.Description; set => _item.Description = value; }
        public Character Belong { get => _belong; set => _belong = value; }
        public bool DMCheck { get => _dmCheck; set => _dmCheck = value; }
        public Character Item => _item;
        public List<Trigger> Triggers => _triggers;
        public Command Command { get => _command; set => _command = value; }

        public virtual object GetContext()
        {
            return _apiObj;
        }

        public virtual void SetContext(object context) { }
    }


}
