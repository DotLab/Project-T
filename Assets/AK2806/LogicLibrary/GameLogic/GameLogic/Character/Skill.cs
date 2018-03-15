using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;

namespace GameLogic.Character
{
    public class SkillType
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

        public string Name { get => name; set => name = value; }

        public SkillType(string name)
        {
            this.Name = name;
        }
        
    }

    public class Skill : IProperty
    {
        private SkillType skillType;
        private int level;
        private BaseCharacter belong;

        public SkillType SkillType { get => skillType; set => skillType = value; }
        public int Level { get => level; set => level = value; }
        public BaseCharacter Belong { get => belong; set => belong = value; }

        public string Description { get => this.SkillType.Name + " " + this.Level.ToString(); set => throw new NotImplementedException(); }

    }
}
