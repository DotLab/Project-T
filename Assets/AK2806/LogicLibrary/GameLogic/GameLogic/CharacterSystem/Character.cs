using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.CharacterSystem
{
    public interface ICharacterProperty : IDescribable, IAttachable<Character> { }

    public sealed class ReadonlyCharacterPropertyList<T> : CharacterPropertyList<T> where T : class, ICharacterProperty
    {
        public ReadonlyCharacterPropertyList(Character owner, IEnumerable<T> properties) :
            base(owner)
        {
            if (properties != null)
            {
                _container.AddRange(properties);
            }
        }

        public override void Add(T item) { }
        public override void AddRange(IEnumerable<T> items) { }
        public override void Clear() { }
        public override void Insert(int index, T item) { }
        public override bool Remove(T item) { return false; }
        public override void RemoveAt(int index) { }
        public override void Reverse() { }
    }

    public class CharacterPropertyList<T> : AttachableList<Character, T> where T : class, ICharacterProperty
    {
        public CharacterPropertyList(Character owner) : base(owner) { }
    }

    public abstract class Character : IExtraProperty, IIdentifiable
    {
        #region Javascript API class
        protected class API : IJSAPI<Character>
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

            public IJSAPI<Extra> getBelongExtra()
            {
                try
                {
                    if (_outer.Belong != null) return (IJSAPI<Extra>)_outer.Belong.GetContext();
                    else return null;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }
            
            public IJSAPI<CharacterPropertyList<Aspect>> getAspectList()
            {
                try
                {
                    return (IJSAPI<CharacterPropertyList<Aspect>>)_outer.Aspects.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI<CharacterPropertyList<Skill>> getSkillList()
            {
                try
                {
                    return (IJSAPI<CharacterPropertyList<Skill>>)_outer.Skills.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI<CharacterPropertyList<Stunt>> getStuntList()
            {
                try
                {
                    return (IJSAPI<CharacterPropertyList<Stunt>>)_outer.Stunts.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI<CharacterPropertyList<Extra>> getExtraList()
            {
                try
                {
                    return (IJSAPI<CharacterPropertyList<Extra>>)_outer.Extras.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI<CharacterPropertyList<Consequence>> getConsequenceList()
            {
                try
                {
                    return (IJSAPI<CharacterPropertyList<Consequence>>)_outer.Consequences.GetContext();
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
            
            public Character Origin(JSContextHelper proof)
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
        protected Extra _belong = null;
        protected readonly CharacterView _view;

        public string ID => _id;
        public string Name { get => _name; set => _name = value ?? throw new ArgumentNullException(nameof(value)); }
        public string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(value)); }
        public Extra Belong { get => _belong; set => _belong = value; }
        public CharacterView View => _view;

        protected Character(string id, CharacterView view)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _apiObj = new API(this);
        }
        
        public abstract CharacterPropertyList<Aspect> Aspects { get; }
        public abstract CharacterPropertyList<Skill> Skills { get; }
        public abstract CharacterPropertyList<Stunt> Stunts { get; }
        public abstract CharacterPropertyList<Extra> Extras { get; }
        public abstract CharacterPropertyList<Consequence> Consequences { get; }
        public abstract int Refresh { get; set; }
        public abstract int Fate { get; set; }
        public abstract int PhysicsStress { get; set; }
        public abstract int PhysicsStressMax { get; set; }
        public abstract int MentalStress { get; set; }
        public abstract int MentalStressMax { get; set; }

        public int SkillLevel(SkillType skillType)
        {
            foreach (Skill skill in this.Skills)
            {
                if (skill.SkillType == skillType)
                {
                    return skill.Level;
                }
            }
            return 0;
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
        
    }

    public sealed class TemporaryCharacter : Character
    {
        private static readonly ReadonlyCharacterPropertyList<Stunt> _stunts = new ReadonlyCharacterPropertyList<Stunt>(null, null);
        private static readonly ReadonlyCharacterPropertyList<Extra> _extras = new ReadonlyCharacterPropertyList<Extra>(null, null);
        private static readonly ReadonlyCharacterPropertyList<Consequence> _consequences = new ReadonlyCharacterPropertyList<Consequence>(null, null);

        private readonly CharacterPropertyList<Aspect> _aspects;
        private readonly CharacterPropertyList<Skill> _skills;
        private int _physicsStress = 0;
        private int _physicsStressMax = 0;
        private int _mentalStress = 0;
        private int _mentalStressMax = 0;

        public override CharacterPropertyList<Aspect> Aspects => _aspects;
        public override CharacterPropertyList<Skill> Skills => _skills;
        public override int PhysicsStress { get => _physicsStress; set => _physicsStress = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Physics stress is less than 0."); }
        public override int PhysicsStressMax { get => _physicsStressMax; set => _physicsStressMax = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Max physics stress is less than 1."); }
        public override int MentalStress { get => _mentalStress; set => _mentalStress = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Mental stress is less than 0."); }
        public override int MentalStressMax { get => _mentalStressMax; set => _mentalStressMax = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Max mental stress is less than 1."); }

        public override CharacterPropertyList<Stunt> Stunts => _stunts;
        public override int Refresh { get => 0; set { } }
        public override int Fate { get => 0; set { } }
        public override CharacterPropertyList<Extra> Extras => _extras;
        public override CharacterPropertyList<Consequence> Consequences => _consequences;

        public TemporaryCharacter(string id, CharacterView view) :
            base(id, view)
        {
            _aspects = new CharacterPropertyList<Aspect>(this);
            _skills = new CharacterPropertyList<Skill>(this);
        }

        public TemporaryCharacter(string id, TemporaryCharacter template) :
            base(id, template.View)
        {
            this.Name = template.Name;
            this.Description = template.Description;
            foreach (Aspect aspect in template.Aspects)
            {
                Aspect clone = new Aspect();
                clone.Name = aspect.Name;
                clone.Description = aspect.Description;
                clone.PersistenceType = aspect.PersistenceType;
                this.Aspects.Add(clone);
            }
            foreach (Skill skill in template.Skills)
            {
                Skill clone = new Skill(skill.SkillType);
                clone.Name = skill.Name;
                clone.Description = skill.Description;
                clone.Level = skill.Level;
                this.Skills.Add(skill);
            }
            this.PhysicsStress = template.PhysicsStress;
            this.PhysicsStressMax = template.PhysicsStressMax;
            this.MentalStress = template.MentalStress;
            this.MentalStressMax = template.MentalStressMax;
        }
    }

    public sealed class CommonCharacter : Character
    {
        private static readonly ReadonlyCharacterPropertyList<Consequence> _consequences = new ReadonlyCharacterPropertyList<Consequence>(null, null);

        private readonly CharacterPropertyList<Aspect> _aspects;
        private readonly CharacterPropertyList<Skill> _skills;
        private int _physicsStress = 0;
        private int _physicsStressMax = 0;
        private int _mentalStress = 0;
        private int _mentalStressMax = 0;

        private int _refresh = 0;
        private int _fate = 0;
        private readonly CharacterPropertyList<Stunt> _stunts;
        private readonly CharacterPropertyList<Extra> _extras;

        public override CharacterPropertyList<Aspect> Aspects => _aspects;
        public override CharacterPropertyList<Skill> Skills => _skills;
        public override int PhysicsStress { get => _physicsStress; set => _physicsStress = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Physics stress is less than 0."); }
        public override int PhysicsStressMax { get => _physicsStressMax; set => _physicsStressMax = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Max physics stress is less than 1."); }
        public override int MentalStress { get => _mentalStress; set => _mentalStress = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Mental stress is less than 0."); }
        public override int MentalStressMax { get => _mentalStressMax; set => _mentalStressMax = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Max mental stress is less than 1."); }

        public override CharacterPropertyList<Stunt> Stunts => _stunts;
        public override CharacterPropertyList<Extra> Extras => _extras;
        public override int Refresh { get => _refresh; set => _refresh = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Refresh point is less than 1."); }
        public override int Fate { get => _fate; set => _fate = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Fate point is less than 0."); }

        public override CharacterPropertyList<Consequence> Consequences => _consequences;

        public CommonCharacter(string id, CharacterView view) :
            base(id, view)
        {
            _aspects = new CharacterPropertyList<Aspect>(this);
            _skills = new CharacterPropertyList<Skill>(this);
            _stunts = new CharacterPropertyList<Stunt>(this);
            _extras = new CharacterPropertyList<Extra>(this);
        }

    }

    public sealed class KeyCharacter : Character
    {
        private readonly CharacterPropertyList<Aspect> _aspects;
        private readonly CharacterPropertyList<Skill> _skills;
        private int _physicsStress = 0;
        private int _physicsStressMax = 0;
        private int _mentalStress = 0;
        private int _mentalStressMax = 0;

        private int _refresh = 0;
        private int _fate = 0;
        private readonly CharacterPropertyList<Stunt> _stunts;
        private readonly CharacterPropertyList<Extra> _extras;

        private readonly CharacterPropertyList<Consequence> _consequences;

        public override CharacterPropertyList<Aspect> Aspects => _aspects;
        public override CharacterPropertyList<Skill> Skills => _skills;
        public override int PhysicsStress { get => _physicsStress; set => _physicsStress = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Physics stress is less than 0."); }
        public override int PhysicsStressMax { get => _physicsStressMax; set => _physicsStressMax = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Max physics stress is less than 1."); }
        public override int MentalStress { get => _mentalStress; set => _mentalStress = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Mental stress is less than 0."); }
        public override int MentalStressMax { get => _mentalStressMax; set => _mentalStressMax = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Max mental stress is less than 1."); }

        public override CharacterPropertyList<Stunt> Stunts => _stunts;
        public override CharacterPropertyList<Extra> Extras => _extras;
        public override int Refresh { get => _refresh; set => _refresh = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Refresh point is less than 1."); }
        public override int Fate { get => _fate; set => _fate = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Fate point is less than 0."); }

        public override CharacterPropertyList<Consequence> Consequences => _consequences;

        public KeyCharacter(string id, CharacterView view) :
            base(id, view)
        {
            _aspects = new CharacterPropertyList<Aspect>(this);
            _skills = new CharacterPropertyList<Skill>(this);
            _stunts = new CharacterPropertyList<Stunt>(this);
            _extras = new CharacterPropertyList<Extra>(this);
            _consequences = new CharacterPropertyList<Consequence>(this);
        }

    }

    public sealed class CharacterManager : IJSContextProvider
    {
        #region Javascript API class
        private sealed class API : IJSAPI<CharacterManager>
        {
            private readonly CharacterManager _outer;

            public API(CharacterManager outer)
            {
                _outer = outer;
            }

            public CharacterManager Origin(JSContextHelper proof)
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

        private ulong _autoIncrement = 0L;

        public enum DataLevel
        {
            Temporary,
            Common,
            Key
        }

        private static readonly CharacterManager _instance = new CharacterManager();
        public static CharacterManager Instance => _instance;
        
        private readonly IdentifiedObjList<Character> _savingCharacters;
        private readonly IdentifiedObjList<Character> _playerCharacters;
        
        public IdentifiedObjList<Character> SavingCharacters => _savingCharacters;
        public IdentifiedObjList<Character> PlayerCharacters => _playerCharacters;

        private CharacterManager()
        {
            _savingCharacters = new IdentifiedObjList<Character>();
            _playerCharacters = new IdentifiedObjList<Character>();
            _apiObj = new API(this);
        }
        
        public Character CreateCharacterWithSaving(DataLevel dataLevel, string id, CharacterView view)
        {
            Character ret = CreateCharacter(dataLevel, id, view);
            _savingCharacters.Add(ret);
            return ret;
        }

        public Character CreateTemporaryCharacter(DataLevel dataLevel, CharacterView view)
        {
            string id = "Character_" + _autoIncrement++;
            Character ret = CreateCharacter(dataLevel, id, view);
            return ret;
        }

        private Character CreateCharacter(DataLevel dataLevel, string id, CharacterView view)
        {
            Character ret = null;
            switch (dataLevel)
            {
                case DataLevel.Temporary:
                    ret = new TemporaryCharacter(id, view);
                    break;
                case DataLevel.Common:
                    ret = new CommonCharacter(id, view);
                    break;
                case DataLevel.Key:
                    ret = new KeyCharacter(id, view);
                    break;
                default:
                    break;
            }
            return ret;
        }

        public Aspect CreateAspect()
        {
            Aspect ret = new Aspect();
            return ret;
        }

        public Consequence CreateConsequence()
        {
            Consequence ret = new Consequence();
            return ret;
        }

        public Skill CreateSkill()
        {
            Skill ret = new Skill(SkillType.Athletics);
            throw new NotImplementedException();
        }
        
        public Stunt CreateStunt()
        {
            throw new NotImplementedException();
        }

        public Extra CreateExtra()
        {
            throw new NotImplementedException();
        }

        public InitiativeEffect CreateInitiativeEffect()
        {
            throw new NotImplementedException();
        }

        public PassiveEffect CreatePassiveEffect()
        {
            throw new NotImplementedException();
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
