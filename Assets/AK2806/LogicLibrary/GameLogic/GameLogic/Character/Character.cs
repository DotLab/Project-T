using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.Character
{
    public interface ICharacter : IProperty, IIdentifiable
    {
        string Name { get; set; }
        PropertyList<Aspect> Aspects { get; set; }
        List<Skill> Skills { get; set; }
        PropertyList<Stunt> Stunts { get; set; }
        int Refresh { get; set; }
        int Fate { get; set; }
        PropertyList<Extra> Extras { get; set; }
        PropertyList<Consequence> Consequences { get; set; }
        int PhysicsStress { get; set; }
        int PhysicsStressMax { get; set; }
        int MentalStress { get; set; }
        int MentalStressMax { get; set; }
        IViewable View { get; set; }
    }

    public class BaseCharacter : ICharacter
    {
        private static readonly PropertyList<Stunt> _emptyStunts = new PropertyList<Stunt>(null);
        private static readonly PropertyList<Extra> _emptyExtras = new PropertyList<Extra>(null);
        private static readonly PropertyList<Consequence> _emptyConsequences = new PropertyList<Consequence>(null);

        private class API
        {
            private BaseCharacter _outer;

            public API(BaseCharacter outer)
            {
                _outer = outer;
            }

            public string getID()
            {
                return _outer.ID;
            }

            public string name { get => _outer.Name; set => _outer.Name = value; }
            public string description { get => _outer.Description; set => _outer.Description = value; }

            public string getBelongsID()
            {
                return _outer.Belong.ID;
            }


        }

        private API _apiObj;

        protected readonly string _id;
        protected string _name;
        protected string _description;
        protected ICharacter _belong;
        protected PropertyList<Aspect> _aspects;
        protected List<Skill> _skills;
        protected int _physicsStress;
        protected int _physicsStressMax;
        protected int _mentalStress;
        protected int _mentalStressMax;
        protected IViewable _view;

        public string ID => _id;
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public ICharacter Belong { get => _belong; set => _belong = value; }
        public PropertyList<Aspect> Aspects { get => _aspects; set => _aspects = value; }
        public List<Skill> Skills { get => _skills; set => _skills = value; }
        public int PhysicsStress { get => _physicsStress; set => _physicsStress = value; }
        public int PhysicsStressMax { get => _physicsStressMax; set => _physicsStressMax = value; }
        public int MentalStress { get => _mentalStress; set => _mentalStress = value; }
        public int MentalStressMax { get => _mentalStressMax; set => _mentalStressMax = value; }
        public IViewable View { get => _view; set => _view = value; }

        public virtual PropertyList<Stunt> Stunts { get => _emptyStunts; set { } }
        public virtual int Refresh { get => 0; set { } }
        public virtual int Fate { get => 0; set { } }
        public virtual PropertyList<Extra> Extras { get => _emptyExtras; set { } }
        public virtual PropertyList<Consequence> Consequences { get => _emptyConsequences; set { } }

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

        public virtual object GetContext()
        {
            throw new NotImplementedException();
        }

        public virtual void SetContext(object context)
        {
            throw new NotImplementedException();
        }
    }

    public class Character : BaseCharacter
    {
        protected PropertyList<Stunt> _stunts;

        public Character(string id) : base(id)
        {
        }

        public override PropertyList<Stunt> Stunts => _stunts;
    }

    public class MainCharacter : Character
    {
        protected int _refresh;
        protected int _fate;
        protected PropertyList<Extra> _extras;
        protected PropertyList<Consequence> _consequences;

        public MainCharacter(string id) : base(id)
        {
        }

        public override int Refresh { get => _refresh; set => _refresh = value; }
        public override int Fate { get => _fate; set => _fate = value; }
        public override PropertyList<Extra> Extras => _extras;
        public override PropertyList<Consequence> Consequences => _consequences;
    }

    public sealed class CharacterManager : JSContext
    {
        private class API
        {
            private CharacterManager _outer;

            public API(CharacterManager outer)
            {
                _outer = outer;
            }


        }

        private API _apiObj;

        private static CharacterManager _instance = new CharacterManager();
        public static CharacterManager Instance => _instance;
        
        private List<ICharacter> _templateNPC;
        private List<ICharacter> _npc;
        private List<ICharacter> _player;

        public List<ICharacter> TemplateNPC { get => _templateNPC; set => _templateNPC = value; }
        public List<ICharacter> Npc { get => _npc; set => _npc = value; }
        public List<ICharacter> Player { get => _player; set => _player = value; }

        private CharacterManager()
        {
            _templateNPC = new List<ICharacter>();
            _npc = new List<ICharacter>();
            _player = new List<ICharacter>();
        }

        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context)
        {
        }
    }
}
