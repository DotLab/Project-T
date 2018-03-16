using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;

namespace GameLogic.Character
{
    public enum CharaAction
    {
        Overcome = 0b0001,
        Advantage = 0b0010,
        Attack = 0b0100,
        Defend = 0b1000
    }

    public struct SkillTypeWhenAction
    {
        public SkillType type;
        public CharaAction action;

        public SkillTypeWhenAction(SkillType type, CharaAction action)
        {
            this.type = type;
            this.action = action;
        }
    }

    public class SkillType
    {
        /*
        private static SkillType Athletics = new SkillType("运动");
        private static SkillType Burglary = new SkillType("盗窃");
        private static SkillType Contacts = new SkillType("人脉");
        private static SkillType Crafts = new SkillType("工艺");
        private static SkillType Deceive = new SkillType("欺诈");
        private static SkillType Drive = new SkillType("驾驶");
        private static SkillType Empathy = new SkillType("共情");
        private static SkillType Fight = new SkillType("战斗");
        private static SkillType Investigate = new SkillType("调查");
        private static SkillType Lore = new SkillType("学识");
        private static SkillType Notice = new SkillType("洞察");
        private static SkillType Physique = new SkillType("体格");
        private static SkillType Provoke = new SkillType("威胁");
        private static SkillType Rapport = new SkillType("交际");
        private static SkillType Resources = new SkillType("资源");
        private static SkillType Shoot = new SkillType("射击");
        private static SkillType Stealth = new SkillType("潜行");
        private static SkillType Will = new SkillType("意志");
        */
        public static void Register(SkillType skillType, SkillTypeWhenAction[] against)
        {
            skillTypes.Add(skillType);
            againstTable.Add(skillType, against);
        }

        private static Dictionary<SkillType, SkillTypeWhenAction[]> againstTable = new Dictionary<SkillType, SkillTypeWhenAction[]>();
        private static List<SkillType> skillTypes = new List<SkillType>();
        
        public static Dictionary<SkillType, SkillTypeWhenAction[]> AgainstTable => againstTable;
        public static List<SkillType> SkillTypes => skillTypes;

        static SkillType()
        {
            
        }

        private string name;
        private CharaAction cando;

        public string Name { get => name; set => name = value; }
        public CharaAction Cando { get => cando; set => cando = value; }

        public SkillType(string name, CharaAction cando)
        {
            this.Name = name;
            this.Cando = cando;
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
