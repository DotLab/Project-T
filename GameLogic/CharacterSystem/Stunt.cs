﻿using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.EventSystem;

namespace GameLogic.CharacterSystem
{
    public interface IStuntProperty : IAttachable<Stunt> { }

    public sealed class Stunt : AutogenIdentifiable, ICharacterProperty
    {
        #region Javascript API class
        private sealed class JSAPI : IJSAPI<Stunt>
        {
            private readonly Stunt _outer;

            public JSAPI(Stunt outer)
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

            public void setInitiativeEffect(IJSAPI<InitiativeEffect> effect)
            {
                try
                {
                    _outer.InitiativeEffect = JSContextHelper.Instance.GetAPIOrigin(effect);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public IJSAPI<InitiativeEffect> getInitiativeEffect()
            {
                try
                {
                    return (IJSAPI<InitiativeEffect>)_outer.InitiativeEffect.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public Stunt Origin(JSContextHelper proof)
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
        private readonly JSAPI _apiObj;

        private string _name = "";
        private string _description = "";
        private Character _belong = null;
        private InitiativeEffect _initiativeEffect;
        private SkillType _boundSkillType;
        private SkillProperty _skillProperty;
        private readonly bool _needDMCheck;

        public Stunt(InitiativeEffect effect, SkillType boundSkillType, bool needDMCheck = true, string name = "", string description = "")
        {
            if (effect.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(effect));
            _initiativeEffect = effect;
            effect.Belong = this;
            _boundSkillType = boundSkillType ?? throw new ArgumentNullException(nameof(boundSkillType));
            _skillProperty = _boundSkillType.Property;
            _needDMCheck = needDMCheck;
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _description = description ?? throw new ArgumentNullException(nameof(description));
            _apiObj = new JSAPI(this);
        }

        protected override string BaseID => "Stunt";

        public override string Name { get => _name; set => _name = value ?? throw new ArgumentNullException(nameof(value)); }
        public override string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(value)); }
        public Character Belong { get => _belong; set => _belong = value; }
        public InitiativeEffect InitiativeEffect
        {
            get => _initiativeEffect;
            set
            {
                if (value.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(value));
                _initiativeEffect.Belong = null;
                _initiativeEffect = value;
                value.Belong = this;
            }
        }
        public SkillType BoundSkillType { get => _boundSkillType; set => _boundSkillType = value ?? throw new ArgumentNullException(nameof(value)); }
        public SkillProperty SkillProperty { get => _skillProperty; set => _skillProperty = value; }
        public bool NeedDMCheck => _needDMCheck;

        public override IJSContext GetContext()
        {
            return _apiObj;
        }

        public override void SetContext(IJSContext context) { }
    }
    
}