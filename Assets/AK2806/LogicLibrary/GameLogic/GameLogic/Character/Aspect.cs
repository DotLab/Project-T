using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;

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

    public interface IAspect : IIdentifiable, IProperty
    {
        AspectTimeType TimeType { get; set; }
        AspectEffectType EffectType { get; set; }
    }

    public interface IConsequence : IAspect
    {
        int Level { get; set; }
    }

    public class Aspect : IAspect
    {
        protected string _description;
        protected readonly string _id;
        protected AspectTimeType _timeType;
        protected AspectEffectType _effectType;
        protected BaseCharacter _belong;

        public Aspect(string id)
        {
            this._id = id;
        }

        public string Description { get => _description; set => _description = value; }
        public string ID => _id;
        public AspectTimeType TimeType { get => _timeType; set => _timeType = value; }
        public AspectEffectType EffectType { get => _effectType; set => _effectType = value; }
        public BaseCharacter Belong { get => _belong; set => _belong = value; }
    }

    public class Consequence : IConsequence
    {
        protected IAspect _aspect;
        protected int _level;

        public IAspect Aspect { get => _aspect; set => _aspect = value; }
        public int Level { get => _level; set => _level = value; }

        public string Description { get => _aspect.Description; set => _aspect.Description = value; }
        public string ID => _aspect.ID;
        public AspectTimeType TimeType { get => _aspect.TimeType; set => _aspect.TimeType = value; }
        public AspectEffectType EffectType { get => _aspect.EffectType; set => _aspect.EffectType = value; }
        public BaseCharacter Belong { get => _aspect.Belong; set => _aspect.Belong = value; }

    }
}
