using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.CharacterSystem
{
    public enum PersistenceType
    {
        Fixed = 0,
        Common = 1,
        Boost = 2
    }

    public enum EffectType
    {
        Negative = 0,
        Positive = 1
    }

    public class Aspect : IProperty
    {
        #region Javascript API class
        private sealed class API : IJSAPI
        {
            private readonly Aspect _outer;

            public API(Aspect outer)
            {
                _outer = outer;
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

            public void setDescription(string value)
            {
                try
                {
                    _outer.Description = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getPersistenceType()
            {
                try
                {
                    return (int)_outer.PersistenceType;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setPersistenceType(int value)
            {
                try
                {
                    _outer.PersistenceType = (PersistenceType)value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getEffectType()
            {
                try
                {
                    return (int)_outer.EffectType;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setEffectType(int value)
            {
                try
                {
                    _outer.EffectType = (EffectType)value;
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
        
        protected string _description;
        protected PersistenceType _persistenceType = PersistenceType.Common;
        protected EffectType _effectType = EffectType.Positive;
        protected Character _belong = null;

        public Aspect(string description)
        {
            _description = description ?? throw new ArgumentNullException(nameof(description));
            _apiObj = new API(this);
        }

        public string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(Description)); }
        public PersistenceType PersistenceType { get => _persistenceType; set => _persistenceType = value; }
        public EffectType EffectType { get => _effectType; set => _effectType = value; }
        public Character Belong { get => _belong; set => _belong = value; }

        public virtual object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
    }

    public class Consequence : Aspect
    {
        #region Javascript API class
        private sealed class API : IJSAPI
        {
            private readonly Consequence _outer;

            public API(Consequence outer)
            {
                _outer = outer;
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

            public void setDescription(string value)
            {
                try
                {
                    _outer.Description = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getPersistenceType()
            {
                try
                {
                    return (int)_outer.PersistenceType;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setPersistenceType(int value)
            {
                try
                {
                    _outer.PersistenceType = (PersistenceType)value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getEffectType()
            {
                try
                {
                    return (int)_outer.EffectType;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setEffectType(int value)
            {
                try
                {
                    _outer.EffectType = (EffectType)value;
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

            public int getCounteractLevel()
            {
                try
                {
                    return _outer.CounteractLevel;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setCounteractLevel(int value)
            {
                try
                {
                    _outer.CounteractLevel = value;
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

        protected int _counteractLevel;
        
        public int CounteractLevel { get => _counteractLevel; set => _counteractLevel = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(CounteractLevel), "Counteract level is less than 0."); }

        public Consequence(string id) :
            base(id)
        {
            _effectType = EffectType.Negative;
            _apiObj = new API(this);
        }

        public override object GetContext()
        {
            return _apiObj;
        }
        
    }
}
