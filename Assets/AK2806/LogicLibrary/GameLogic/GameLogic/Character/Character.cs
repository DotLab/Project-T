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

    }

    class MainCharacter : Character
    {

    }
}
