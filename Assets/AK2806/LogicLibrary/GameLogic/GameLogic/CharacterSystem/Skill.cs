using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Container.BattleComponent;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.CharacterSystem
{
    public struct SkillProperty
    {
        public static readonly SkillProperty INIT = new SkillProperty
        {
            level = 0, canAttack = false, canDefend = false, canMove = false,
            useRange = new Range { lowOpen = false, low = 0, highOpen = false, high = 0 },
            affectRange = new Range { lowOpen = false, low = 0, highOpen = false, high = 0 },
            islinearUse = false, islinearAffect = false,
            linearAffectDirection = Direction.POSITIVE_ROW & Direction.POSITIVE_COL & Direction.NEGATIVE_ROW & Direction.NEGATIVE_COL,
            linearUseDirection = Direction.POSITIVE_ROW & Direction.POSITIVE_COL & Direction.NEGATIVE_ROW & Direction.NEGATIVE_COL,
            targetCount = 1
        };

        public int level;
        public bool canAttack;
        public bool canDefend;
        public bool canMove;
        public Range useRange;
        public bool islinearUse;
        public Direction linearUseDirection;
        public Range affectRange;
        public bool islinearAffect;
        public Direction linearAffectDirection;
        public int targetCount;
    }

    public sealed class SkillType : IEquatable<SkillType>
    {
        public static readonly SkillType Athletics = new SkillType("Athletics", "运动", false, true, true);
        public static readonly SkillType Burglary = new SkillType("Burglary", "盗窃");
        public static readonly SkillType Contacts = new SkillType("Contacts", "人脉");
        public static readonly SkillType Crafts = new SkillType("Crafts", "工艺");
        public static readonly SkillType Deceive = new SkillType("Deceive", "欺诈");
        public static readonly SkillType Drive = new SkillType("Drive", "驾驶");
        public static readonly SkillType Empathy = new SkillType("Empathy", "共情");
        public static readonly SkillType Fight = new SkillType("Fight", "战斗", true, true);
        public static readonly SkillType Investigate = new SkillType("Investigate", "调查");
        public static readonly SkillType Lore = new SkillType("Lore", "学识");
        public static readonly SkillType Notice = new SkillType("Notice", "洞察");
        public static readonly SkillType Physique = new SkillType("Physique", "体格", false, true);
        public static readonly SkillType Provoke = new SkillType("Provoke", "威胁", true);
        public static readonly SkillType Rapport = new SkillType("Rapport", "交际");
        public static readonly SkillType Resources = new SkillType("Resources", "资源");
        public static readonly SkillType Shoot = new SkillType("Shoot", "射击");
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
        private readonly SkillProperty _property;

        public string ID => _id;
        public string Name => _name;
        public SkillProperty Property => _property;

        private SkillType(string id, string name, bool canAttack = false, bool canDefend = false, bool canMove = false)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _property = SkillProperty.INIT;
            _property.level = 0;
            _property.canAttack = canAttack;
            _property.canDefend = canDefend;
            _property.canMove = canMove;
        }

        public bool Equals(SkillType other)
        {
            return !(other is null) && _id == other._id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SkillType);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
        
        public static bool operator==(SkillType a, SkillType b)
        {
            return a.Equals(b);
        }

        public static bool operator!=(SkillType a, SkillType b)
        {
            return !(a == b);
        }
    }
    
    public sealed class Skill : IDescribable
    {
        private string _description = "";
        private SkillType _skillType;
        private SkillProperty _property;

        public string Name { get => _skillType.Name; set { } }
        public string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(value)); }
        public SkillType SkillType { get => _skillType; set => _skillType = value ?? throw new ArgumentNullException(nameof(value)); }
        public SkillProperty Property { get => _property; set => _property = value; }

        public Skill(SkillType skillType)
        {
            _skillType = skillType ?? throw new ArgumentNullException(nameof(skillType));
            _property = skillType.Property;
        }

    }
}
