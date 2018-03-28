using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;
using GameLogic.Utilities;

namespace GameLogic.Character
{
    public class BaseCharacter : IProperty, IGroupable
    {
        protected string name;
        protected string description;
        protected PropertyList<IAspect> _aspects;
        protected List<Skill> skills;
        protected BaseCharacter belong;
        protected string group;
        protected string id;

        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public PropertyList<IAspect> Aspects => _aspects;
        public List<Skill> Skills => skills;
        public BaseCharacter Belong { get => belong; set => belong = value; }
        public string Group { get => group; set => group = value; }
        public string ID { get => id; set => id = value; }

        public int RollDice(SkillType skillType)
        {
            return FateDice.Roll() + this.SkillLevel(skillType);
        }
        
        public int SkillLevel(SkillType skillType)
        {
            foreach (Skill skill in this.skills)
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
        protected int _physicsStress;
        protected int _physicsStressMax;
        protected int _mentalStress;
        protected int _mentalStressMax;

        public int PhysicsStress { get => _physicsStress; set => _physicsStress = value; }
        public int PhysicsStressMax { get => _physicsStressMax; set => _physicsStressMax = value; }
        public int MentalStress { get => _mentalStress; set => _mentalStress = value; }
        public int MentalStressMax { get => _mentalStressMax; set => _mentalStressMax = value; }
        public PropertyList<IStunt> Stunts => _stunts;
    }

    public class MainCharacter : Character
    {
        protected int _refresh;
        protected int _fate;
        protected PropertyList<IExtra> _extras;
        protected PropertyList<IConsequence> _consequences;

        public int Refresh { get => _refresh; set => _refresh = value; }
        public int Fate { get => _fate; set => _fate = value; }
        public PropertyList<IExtra> Extras => _extras;
        public PropertyList<IConsequence> Consequences => _consequences;
    }
}
