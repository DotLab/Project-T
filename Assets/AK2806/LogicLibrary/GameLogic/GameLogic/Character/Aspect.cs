using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;

namespace GameLogic.Character
{
    enum AspectTimeType
    {
        Fixed,
        Advance,
        Boost
    }

    enum AspectEffectType
    {
        Negative,
        Positive
    }

    interface IAspect : IGroupable, IProperty
    {
        AspectTimeType TimeType
        {
            get; set;
        }

        AspectEffectType EffectType
        {
            get; set;
        }
    }

    class Aspect : IAspect
    {
        private string description;
        private string id;
        private string group;
        private AspectTimeType timeType;
        private AspectEffectType effectType;
        private BaseCharacter belong;

        public string Description { get => description; set => description = value; }
        public string ID { get => id; set => id = value; }
        public string Group { get => group; set => group = value; }
        public AspectTimeType TimeType { get => timeType; set => timeType = value; }
        public AspectEffectType EffectType { get => effectType; set => effectType = value; }
        public BaseCharacter Belong { get => belong; set => belong = value; }
    }

    class Consequence : IAspect
    {
        private IAspect aspect;
        private int level;

        public IAspect Aspect { get => aspect; set => aspect = value; }
        public int Level { get => level; set => level = value; }

        public string Description { get => this.Aspect.Description; set => this.Aspect.Description = value; }
        public string Group { get => this.Aspect.Group; set => this.Aspect.Group = value; }
        public string ID { get => this.Aspect.ID; set => this.Aspect.ID = value; }
        public AspectTimeType TimeType { get => this.Aspect.TimeType; set => this.Aspect.TimeType = value; }
        public AspectEffectType EffectType { get => this.Aspect.EffectType; set => this.Aspect.EffectType = value; }
        public BaseCharacter Belong { get => this.Aspect.Belong; set => this.Aspect.Belong = value; }

    }
}
