using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.CharacterSystem
{
    public enum CharacterAction
    {
        Overcome = 0b0001,
        Advantage = 0b0010,
        Attack = 0b0100,
        Support = 0b1000
    }

    public sealed class SkillType
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
        private static SkillType Provoke = new SkillType("煽动");
        private static SkillType Rapport = new SkillType("交际");
        private static SkillType Resources = new SkillType("资源");
        private static SkillType Shoot = new SkillType("射击");
        private static SkillType Stealth = new SkillType("潜行");
        private static SkillType Will = new SkillType("意志");
        */
        private static List<SkillType> skillTypes = new List<SkillType>();

        public static List<SkillType> SkillTypes => skillTypes;
        
        static SkillType()
        {
            
        }

        private string _name;
        private CharacterAction _cando;

        public string Name { get => _name; set => _name = value; }
        public CharacterAction Cando { get => _cando; set => _cando = value; }

        public SkillType(string name, CharacterAction cando)
        {
            this._name = name;
            this._cando = cando;
        }
        
    }

    public class Skill : IProperty
    {
        private sealed class API : IJSAPI
        {
            private readonly Skill _outer;

            public API(Skill outer)
            {
                _outer = outer;
            }

            public IJSContextProvider Origin(JSContextHelper proof)
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

        private readonly API _apiObj;

        protected SkillType _skillType;
        protected int _level;
        protected Character _belong;
        protected string _description;

        public SkillType SkillType { get => _skillType; set => _skillType = value; }
        public int Level { get => _level; set => _level = value; }
        public Character Belong { get => _belong; set => _belong = value; }
        public string Description { get => _description; set => _description = value; }

        public Skill()
        {
            _apiObj = new API(this);
        }

        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
    }
}
