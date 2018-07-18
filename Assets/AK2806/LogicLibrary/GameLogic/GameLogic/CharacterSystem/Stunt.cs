using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;

namespace GameLogic.CharacterSystem
{

    public class Stunt : IProperty
    {
        private sealed class API : IJSAPI
        {
            private readonly Stunt _outer;

            public API(Stunt outer)
            {
                _outer = outer;
            }
            /*
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
            */
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

        protected string _name = "";
        protected string _description = "";
        protected Character _belong = null;
        protected InitiativeEffect _initiativeEffect;

        public Stunt(InitiativeEffect effect = null, string name = "", string description = "")
        {
            _initiativeEffect = effect;
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _description = description ?? throw new ArgumentNullException(nameof(description));
            _apiObj = new API(this);
        }

        public string Name { get => _name; set => _name = value ?? throw new ArgumentNullException(nameof(Name)); }
        public string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(Description)); }
        public Character Belong { get => _belong; set => _belong = value; }
        public InitiativeEffect InitiativeEffect { get => _initiativeEffect; set => _initiativeEffect = value; }

        public virtual IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
