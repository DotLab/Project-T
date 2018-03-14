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

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public AspectTimeType TimeType
        {
            get
            {
                return this.timeType;
            }
            set
            {
                this.timeType = value;
            }
        }

        public AspectEffectType EffectType
        {
            get
            {
                return this.effectType;
            }
            set
            {
                this.effectType = value;
            }
        }

        public BaseCharacter Belong
        {
            get
            {
                return this.belong;
            }
            set
            {
                this.belong = value;
            }
        }

        public string Group
        {
            get
            {
                return this.group;
            }
            set
            {
                this.group = value;
            }
        }

        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        
    }

    class Consequence : IAspect
    {
        private IAspect aspect;
        private int level;

        public IAspect Aspect
        {
            get
            {
                return this.aspect;
            }
            set
            {
                this.aspect = value;
            }
        }

        public int Level
        {
            get
            {
                return this.level;
            }
            set
            {
                this.level = value;
            }
        }

        public string Description
        {
            get
            {
                return this.Aspect.Description;
            }
            set
            {
                this.Aspect.Description = value;
            }
        }

        public string Group
        {
            get
            {
                return this.Aspect.Group;
            }
            set
            {
                this.Aspect.Group = value;
            }
        }

        public string ID
        {
            get
            {
                return this.Aspect.ID;
            }
            set
            {
                this.Aspect.ID = value;
            }
        }

        public AspectTimeType TimeType
        {
            get
            {
                return this.Aspect.TimeType;
            }
            set
            {
                this.Aspect.TimeType = value;
            }
        }

        public AspectEffectType EffectType
        {
            get
            {
                return this.Aspect.EffectType;
            }
            set
            {
                this.Aspect.EffectType = value;
            }
        }

        public BaseCharacter Belong
        {
            get
            {
                return this.Aspect.Belong;
            }
            set
            {
                this.Aspect.Belong = value;
            }
        }

    }
}
