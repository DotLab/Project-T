using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;

namespace GameLogic.CharacterSystem
{
    public interface IStuntProperty : IAttachable<Stunt> { }

    public class Stunt : ICharacterProperty
    {
        #region Javascript API class
        protected class API : IJSAPI
        {
            private readonly Stunt _outer;

            public API(Stunt outer)
            {
                _outer = outer;
            }

            public string getName()
            {
                try
                {
                    return _outer.Name;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void setName(string value)
            {
                try
                {
                    _outer.Name = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }
            
            public string getDescription()
            {
                try
                {
                    return _outer.Description;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void setDescription(string name)
            {
                try
                {
                    _outer.Name = name;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
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
            
            public IJSAPI getInitiativeEffect()
            {
                try
                {
                    return (IJSAPI)_outer.InitiativeEffect.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
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

        protected string _name = "";
        protected string _description = "";
        protected Character _belong = null;
        protected InitiativeEffect _initiativeEffect;

        public Stunt(InitiativeEffect effect, string name = "", string description = "")
        {
            _initiativeEffect = effect ?? throw new ArgumentNullException(nameof(name));
            if (effect.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(effect));
            effect.Belong = this;
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _description = description ?? throw new ArgumentNullException(nameof(description));
            _apiObj = new API(this);
        }

        ~Stunt() => _initiativeEffect.Belong = null;

        public string Name { get => _name; set => _name = value ?? throw new ArgumentNullException(nameof(value)); }
        public string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(value)); }
        public Character Belong { get => _belong; set => _belong = value; }
        public InitiativeEffect InitiativeEffect
        {
            get => _initiativeEffect;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(value));
                _initiativeEffect.Belong = null;
                _initiativeEffect = value;
                value.Belong = this;
            }
        }

        public virtual IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
    
}
