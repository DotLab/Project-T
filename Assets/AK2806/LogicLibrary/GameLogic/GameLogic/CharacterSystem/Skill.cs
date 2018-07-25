using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.CharacterSystem
{
    public sealed class SkillType : IEquatable<SkillType>
    {
        public static readonly SkillType Athletics = new SkillType("Athletics", "运动", false, true, true);
        public static readonly SkillType Burglary = new SkillType("Burglary", "盗窃");
        public static readonly SkillType Contacts = new SkillType("Contacts", "人脉");
        public static readonly SkillType Crafts = new SkillType("Crafts", "工艺");
        public static readonly SkillType Deceive = new SkillType("Deceive", "欺诈");
        public static readonly SkillType Drive = new SkillType("Drive", "驾驶", false, false, true);
        public static readonly SkillType Empathy = new SkillType("Empathy", "共情");
        public static readonly SkillType Fight = new SkillType("Fight", "战斗", true, true);
        public static readonly SkillType Investigate = new SkillType("Investigate", "调查");
        public static readonly SkillType Lore = new SkillType("Lore", "学识");
        public static readonly SkillType Notice = new SkillType("Notice", "洞察");
        public static readonly SkillType Physique = new SkillType("Physique", "体格", false, true);
        public static readonly SkillType Provoke = new SkillType("Provoke", "威胁", true);
        public static readonly SkillType Rapport = new SkillType("Rapport", "交际");
        public static readonly SkillType Resources = new SkillType("Resources", "资源");
        public static readonly SkillType Shoot = new SkillType("Shoot", "射击", true);
        public static readonly SkillType Stealth = new SkillType("Stealth", "潜行");
        public static readonly SkillType Will = new SkillType("Will", "意志", false, true);
        
        private static readonly Dictionary<string, SkillType> skillTypes = new Dictionary<string, SkillType>();
        public static Dictionary<string, SkillType> SkillTypes => skillTypes;
        
        static SkillType()
        {
            skillTypes.Add(Athletics.Name, Athletics);
            skillTypes.Add(Burglary.Name, Burglary);
            skillTypes.Add(Contacts.Name, Contacts);
            skillTypes.Add(Crafts.Name, Crafts);
            skillTypes.Add(Deceive.Name, Deceive);
            skillTypes.Add(Drive.Name, Drive);
            skillTypes.Add(Empathy.Name, Empathy);
            skillTypes.Add(Fight.Name, Fight);
            skillTypes.Add(Investigate.Name, Investigate);
            skillTypes.Add(Lore.Name, Lore);
            skillTypes.Add(Notice.Name, Notice);
            skillTypes.Add(Physique.Name, Physique);
            skillTypes.Add(Provoke.Name, Provoke);
            skillTypes.Add(Rapport.Name, Rapport);
            skillTypes.Add(Resources.Name, Resources);
            skillTypes.Add(Shoot.Name, Shoot);
            skillTypes.Add(Stealth.Name, Stealth);
            skillTypes.Add(Will.Name, Will);
        }

        private readonly string _id;
        private readonly string _name;
        private readonly bool _canAttack;
        private readonly bool _canDefend;
        private readonly bool _canMove;

        public string Id => _id;
        public string Name => _name;
        public bool CanAttack => _canAttack;
        public bool CanDefend => _canDefend;
        public bool CanMove => _canMove;
        
        private SkillType(string id, string name, bool canAttack = false, bool canDefend = false, bool canMove = false)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _canAttack = canAttack;
            _canDefend = canDefend;
            _canMove = canMove;
        }

        public bool Equals(SkillType other)
        {
            return other != null && _id == other._id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SkillType);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
        
    }

    public sealed class Skill : ICharacterProperty
    {
        #region Javascript API class
        private sealed class API : IJSAPI<Skill>
        {
            private readonly Skill _outer;

            public API(Skill outer)
            {
                _outer = outer;
            }

            public string getName()
            {
                try
                {
                    return _outer.Name;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }
            
            public string getDescription()
            {
                try
                {
                    return _outer.Description;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void setDescription(string name)
            {
                try
                {
                    _outer.Name = name;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getLevel()
            {
                try
                {
                    return _outer.Level;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setLevel(int value)
            {
                try
                {
                    _outer.Level = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public string getSkillType()
            {
                try
                {
                    return _outer.SkillType.Id;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void setSkillType(string id)
            {
                try
                {
                    _outer.SkillType = SkillType.SkillTypes[id];
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public Skill Origin(JSContextHelper proof)
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
        #endregion
        private readonly API _apiObj;
        
        private string _description = "";
        private Character _belong = null;
        private SkillType _skillType;
        private int _level = 0;
        private bool _canAttack;
        private bool _canDefend;
        private bool _canMove;

        public string Name { get => _skillType.Name; set { } }
        public string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(value)); }
        public Character Belong { get => _belong; set => _belong = value; }
        public SkillType SkillType { get => _skillType; set => _skillType = value ?? throw new ArgumentNullException(nameof(value)); }
        public int Level { get => _level; set => _level = value; }
        public bool CanAttack { get => _canAttack; set => _canAttack = value; }
        public bool CanDefend { get => _canDefend; set => _canDefend = value; }
        public bool CanMove { get => _canMove; set => _canMove = value; }

        public Skill(SkillType skillType)
        {
            _skillType = skillType ?? throw new ArgumentNullException(nameof(skillType));
            _canAttack = skillType.CanAttack;
            _canDefend = skillType.CanDefend;
            _canMove = skillType.CanMove;
            _apiObj = new API(this);
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
        
    }
}
