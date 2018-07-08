using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;

namespace GameLogic.Character
{
    public enum AspectTimeType
    {
        Fixed,
        Advance,
        Boost
    }

    public enum AspectEffectType
    {
        Negative,
        Positive
    }

    public class Aspect : IIdentifiable, IProperty
    {
        protected string _description;
        protected readonly string _id;
        protected AspectTimeType _timeType;
        protected AspectEffectType _effectType;
        protected ICharacter _belong;

        public Aspect(string id)
        {
            this._id = id;
        }

        public string Description { get => _description; set => _description = value; }
        public string ID => _id;
        public AspectTimeType TimeType { get => _timeType; set => _timeType = value; }
        public AspectEffectType EffectType { get => _effectType; set => _effectType = value; }
        public ICharacter Belong { get => _belong; set => _belong = value; }

        public virtual object GetContext()
        {
            throw new NotImplementedException();
        }

        public virtual void SetContext(object context)
        {
            throw new NotImplementedException();
        }
    }

    public class Consequence : Aspect
    {
        protected int _level;
        
        public int Level { get => _level; set => _level = value; }

        public Consequence(string id) :
            base(id)
        {

        }

        public override object GetContext()
        {
            throw new NotImplementedException();
        }

        public override void SetContext(object context)
        {
            throw new NotImplementedException();
        }
    }
}
