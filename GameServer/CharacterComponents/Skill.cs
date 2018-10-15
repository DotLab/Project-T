using GameServer.Core;
using GameServer.Core.ScriptSystem;
using GameUtil;
using System;
using System.Collections.Generic;

namespace GameServer.CharacterComponents {
	public sealed class Skill : IJSContextProvider, IDescribable, ICloneable {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<Skill> {
			private readonly Skill _outer;

			public JSAPI(Skill outer) {
				_outer = outer;
			}

			public string getName() {
				try {
					return _outer.Name;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setName(string val) {
				try {
					_outer.Name = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<SkillType> getSkillType() {
				try {
					return (IJSAPI<SkillType>)_outer.SkillType.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public int getLevel() {
				try {
					return _outer.Level;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return 0;
				}
			}

			public void setLevel(int val) {
				try {
					_outer.Level = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public SkillBattleMapProperty getBattleMapProperty() {
				try {
					return _outer.BattleMapProperty;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return new SkillBattleMapProperty();
				}
			}

			public void setBattleMapProperty(SkillBattleMapProperty val) {
				try {
					_outer.BattleMapProperty = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public SkillSituationLimit getSituationLimit() {
				try {
					return _outer.SituationLimit;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return new SkillSituationLimit();
				}
			}

			public void setSituationLimit(SkillSituationLimit val) {
				try {
					_outer.SituationLimit = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<Skill> clone() {
				try {
					return (IJSAPI<Skill>)_outer.Clone().GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public Skill Origin(JSContextHelper proof) {
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

		private readonly SkillType _skillType;
		private string _name;
		private bool _nameUseRef = true;
		private int _level;
		private bool _levelUseRef = true;
		private int _targetMaxCount;
		private bool _targetMaxCountUseRef = true;
		private bool _damageMental = false;
		private bool _damageMentalUseRef = true;
		private SkillBattleMapProperty _battleMapProperty;
		private bool _battleMapPropertyUseRef = true;
		private SkillSituationLimit _situationLimit;
		private bool _situationLimitUseRef = true;

		public string Name { get => _nameUseRef ? _skillType.Name : _name; set { _name = value; _nameUseRef = false; } }
		public string Description { get => ""; set { } }
		public SkillType SkillType => _skillType;
		public int Level { get => _levelUseRef ? _skillType.Level : _level; set { _level = value; _levelUseRef = false; } }
		public int TargetMaxCount { get => _targetMaxCountUseRef ? _skillType.TargetMaxCount : _targetMaxCount; set { _targetMaxCount = value; _targetMaxCountUseRef = false; } }
		public bool DamageMental { get => _damageMentalUseRef ? _skillType.DamageMental : _damageMental; set { _damageMental = value; _damageMentalUseRef = false; } }
		public SkillBattleMapProperty BattleMapProperty { get => _battleMapPropertyUseRef ? _skillType.BattleMapProperty : _battleMapProperty; set { _battleMapProperty = value; _battleMapPropertyUseRef = false; } }
		public SkillSituationLimit SituationLimit { get => _situationLimitUseRef ? _skillType.SituationLimit : _situationLimit; set { _situationLimit = value; _situationLimitUseRef = false; } }

		public Skill(SkillType skillType) {
			_skillType = skillType ?? throw new ArgumentNullException(nameof(skillType));
			_name = skillType.Name;
			_level = skillType.Level;
			_targetMaxCount = skillType.TargetMaxCount;
			_damageMental = skillType.DamageMental;
			_battleMapProperty = skillType.BattleMapProperty;
			_situationLimit = skillType.SituationLimit;
			_apiObj = new JSAPI(this);
		}

		public Skill Clone() {
			var ret = new Skill(_skillType);
			ret._name = _name;
			ret._nameUseRef = _nameUseRef;
			ret._level = _level;
			ret._levelUseRef = _levelUseRef;
			ret._targetMaxCount = _targetMaxCount;
			ret._targetMaxCountUseRef = _targetMaxCountUseRef;
			ret._damageMental = _damageMental;
			ret._damageMentalUseRef = _damageMentalUseRef;
			ret._battleMapProperty = _battleMapProperty;
			ret._battleMapPropertyUseRef = _battleMapPropertyUseRef;
			ret._situationLimit = _situationLimit;
			ret._situationLimitUseRef = _situationLimitUseRef;
			return ret;
		}

		public void ResetLevel() {
			_levelUseRef = true;
		}

		public void ResetBattleMapProperty() {
			_battleMapPropertyUseRef = true;
		}

		public void ResetSituationLimit() {
			_situationLimitUseRef = true;
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }

		object ICloneable.Clone() {
			return Clone();
		}
	}

	public sealed class SkillType : IJSContextProvider, IEquatable<SkillType> {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<SkillType> {
			private readonly SkillType _outer;

			public JSAPI(SkillType outer) {
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

			public SkillType Origin(JSContextHelper proof) {
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

		public static readonly SkillType Athletics = new SkillType("Athletics", "运动");
		public static readonly SkillType Burglary = new SkillType("Burglary", "盗窃");
		public static readonly SkillType Contacts = new SkillType("Contacts", "人脉");
		public static readonly SkillType Crafts = new SkillType("Crafts", "工艺");
		public static readonly SkillType Deceive = new SkillType("Deceive", "欺诈");
		public static readonly SkillType Drive = new SkillType("Drive", "驾驶");
		public static readonly SkillType Empathy = new SkillType("Empathy", "共情");
		public static readonly SkillType Fight = new SkillType("Fight", "战斗");
		public static readonly SkillType Investigate = new SkillType("Investigate", "调查");
		public static readonly SkillType Lore = new SkillType("Lore", "学识");
		public static readonly SkillType Notice = new SkillType("Notice", "洞察");
		public static readonly SkillType Physique = new SkillType("Physique", "体格");
		public static readonly SkillType Provoke = new SkillType("Provoke", "威胁");
		public static readonly SkillType Rapport = new SkillType("Rapport", "交际");
		public static readonly SkillType Resources = new SkillType("Resources", "资源");
		public static readonly SkillType Shoot = new SkillType("Shoot", "射击");
		public static readonly SkillType Stealth = new SkillType("Stealth", "潜行");
		public static readonly SkillType Will = new SkillType("Will", "意志");

		public static readonly SkillType Hinder = new SkillType("Hinder", "阻碍");

		private static readonly Dictionary<string, SkillType> skillTypes = new Dictionary<string, SkillType>();
		public static Dictionary<string, SkillType> SkillTypes => skillTypes;

		private static readonly Dictionary<SkillType, List<SkillType>> _OVERCOME = new Dictionary<SkillType, List<SkillType>>();
		private static readonly Dictionary<SkillType, List<SkillType>> _EVADE = new Dictionary<SkillType, List<SkillType>>();
		private static readonly Dictionary<SkillType, List<SkillType>> _DEFEND = new Dictionary<SkillType, List<SkillType>>();

		public static Dictionary<SkillType, List<SkillType>> OVERCOME => _OVERCOME;
		public static Dictionary<SkillType, List<SkillType>> EVADE => _EVADE;
		public static Dictionary<SkillType, List<SkillType>> DEFEND => _DEFEND;

		static SkillType() {
			Athletics._situationLimit.resistableSituation |= CharacterAction.ATTACK;
			Fight._situationLimit.usableSituation |= CharacterAction.ATTACK;
			Fight._situationLimit.resistableSituation |= CharacterAction.ATTACK;
			Fight._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 1, highOpen = false };
			Physique._situationLimit.resistableSituation |= CharacterAction.ATTACK;
			Physique._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 1, highOpen = false };
			Provoke._situationLimit.usableSituation |= CharacterAction.ATTACK;
			Provoke._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 8, highOpen = false };
			Will._situationLimit.resistableSituation |= CharacterAction.ATTACK;
			Will._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 1, highOpen = false };
			Lore._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 8, highOpen = false };
			Burglary._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 1, highOpen = false };
			Contacts._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 8, highOpen = false };
			Crafts._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 1, highOpen = false };
			Deceive._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 8, highOpen = false };
			Empathy._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 8, highOpen = false };
			Investigate._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 1, highOpen = false };
			Notice._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 8, highOpen = false };
			Rapport._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 8, highOpen = false };
			Resources._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 1, highOpen = false };
			Shoot._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 8, highOpen = false };
			Shoot._battleMapProperty.islinearUse = true;
			Stealth._battleMapProperty.useRange = new Range() { low = 0, lowOpen = false, high = 1, highOpen = false };

			skillTypes.Add(Athletics.ID, Athletics);
			skillTypes.Add(Burglary.ID, Burglary);
			skillTypes.Add(Contacts.ID, Contacts);
			skillTypes.Add(Crafts.ID, Crafts);
			skillTypes.Add(Deceive.ID, Deceive);
			skillTypes.Add(Drive.ID, Drive);
			skillTypes.Add(Empathy.ID, Empathy);
			skillTypes.Add(Fight.ID, Fight);
			skillTypes.Add(Investigate.ID, Investigate);
			skillTypes.Add(Lore.ID, Lore);
			skillTypes.Add(Notice.ID, Notice);
			skillTypes.Add(Physique.ID, Physique);
			skillTypes.Add(Provoke.ID, Provoke);
			skillTypes.Add(Rapport.ID, Rapport);
			skillTypes.Add(Resources.ID, Resources);
			skillTypes.Add(Shoot.ID, Shoot);
			skillTypes.Add(Stealth.ID, Stealth);
			skillTypes.Add(Will.ID, Will);

			Hinder._situationLimit.usableSituation = 0;
			Hinder._situationLimit.resistableSituation = 0;

			skillTypes.Add(Hinder.ID, Hinder);

			////////////////////////////////////////////////////////////////////////////

			_OVERCOME.Add(Athletics, new List<SkillType>(new SkillType[] { Athletics, Physique }));
			_OVERCOME.Add(Burglary, new List<SkillType>(new SkillType[] { }));
			_OVERCOME.Add(Contacts, new List<SkillType>(new SkillType[] { Contacts, Deceive, Investigate }));
			_OVERCOME.Add(Crafts, new List<SkillType>(new SkillType[] { }));
			_OVERCOME.Add(Deceive, new List<SkillType>(new SkillType[] { Deceive, Investigate, Lore }));
			_OVERCOME.Add(Drive, new List<SkillType>(new SkillType[] { Drive }));
			_OVERCOME.Add(Empathy, new List<SkillType>(new SkillType[] { Deceive }));
			_OVERCOME.Add(Fight, new List<SkillType>(new SkillType[] { Fight }));
			_OVERCOME.Add(Investigate, new List<SkillType>(new SkillType[] { Deceive }));
			_OVERCOME.Add(Lore, new List<SkillType>(new SkillType[] { Deceive }));
			_OVERCOME.Add(Notice, new List<SkillType>(new SkillType[] { Burglary, Stealth }));
			_OVERCOME.Add(Physique, new List<SkillType>(new SkillType[] { Physique, Fight, Shoot, Crafts }));
			_OVERCOME.Add(Provoke, new List<SkillType>(new SkillType[] { Provoke }));
			_OVERCOME.Add(Rapport, new List<SkillType>(new SkillType[] { Rapport }));
			_OVERCOME.Add(Resources, new List<SkillType>(new SkillType[] { Resources }));
			_OVERCOME.Add(Shoot, new List<SkillType>(new SkillType[] { Athletics, Drive }));
			_OVERCOME.Add(Stealth, new List<SkillType>(new SkillType[] { Notice, Investigate }));
			_OVERCOME.Add(Will, new List<SkillType>(new SkillType[] { Provoke }));

			_EVADE.Add(Athletics, new List<SkillType>(new SkillType[] { Athletics, Fight, Shoot, Physique }));
			_EVADE.Add(Burglary, new List<SkillType>(new SkillType[] { }));
			_EVADE.Add(Contacts, new List<SkillType>(new SkillType[] { Contacts, Deceive }));
			_EVADE.Add(Crafts, new List<SkillType>(new SkillType[] { }));
			_EVADE.Add(Deceive, new List<SkillType>(new SkillType[] { Deceive, Investigate, Lore }));
			_EVADE.Add(Drive, new List<SkillType>(new SkillType[] { Drive }));
			_EVADE.Add(Empathy, new List<SkillType>(new SkillType[] { Deceive }));
			_EVADE.Add(Fight, new List<SkillType>(new SkillType[] { Fight }));
			_EVADE.Add(Investigate, new List<SkillType>(new SkillType[] { Deceive }));
			_EVADE.Add(Lore, new List<SkillType>(new SkillType[] { Deceive }));
			_EVADE.Add(Notice, new List<SkillType>(new SkillType[] { Burglary, Stealth }));
			_EVADE.Add(Physique, new List<SkillType>(new SkillType[] { Physique, Fight, Shoot, Crafts }));
			_EVADE.Add(Provoke, new List<SkillType>(new SkillType[] { Provoke }));
			_EVADE.Add(Rapport, new List<SkillType>(new SkillType[] { Rapport }));
			_EVADE.Add(Resources, new List<SkillType>(new SkillType[] { Resources }));
			_EVADE.Add(Shoot, new List<SkillType>(new SkillType[] { Athletics, Drive }));
			_EVADE.Add(Stealth, new List<SkillType>(new SkillType[] { Notice, Investigate }));
			_EVADE.Add(Will, new List<SkillType>(new SkillType[] { Provoke }));

			_DEFEND.Add(Athletics, new List<SkillType>(new SkillType[] { Fight, Shoot }));
			_DEFEND.Add(Physique, new List<SkillType>(new SkillType[] { Fight, Shoot }));
			_DEFEND.Add(Fight, new List<SkillType>(new SkillType[] { Fight }));
			_DEFEND.Add(Will, new List<SkillType>(new SkillType[] { Provoke }));
		}

		public static bool CanInitiativeUseSkill(Character initiative, SkillType skillType, CharacterAction action) {
			var situationLimit = initiative.GetSkill(skillType).SituationLimit;
			if ((situationLimit.usableSituation & action) == 0) return false;
			else return true;
		}

		public static bool CanPassiveUseSkill(Character passive, SkillType skillType, CharacterAction action) {
			var situationLimit = passive.GetSkill(skillType).SituationLimit;
			if ((situationLimit.resistableSituation & action) == 0) return false;
			else return true;
		}

		public static bool CanResistSkillWithoutDMCheck(SkillType initiativeUsing, SkillType resist, CharacterAction action) {
			Dictionary<SkillType, List<SkillType>> resistTable;
			switch (action) {
				case CharacterAction.CREATE_ASPECT:
					resistTable = _EVADE;
					break;
				case CharacterAction.ATTACK:
					resistTable = _DEFEND;
					break;
				case CharacterAction.HINDER:
					resistTable = _OVERCOME;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(action));
			}
			if (resistTable.TryGetValue(resist, out List<SkillType> initiativeSkills)) return initiativeSkills.Contains(initiativeUsing);
			else return false;
		}

		private readonly string _id;
		private readonly string _name;
		private int _level = 0;
		private int _targetMaxCount = 1;
		private bool _damageMental = false;
		private SkillBattleMapProperty _battleMapProperty = SkillBattleMapProperty.INIT;
		private SkillSituationLimit _situationLimit = SkillSituationLimit.INIT;

		public string ID => _id;
		public string Name => _name;
		public int Level => _level;
		public int TargetMaxCount => _targetMaxCount;
		public bool DamageMental => _damageMental;
		public SkillBattleMapProperty BattleMapProperty => _battleMapProperty;
		public SkillSituationLimit SituationLimit => _situationLimit;

		private SkillType(string id, string name) {
			_id = id ?? throw new ArgumentNullException(nameof(id));
			_name = name ?? throw new ArgumentNullException(nameof(name));
			_apiObj = new JSAPI(this);
		}

		public bool Equals(SkillType other) {
			return !(other is null) && _id == other._id;
		}

		public override bool Equals(object obj) {
			return Equals(obj as SkillType);
		}

		public override int GetHashCode() {
			return _id.GetHashCode();
		}
		
		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}
}
