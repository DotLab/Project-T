using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;

namespace GameLogic.CharacterSystem
{
    public interface IExtraProperty : IAttachable<Extra> { }

    public class ExtraPropertyList<T> : AttachableList<Extra, T> where T : class, IExtraProperty
    {
        public ExtraPropertyList(Extra owner) : base(owner) { }
    }

    public class Extra : ICharacterProperty
    {
        #region Javascript API class
        protected class API : IJSAPI<Extra>
        {
            private readonly Extra _outer;

            public API(Extra outer)
            {
                _outer = outer;
            }
            
            public IJSAPI<Character> getItem()
            {
                try
                {
                    return (IJSAPI<Character>)_outer.Item.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void setItem(IJSAPI<Character> item)
            {
                try
                {
                    _outer.Item = JSContextHelper.Instance.GetAPIOrigin(item);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public IJSAPI<Character> getBelong()
            {
                try
                {
                    if (_outer.Belong != null) return (IJSAPI<Character>)_outer.Belong.GetContext();
                    else return null;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void setCustomData(object value)
            {
                try
                {
                    _outer.CustomData = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public object getCustomData()
            {
                try
                {
                    return _outer.CustomData;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI<ExtraPropertyList<PassiveEffect>> getPassiveEffectList()
            {
                try
                {
                    return (IJSAPI<ExtraPropertyList<PassiveEffect>>)_outer.PassiveEffects.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public Extra Origin(JSContextHelper proof)
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
        protected Character _item;
        protected bool _isTool;
        protected bool _isLongRangeWeapon;
        protected bool _isVehicle;
        protected object _customData = null;
        protected readonly ExtraPropertyList<PassiveEffect> _passiveEffects;

        public Extra(Character item)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item));
            if (item.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(item));
            item.Belong = this;
            _passiveEffects = new ExtraPropertyList<PassiveEffect>(this);
            _apiObj = new API(this);
        }

        ~Extra() => _item.Belong = null;

        public string Name { get => _item.Name; set => _item.Name = value; }
        public string Description { get => _item.Description; set => _item.Description = value; }
        public Character Belong { get => _belong; set => _belong = value; }
        public Character Item
        {
            get => _item;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(value));
                _item.Belong = null;
                _item = value;
                value.Belong = this;
            }
        }
        public bool IsTool { get => _isTool; set { _isTool = value; if (!value) _isLongRangeWeapon = _isVehicle = false; } }
        public bool IsLongRangeWeapon { get => _isLongRangeWeapon; set { _isLongRangeWeapon = value; if (value) _isTool = true; } }
        public bool IsVehicle { get => _isVehicle; set { _isVehicle = value; if (value) _isTool = true; } }
        public object CustomData { get => _customData; set => _customData = value; }
        public ExtraPropertyList<PassiveEffect> PassiveEffects => _passiveEffects;

        public virtual IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }

}
