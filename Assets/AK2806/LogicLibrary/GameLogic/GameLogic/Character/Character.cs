using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;
using GameLogic.Utilities;

namespace GameLogic.Character
{
    public class BaseCharacter : IProperty, IIdentifiable
    {
        protected string _name;
        protected string _description;
        protected PropertyList<IAspect> _aspects;
        protected List<Skill> _skills;
        protected BaseCharacter _belong;
        protected readonly string _id;
        protected int _physicsStress;
        protected int _physicsStressMax;
        protected int _mentalStress;
        protected int _mentalStressMax;

        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public PropertyList<IAspect> Aspects => _aspects;
        public List<Skill> Skills => _skills;
        public BaseCharacter Belong { get => _belong; set => _belong = value; }
        public string ID => _id;
        public int PhysicsStress { get => _physicsStress; set => _physicsStress = value; }
        public int PhysicsStressMax { get => _physicsStressMax; set => _physicsStressMax = value; }
        public int MentalStress { get => _mentalStress; set => _mentalStress = value; }
        public int MentalStressMax { get => _mentalStressMax; set => _mentalStressMax = value; }

        protected Dictionary<SkillType, SkillType[]> againstOvercome = new Dictionary<SkillType, SkillType[]>();
        protected Dictionary<SkillType, SkillType[]> againstAdvantage = new Dictionary<SkillType, SkillType[]>();
        protected Dictionary<SkillType, SkillType[]> againstAttack = new Dictionary<SkillType, SkillType[]>();

        public Dictionary<SkillType, SkillType[]> AgainstTable(CharaAction action)
        {
            switch (action)
            {
                case CharaAction.Overcome:
                    return againstOvercome;
                case CharaAction.Advantage:
                    return againstAdvantage;
                case CharaAction.Attack:
                    return againstAttack;
                default:
                    return null;
            }
        }

        public BaseCharacter(string id)
        {
            this._id = id;
        }

        public int RollDice(SkillType skillType)
        {
            return FateDice.Roll() + this.SkillLevel(skillType);
        }
        
        public virtual int SkillLevel(SkillType skillType)
        {
            foreach (Skill skill in this._skills)
            {
                if (skill.SkillType == skillType)
                {
                    return skill.Level;
                }
            }
            return 0;
        }
    }

    public class Character : BaseCharacter
    {
        protected PropertyList<IStunt> _stunts;

        public Character(string id) : base(id)
        {
        }

        public PropertyList<IStunt> Stunts => _stunts;
    }

    public class MainCharacter : Character
    {
        protected int _refresh;
        protected int _fate;
        protected PropertyList<IExtra> _extras;
        protected PropertyList<IConsequence> _consequences;

        public MainCharacter(string id) : base(id)
        {
        }

        public int Refresh { get => _refresh; set => _refresh = value; }
        public int Fate { get => _fate; set => _fate = value; }
        public PropertyList<IExtra> Extras => _extras;
        public PropertyList<IConsequence> Consequences => _consequences;
    }

    public static class CharacterManager
    {
        private static List<BaseCharacter> baseCharacters = new List<BaseCharacter>();
        private static List<Character> characters = new List<Character>();
        private static List<MainCharacter> mainCharacters = new List<MainCharacter>();

        public static List<BaseCharacter> BaseCharacters => baseCharacters;
        public static List<Character> Characters => characters;
        public static List<MainCharacter> MainCharacters => mainCharacters;
    }
}
