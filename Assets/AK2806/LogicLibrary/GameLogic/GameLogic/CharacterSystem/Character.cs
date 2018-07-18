using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.CharacterSystem
{
    public abstract class Character : IProperty, IIdentifiable
    {
        #region Javascript API class
        private sealed class API : IJSAPI
        {
            private readonly Character _outer;

            public API(Character outer)
            {
                _outer = outer;
            }

            public string getID()
            {
                try
                {
                    return _outer.ID;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
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

            public void setName(string name)
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

            public IJSAPI getBelong()
            {
                try
                {
                    if (_outer.Belong != null) return (IJSAPI)_outer.Belong.GetContext();
                    else return null;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }
            
            public IJSAPI getAspectList()
            {
                try
                {
                    return (IJSAPI)_outer.Aspects.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI getSkillList()
            {
                try
                {
                    return (IJSAPI)_outer.Skills.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI getStuntList()
            {
                try
                {
                    return (IJSAPI)_outer.Stunts.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI getExtraList()
            {
                try
                {
                    return (IJSAPI)_outer.Extras.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI getConsequenceList()
            {
                try
                {
                    return (IJSAPI)_outer.Consequences.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public int getRefreshPoint()
            {
                try
                {
                    return _outer.Refresh;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setRefreshPoint(int value)
            {
                try
                {
                    _outer.Refresh = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getFatePoint()
            {
                try
                {
                    return _outer.Fate;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setFatePoint(int value)
            {
                try
                {
                    _outer.Fate = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getPhysicsStress()
            {
                try
                {
                    return _outer.PhysicsStress;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setPhysicsStress(int value)
            {
                try
                {
                    _outer.PhysicsStress = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getPhysicsStressMax()
            {
                try
                {
                    return _outer.PhysicsStressMax;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setPhysicsStressMax(int value)
            {
                try
                {
                    _outer.PhysicsStressMax = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getMentalStress()
            {
                try
                {
                    return _outer.MentalStress;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setMentalStress(int value)
            {
                try
                {
                    _outer.MentalStress = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getMentalStressMax()
            {
                try
                {
                    return _outer.MentalStressMax;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void setMentalStressMax(int value)
            {
                try
                {
                    _outer.MentalStressMax = value;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
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
        #endregion
        private readonly API _apiObj;
        
        protected readonly string _id;
        protected string _name = "";
        protected string _description = "";
        protected Character _belong = null;
        protected readonly Viewable _view;

        public string ID => _id;
        public string Name { get => _name; set => _name = value ?? throw new ArgumentNullException(nameof(Name)); }
        public string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(Description)); }
        public Character Belong { get => _belong; set => _belong = value; }
        public Viewable View => _view;

        public Character(string id, Viewable view)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _apiObj = new API(this);
        }

        public abstract PropertyList<Aspect> Aspects { get; }
        public abstract PropertyList<Skill> Skills { get; }
        public abstract PropertyList<Stunt> Stunts { get; }
        public abstract PropertyList<Extra> Extras { get; }
        public abstract PropertyList<Consequence> Consequences { get; }
        public abstract int Refresh { get; set; }
        public abstract int Fate { get; set; }
        public abstract int PhysicsStress { get; set; }
        public abstract int PhysicsStressMax { get; set; }
        public abstract int MentalStress { get; set; }
        public abstract int MentalStressMax { get; set; }

        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
        
        protected Dictionary<SkillType, SkillType[]> againstOvercome = new Dictionary<SkillType, SkillType[]>();
        protected Dictionary<SkillType, SkillType[]> againstAdvantage = new Dictionary<SkillType, SkillType[]>();
        protected Dictionary<SkillType, SkillType[]> againstAttack = new Dictionary<SkillType, SkillType[]>();

        public Dictionary<SkillType, SkillType[]> AgainstTable(CharacterAction action)
        {
            switch (action)
            {
                case CharacterAction.Overcome:
                    return againstOvercome;
                case CharacterAction.Advantage:
                    return againstAdvantage;
                case CharacterAction.Attack:
                    return againstAttack;
                default:
                    return null;
            }
        }

    }

    public class TemporaryCharacter : Character
    {
        protected readonly PropertyList<Aspect> _aspects;
        protected readonly PropertyList<Skill> _skills;
        protected int _physicsStress = 0;
        protected int _physicsStressMax = 0;
        protected int _mentalStress = 0;
        protected int _mentalStressMax = 0;

        private readonly ReadonlyPropertyList<Stunt> _stunts;
        private readonly ReadonlyPropertyList<Extra> _extras;
        private readonly ReadonlyPropertyList<Consequence> _consequences;

        public override PropertyList<Aspect> Aspects => _aspects;
        public override PropertyList<Skill> Skills => _skills;
        public override int PhysicsStress { get => _physicsStress; set => _physicsStress = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(PhysicsStress), "Physics stress is less than 0."); }
        public override int PhysicsStressMax { get => _physicsStressMax; set => _physicsStressMax = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(PhysicsStressMax), "Max physics stress is less than 1."); }
        public override int MentalStress { get => _mentalStress; set => _mentalStress = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(MentalStress), "Mental stress is less than 0."); }
        public override int MentalStressMax { get => _mentalStressMax; set => _mentalStressMax = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(MentalStressMax), "Max mental stress is less than 1."); }

        public override PropertyList<Stunt> Stunts => _stunts;
        public override int Refresh { get => 0; set { } }
        public override int Fate { get => 0; set { } }
        public override PropertyList<Extra> Extras => _extras;
        public override PropertyList<Consequence> Consequences => _consequences;

        public TemporaryCharacter(string id, Viewable view) : base(id, view)
        {
            _aspects = new PropertyList<Aspect>(this);
            _skills = new PropertyList<Skill>(this);
            _stunts = new ReadonlyPropertyList<Stunt>(this, null);
            _extras = new ReadonlyPropertyList<Extra>(this, null);
            _consequences = new ReadonlyPropertyList<Consequence>(this, null);
        }

        public int RollDice(SkillType skillType)
        {
            //return FateDice.Roll() + this.SkillLevel(skillType);
            return this.SkillLevel(skillType);
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

    public class CommonCharacter : TemporaryCharacter
    {
        protected int _refresh = 0;
        protected int _fate = 0;
        protected readonly PropertyList<Stunt> _stunts;
        protected readonly PropertyList<Extra> _extras;
        
        public override PropertyList<Stunt> Stunts => _stunts;
        public override PropertyList<Extra> Extras => _extras;
        public override int Refresh { get => _refresh; set => _refresh = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(Refresh), "Refresh point is less than 1."); }
        public override int Fate { get => _fate; set => _fate = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(Fate), "Fate point is less than 0."); }

        public CommonCharacter(string id, Viewable view) : base(id, view)
        {
            _stunts = new PropertyList<Stunt>(this);
            _extras = new PropertyList<Extra>(this);
        }

    }

    public class KeyCharacter : CommonCharacter
    {
        protected readonly PropertyList<Consequence> _consequences;

        public override PropertyList<Consequence> Consequences => _consequences;

        public KeyCharacter(string id, Viewable view) : base(id, view)
        {
            _consequences = new PropertyList<Consequence>(this);
        }

    }

    public sealed class CharacterManager : IJSContextProvider
    {
        private sealed class API : IJSAPI
        {
            private readonly CharacterManager _outer;

            public API(CharacterManager outer)
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

        private static readonly CharacterManager _instance = new CharacterManager();
        public static CharacterManager Instance => _instance;

        private readonly IdentifiedObjList<Character> _templateItems;
        private readonly IdentifiedObjList<Character> _keyItems;
        private readonly IdentifiedObjList<Character> _templateCharacters;
        private readonly IdentifiedObjList<Character> _keyCharacters;
        private readonly IdentifiedObjList<Character> _players;

        public IdentifiedObjList<Character> TemplateItems => _templateItems;
        public IdentifiedObjList<Character> KeyItems => _keyItems;
        public IdentifiedObjList<Character> TemplateCharacters => _templateCharacters;
        public IdentifiedObjList<Character> KeyCharacters => _keyCharacters;
        public IdentifiedObjList<Character> Players => _players;

        private CharacterManager()
        {
            _templateItems = new IdentifiedObjList<Character>();
            _keyItems = new IdentifiedObjList<Character>();
            _templateCharacters = new IdentifiedObjList<Character>();
            _keyCharacters = new IdentifiedObjList<Character>();
            _players = new IdentifiedObjList<Character>();
            _apiObj = new API(this);
        }

        public Character CreateTempChara(string templateID)
        {
            return this.CreateTempChara(_templateCharacters[templateID]);
        }

        public Character CreateTempChara(Character template)
        {
            throw new NotImplementedException();
        }



        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
    }
}
