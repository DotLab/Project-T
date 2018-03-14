using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;
using GameLogic.Utilities;

namespace GameLogic.Character
{
    class BaseCharacter : IProperty, IGroupable
    {
        private string name;
        private string description;
        private string group;
        private BaseCharacter belong;
        private List<Skill> skills;

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        
        public List<Skill> Skills
        {
            get
            {
                return this.skills;
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
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public int UseSkill(Skill skill)
        {
            return 0;
        }

        public int UseSkill(int index)
        {
            return 0;
        }
        
    }

    class Character : BaseCharacter
    {
        private PropertyList<IStunt> stunts;
        private int p_stress;
        private int p_stressMax;
        private int m_stress;
        private int m_stressMax;

        public int PhysicsStress { get => p_stress; set => p_stress = value; }
        public int PhysicsStressMax { get => p_stressMax; set => p_stressMax = value; }
        public int MentalStress { get => m_stress; set => m_stress = value; }
        public int MentalStressMax { get => m_stressMax; set => m_stressMax = value; }
        public PropertyList<IStunt> Stunts => stunts;
    }

    class MainCharacter : Character
    {
        private int refresh;
        private int fate;
        private PropertyList<IAspect> aspects;
        private PropertyList<IExtra> extras;
        private PropertyList<Consequence> consequences;

        public int Refresh { get => refresh; set => refresh = value; }
        public int Fate { get => fate; set => fate = value; }
        public PropertyList<IAspect> Aspects => aspects;
        public PropertyList<IExtra> Extras => extras;
        public PropertyList<Consequence> Consequences => consequences;
    }
}
