using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;
using GameLogic.Utilities;

namespace GameLogic.Character
{
    public class BaseCharacter : IProperty, IGroupable
    {
        private string name;
        private string description;
        private string group;
        private BaseCharacter belong;
        private List<Skill> skills;
        
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public string Group { get => group; set => group = value; }
        public BaseCharacter Belong { get => belong; set => belong = value; }
        public List<Skill> Skills => skills;
        public string ID { get => name; set => name = value; }

        public int UseSkill(Skill skill)
        {
            return 0;
        }

        public int UseSkill(int index)
        {
            return 0;
        }
        
    }

    public class Character : BaseCharacter
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

    public class MainCharacter : Character
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
