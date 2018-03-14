using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;

namespace GameLogic.Character
{
    class SkillType
    {
        private static Dictionary<SkillType, SkillType[]> skillTypeDict = new Dictionary<SkillType, SkillType[]>();
        private static List<SkillType> skillTypes = new List<SkillType>();

        public static Dictionary<SkillType, SkillType[]> SkillTypeDict()
        {
            return skillTypeDict;
        }

        public static List<SkillType> SkillTypes()
        {
            return skillTypes;
        }

        static SkillType()
        {

        }

        private string name;

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
        
    }

    class Skill : IProperty
    {
        private SkillType skillType;
        private int level;
        private BaseCharacter belong;

        public SkillType SkillType
        {
            get
            {
                return this.skillType;
            }
            set
            {
                this.skillType = value;
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
                return this.SkillType.Name + " " + this.Level.ToString();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
