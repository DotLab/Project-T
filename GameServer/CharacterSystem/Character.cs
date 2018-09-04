using GameLib.Campaign;
using GameLib.Container;
using GameLib.Container.StoryComponent;
using GameLib.Core;
using GameLib.Core.ScriptSystem;
using GameLib.Utilities;
using System;
using System.Collections.Generic;

namespace GameLib.CharacterSystem {
	public interface ICharacterProperty : IIdentifiable, IAttachable<Character> { }
	
	public class CharacterPropertyList<T> : AttachableList<Character, T> where T : class, ICharacterProperty {
		public CharacterPropertyList(Character owner) : base(owner) { }
	}

	public abstract class Character : IIdentifiable, IExtraProperty {
		#region Javascript API class
		protected class JSAPI : IJSAPI<Character> {
			private readonly Character _outer;

			public JSAPI(Character outer) {
				_outer = outer;
			}

			public string getID() {
				try {
					return _outer.ID;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public string getName() {
				try {
					return _outer.Name;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setName(string name) {
				try {
					_outer.Name = name;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public string getDescription() {
				try {
					return _outer.Description;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setDescription(string name) {
				try {
					_outer.Name = name;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<Extra> getBelongExtra() {
				try {
					if (_outer.Belong != null) return (IJSAPI<Extra>)_outer.Belong.GetContext();
					else return null;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<CharacterPropertyList<Aspect>> getAspectList() {
				try {
					return (IJSAPI<CharacterPropertyList<Aspect>>)_outer.Aspects.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public SkillProperty getSkillProperty(string skillID) {
				try {
					return _outer.GetSkillProperty(SkillType.SkillTypes[skillID]);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return new SkillProperty();
				}
			}

			public void setSkillProperty(string skillID, SkillProperty property) {
				try {
					_outer.SetSkillProperty(SkillType.SkillTypes[skillID], property);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<CharacterPropertyList<Stunt>> getStuntList() {
				try {
					if (_outer.Stunts == null) return null;
					return (IJSAPI<CharacterPropertyList<Stunt>>)_outer.Stunts.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<CharacterPropertyList<Extra>> getExtraList() {
				try {
					if (_outer.Extras == null) return null;
					return (IJSAPI<CharacterPropertyList<Extra>>)_outer.Extras.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<CharacterPropertyList<Consequence>> getConsequenceList() {
				try {
					if (_outer.Consequences == null) return null;
					return (IJSAPI<CharacterPropertyList<Consequence>>)_outer.Consequences.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public int getRefreshPoint() {
				try {
					return _outer.RefreshPoint;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setRefreshPoint(int value) {
				try {
					_outer.RefreshPoint = value;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getFatePoint() {
				try {
					return _outer.FatePoint;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setFatePoint(int value) {
				try {
					_outer.FatePoint = value;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getPhysicsStress() {
				try {
					return _outer.PhysicsStress;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setPhysicsStress(int value) {
				try {
					_outer.PhysicsStress = value;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getPhysicsStressMax() {
				try {
					return _outer.PhysicsStressMax;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setPhysicsStressMax(int value) {
				try {
					_outer.PhysicsStressMax = value;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getMentalStress() {
				try {
					return _outer.MentalStress;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setMentalStress(int value) {
				try {
					_outer.MentalStress = value;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getMentalStressMax() {
				try {
					return _outer.MentalStressMax;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setMentalStressMax(int value) {
				try {
					_outer.MentalStressMax = value;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public Character Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;
		
		protected sealed class Skill : IDescribable {
			private readonly SkillType _skillType;
			private SkillProperty _property;
			private SkillSituation _situation;

			public string Name { get => _skillType.Name; set { } }
			public string Description { get => ""; set { } }
			public SkillType SkillType => _skillType;
			public SkillProperty Property { get => _property; set => _property = value; }
			public SkillSituation Situation { get => _situation; set => _situation = value; }

			public Skill(SkillType skillType) {
				_skillType = skillType ?? throw new ArgumentNullException(nameof(skillType));
				_property = skillType.Property;
				_situation = skillType.Situation;
			}
		}

		private readonly string _id;
		private string _name = "";
		private string _description = "";
		private Extra _belong = null;
		private readonly CharacterView _view;
		private Player _controlPlayer = null;
		private bool _dead = false;

		protected readonly List<Skill> _skills;
		private readonly CharacterPropertyList<Aspect> _aspects;
		private int _physicsStress = 0;
		private int _physicsStressMax = 0;
		private bool _physicsInvincible = false;
		private int _mentalStress = 0;
		private int _mentalStressMax = 0;
		private bool _mentalInvincible = false;

		public string ID => _id;
		public string Name { get => _name; set => _name = value ?? throw new ArgumentNullException(nameof(value)); }
		public string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(value)); }
		public Extra Belong => _belong;
		public CharacterView View => _view;
		public Player ControlPlayer { get => _controlPlayer; set => _controlPlayer = value; }
		public User Controller => _controlPlayer ?? (User)Game.DM;
		public bool Dead => _dead;
		
		public CharacterPropertyList<Aspect> Aspects => _aspects;
		public int PhysicsStress { get => _physicsStress; set => _physicsStress = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Physics stress is less than 0."); }
		public int PhysicsStressMax { get => _physicsStressMax; set => _physicsStressMax = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Max physics stress is less than 1."); }
		public bool PhysicsInvincible { get => _physicsInvincible; set => _physicsInvincible = value; }
		public int MentalStress { get => _mentalStress; set => _mentalStress = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Mental stress is less than 0."); }
		public int MentalStressMax { get => _mentalStressMax; set => _mentalStressMax = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Max mental stress is less than 1."); }
		public bool MentalInvincible { get => _mentalInvincible; set => _mentalInvincible = value; }

		public void SetBelong(Extra belong) {
			_belong = belong;
		}

		protected Character(string id, CharacterView view) {
			_id = id ?? throw new ArgumentNullException(nameof(id));
			_view = view ?? throw new ArgumentNullException(nameof(view));
			_skills = new List<Skill>();
			_aspects = new CharacterPropertyList<Aspect>(this);
			_apiObj = new JSAPI(this);
		}

		public abstract int RefreshPoint { get; set; }
		public abstract int FatePoint { get; set; }
		public abstract CharacterPropertyList<Stunt> Stunts { get; }
		public abstract CharacterPropertyList<Extra> Extras { get; }
		public abstract CharacterPropertyList<Consequence> Consequences { get; }
		
		public SkillSituation GetSkillSituation(SkillType skillType) {
			foreach (var skill in _skills) {
				if (skill.SkillType == skillType) {
					return skill.Situation;
				}
			}
			var ret = skillType.Situation;
			if (skillType == SkillType.Shoot) {
				if (this.Extras != null) {
					foreach (var extra in this.Extras) {
						if (extra.IsLongRangeWeapon) {
							ret.canAttack = true;
							break;
						}
					}
				}
			}
			return ret;
		}

		public void SetSkillSituation(SkillType skillType, SkillSituation situation) {
			foreach (var skill in _skills) {
				if (skill.SkillType == skillType) {
					skill.Situation = situation;
					return;
				}
			}
			var newSkill = new Skill(skillType);
			newSkill.Situation = situation;
			_skills.Add(newSkill);
		}
		
		public SkillProperty GetSkillProperty(SkillType skillType) {
			foreach (var skill in _skills) {
				if (skill.SkillType == skillType) {
					return skill.Property;
				}
			}
			return skillType.Property;
		}

		public void SetSkillProperty(SkillType skillType, SkillProperty property) {
			foreach (var skill in _skills) {
				if (skill.SkillType == skillType) {
					skill.Property = property;
					return;
				}
			}
			var newSkill = new Skill(skillType);
			newSkill.Property = property;
			_skills.Add(newSkill);
		}

		public void Damage(int val, bool mental, Character whoCause, string causeMsg) {
			if (val < 0) throw new ArgumentOutOfRangeException("Damage is less than 0.", nameof(val));
			int count_level_2 = 0;
			int count_level_4 = 0;
			int count_level_6 = 0;
			do {
				if (this.Consequences != null) {
					foreach (var consequence in this.Consequences) {
						if (consequence.CounteractLevel == 2) ++count_level_2;
						else if (consequence.CounteractLevel == 4) ++count_level_4;
						else if (consequence.CounteractLevel == 6) ++count_level_6;
					}
					if (val <= 2 && count_level_2 < 2) {
						var slight = new Consequence();
						slight.Name = whoCause.Name + " " + causeMsg;
						slight.CounteractLevel = 2;
						slight.MentalDamage = mental;
						this.Consequences.Add(slight);
					} else if (val <= 4 && count_level_4 < 1) {
						var medium = new Consequence();
						medium.Name = whoCause.Name + " " + causeMsg;
						medium.CounteractLevel = 4;
						medium.MentalDamage = mental;
						this.Consequences.Add(medium);
					} else if (val <= 6 && count_level_6 < 1) {
						var serious = new Consequence();
						serious.Name = whoCause.Name + " " + causeMsg;
						serious.PersistenceType = PersistenceType.Fixed;
						serious.CounteractLevel = 6;
						serious.MentalDamage = mental;
						this.Consequences.Add(serious);
					} else break;
					return;
				}
			} while (false);
			if (mental) {
				_mentalStress += val;
			} else {
				_physicsStress += val;
			}
		}

		public void MarkDead() {
			_dead = true;
		}

		public Aspect FindAspectByID(string id) {
			foreach (var aspect in this.Aspects) {
				if (aspect.ID == id) {
					return aspect;
				}
			}
			return null;
		}

		public Stunt FindStuntByID(string id) {
			if (this.Stunts != null) {
				foreach (var stunt in this.Stunts) {
					if (stunt.ID == id) {
						return stunt;
					}
				}
			}
			return null;
		}

		public Extra FindExtraByID(string id) {
			if (this.Extras != null) {
				foreach (var extra in this.Extras) {
					if (extra.ID == id) {
						return extra;
					}
				}
			}
			return null;
		}

		public Consequence FindConsequenceByID(string id) {
			if (this.Consequences != null) {
				foreach (var consequence in this.Consequences) {
					if (consequence.ID == id) {
						return consequence;
					}
				}
			}
			return null;
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

	public sealed class TemporaryCharacter : Character {
		public override int RefreshPoint { get => 0; set { } }
		public override int FatePoint { get => 0; set { } }
		public override CharacterPropertyList<Stunt> Stunts => null;
		public override CharacterPropertyList<Extra> Extras => null;
		public override CharacterPropertyList<Consequence> Consequences => null;

		public TemporaryCharacter(string id, CharacterView view) :
			base(id, view) {

		}

		public TemporaryCharacter(string id, TemporaryCharacter template) :
			base(id, template.View) {
			this.Name = template.Name;
			this.Description = template.Description;
			this.ControlPlayer = template.ControlPlayer;
			foreach (Aspect aspect in template.Aspects) {
				Aspect clone = new Aspect {
					Name = aspect.Name,
					Description = aspect.Description,
					PersistenceType = aspect.PersistenceType
				};
				this.Aspects.Add(clone);
			}
			foreach (Skill skill in template._skills) {
				Skill clone = new Skill(skill.SkillType) {
					Name = skill.Name,
					Property = skill.Property,
					Situation = skill.Situation
				};
				_skills.Add(skill);
			}
			this.PhysicsStress = template.PhysicsStress;
			this.PhysicsStressMax = template.PhysicsStressMax;
			this.PhysicsInvincible = template.PhysicsInvincible;
			this.MentalStress = template.MentalStress;
			this.MentalStressMax = template.MentalStressMax;
			this.MentalInvincible = template.MentalInvincible;
		}
	}

	public sealed class CommonCharacter : Character {
		private int _refreshPoint = 0;
		private int _fatePoint = 0;
		private readonly CharacterPropertyList<Stunt> _stunts;
		private readonly CharacterPropertyList<Extra> _extras;
		
		public override int RefreshPoint { get => _refreshPoint; set => _refreshPoint = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Refresh point is less than 1."); }
		public override int FatePoint { get => _fatePoint; set => _fatePoint = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Fate point is less than 0."); }
		public override CharacterPropertyList<Stunt> Stunts => _stunts;
		public override CharacterPropertyList<Extra> Extras => _extras;
		public override CharacterPropertyList<Consequence> Consequences => null;

		public CommonCharacter(string id, CharacterView view) :
			base(id, view) {
			_stunts = new CharacterPropertyList<Stunt>(this);
			_extras = new CharacterPropertyList<Extra>(this);
		}

	}

	public sealed class KeyCharacter : Character {
		private int _refreshPoint = 0;
		private int _fatePoint = 0;
		private readonly CharacterPropertyList<Stunt> _stunts;
		private readonly CharacterPropertyList<Extra> _extras;
		private readonly CharacterPropertyList<Consequence> _consequences;

		public override int RefreshPoint { get => _refreshPoint; set => _refreshPoint = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Refresh point is less than 1."); }
		public override int FatePoint { get => _fatePoint; set => _fatePoint = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Fate point is less than 0."); }
		public override CharacterPropertyList<Stunt> Stunts => _stunts;
		public override CharacterPropertyList<Extra> Extras => _extras;
		public override CharacterPropertyList<Consequence> Consequences => _consequences;
		
		public KeyCharacter(string id, CharacterView view) :
			base(id, view) {
			_stunts = new CharacterPropertyList<Stunt>(this);
			_extras = new CharacterPropertyList<Extra>(this);
			_consequences = new CharacterPropertyList<Consequence>(this);
		}

	}

	public sealed class CharacterManager : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<CharacterManager> {
			private readonly CharacterManager _outer;

			public JSAPI(CharacterManager outer) {
				_outer = outer;
			}

			public CharacterManager Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private ulong _autoIncrement = 0L;

		public enum DataLevel {
			Temporary,
			Common,
			Key
		}

		private static readonly CharacterManager _instance = new CharacterManager();
		public static CharacterManager Instance => _instance;

		private readonly IdentifiedObjList<Character> _savingNPCharacters;
		private readonly IdentifiedObjList<Character> _playerCharacters;

		public IdentifiedObjList<Character> SavingNPCharacters => _savingNPCharacters;
		public IdentifiedObjList<Character> PlayerCharacters => _playerCharacters;

		private CharacterManager() {
			_savingNPCharacters = new IdentifiedObjList<Character>();
			_playerCharacters = new IdentifiedObjList<Character>();
			_apiObj = new JSAPI(this);
		}

		public Character FindCharacterOrItemRecursivelyByID(string id) {
			if (CampaignManager.Instance.CurrentContainer == ContainerType.STORY) {
				if (StorySceneContainer.Instance.PlayerCharacters.TryGetValue(id, out Character character)) {
					return character;
				} else if (StorySceneContainer.Instance.ObjList.TryGetValue(id, out ISceneObject storyObject)) {
					return storyObject.CharacterRef;
				} else {
					foreach (var character2 in StorySceneContainer.Instance.PlayerCharacters) {
						var item = this.FindItemRecursivelyByID(character2, id);
						if (item != null) return item;
					}
					foreach (var obj in StorySceneContainer.Instance.ObjList) {
						var item = this.FindItemRecursivelyByID(obj.CharacterRef, id);
						if (item != null) return item;
					}
				}
			} else if (CampaignManager.Instance.CurrentContainer == ContainerType.BATTLE) {
				var sceneObject = BattleSceneContainer.Instance.FindObject(id);
				if (sceneObject != null) return sceneObject.CharacterRef;
				foreach (var sceneObject2 in BattleSceneContainer.Instance.ObjList) {
					var item = this.FindItemRecursivelyByID(sceneObject2.CharacterRef, id);
					if (item != null) return item;
				}
			}
			return null;
		}

		public Character FindCharacterByID(string id) {
			if (CampaignManager.Instance.CurrentContainer == ContainerType.STORY) {
				if (StorySceneContainer.Instance.PlayerCharacters.TryGetValue(id, out Character character)) {
					return character;
				} else if (StorySceneContainer.Instance.ObjList.TryGetValue(id, out ISceneObject storyObject)) {
					return storyObject.CharacterRef;
				}
			} else if (CampaignManager.Instance.CurrentContainer == ContainerType.BATTLE) {
				var sceneObject = BattleSceneContainer.Instance.FindObject(id);
				if (sceneObject != null) return sceneObject.CharacterRef;
			}
			return null;
		}

		public Character FindItemRecursivelyByID(Character owner, string itemID) {
			if (owner.Extras == null) return null;
			var extra = owner.FindExtraByID(itemID);
			if (extra != null) return extra.Item;
			else {
				Character item = this.FindItemRecursivelyByID(extra.Item, itemID);
				if (item != null) return item;
			}
			return null;
		}

		public Character CreateCharacterWithSaving(DataLevel dataLevel, string id, CharacterView view) {
			Character ret = CreateCharacter(dataLevel, id, view);
			_savingNPCharacters.Add(ret);
			return ret;
		}

		public Character CreateTemporaryCharacter(DataLevel dataLevel, CharacterView view) {
			string id = "Character_" + _autoIncrement++;
			Character ret = CreateCharacter(dataLevel, id, view);
			return ret;
		}

		private Character CreateCharacter(DataLevel dataLevel, string id, CharacterView view) {
			Character ret = null;
			switch (dataLevel) {
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
					throw new ArgumentOutOfRangeException(nameof(dataLevel));
			}
			return ret;
		}

		public Aspect CreateAspect() {
			Aspect ret = new Aspect();
			return ret;
		}

		public Consequence CreateConsequence() {
			Consequence ret = new Consequence();
			return ret;
		}
		
		public Stunt CreateStunt() {
			throw new NotImplementedException();
		}

		public Extra CreateExtra() {
			throw new NotImplementedException();
		}

		public InitiativeEffect CreateInitiativeEffect() {
			throw new NotImplementedException();
		}

		public PassiveEffect CreatePassiveEffect() {
			throw new NotImplementedException();
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}
}
