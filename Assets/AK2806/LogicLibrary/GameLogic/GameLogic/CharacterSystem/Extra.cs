using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;

namespace GameLogic.CharacterSystem
{
    public interface IExtraProperty : IAttachable<Extra> { }

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
        protected readonly Character _item;
        protected readonly ExtraPropertyList<PassiveEffect> _passiveEffects;

        public Extra(Character item)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _passiveEffects = new ExtraPropertyList<PassiveEffect>(this);
            _apiObj = new API(this);
        }

        public string Name { get => _item.Name; set => _item.Name = value; }
        public string Description { get => _item.Description; set => _item.Description = value; }
        public Character Belong { get => _belong; set => _belong = value; }
        public Character Item => _item;
        public ExtraPropertyList<PassiveEffect> PassiveEffects => _passiveEffects;

        public virtual IJSContext GetContext()
        {
            return _apiObj;
        }

        public virtual void SetContext(IJSContext context) { }
    }

    public class ExtraPropertyList<T> : AttachableList<Extra, T> where T : class, IExtraProperty
    {
        public ExtraPropertyList(Extra owner) : base(owner) { }
    }
}
