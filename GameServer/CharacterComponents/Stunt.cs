using GameServer.Core;
using GameServer.Core.ScriptSystem;
using GameServer.EventSystem;
using GameUtil;
using System;
using System.Collections.Generic;

namespace GameServer.CharacterComponents {
	public interface IStuntProperty : IAttachable<Stunt> { }

	public class StuntPropertyList<T> : AttachableList<Stunt, T> where T : class, IStuntProperty {
		private new class JSAPI : AttachableList<Stunt, T>.JSAPI, IJSAPI<StuntPropertyList<T>> {
			private readonly StuntPropertyList<T> _outer;

			public JSAPI(StuntPropertyList<T> outer) : base(outer) {
				_outer = outer;
			}

			StuntPropertyList<T> IJSAPI<StuntPropertyList<T>>.Origin(JSContextHelper proof) {
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

		private readonly JSAPI _apiObj;

		public StuntPropertyList(Stunt owner) : base(owner) {
			_apiObj = new JSAPI(this);
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}
	}
	
	public sealed class Stunt : AutogenIdentifiable, ICharacterProperty {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<Stunt> {
			private readonly Stunt _outer;

			public JSAPI(Stunt outer) {
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

			public void setName(string value) {
				try {
					_outer.Name = value;
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

			public IJSAPI<Character> getBelong() {
				try {
					if (_outer.Belong == null) return null;
					return (IJSAPI<Character>)_outer.Belong.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Condition> getUsingCondition() {
				try {
					if (_outer.UsingCondition == null) return null;
					return (IJSAPI<Condition>)_outer.UsingCondition.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setUsingCondition(IJSAPI<Condition> val) {
				try {
					if (val == null) _outer.UsingCondition = null;
					else _outer.UsingCondition = JSContextHelper.Instance.GetAPIOrigin(val);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<Condition> getTargetCondition() {
				try {
					if (_outer.TargetCondition == null) return null;
					return (IJSAPI<Condition>)_outer.TargetCondition.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setTargetCondition(IJSAPI<Condition> val) {
				try {
					if (val == null) _outer.TargetCondition = null;
					else _outer.TargetCondition = JSContextHelper.Instance.GetAPIOrigin(val);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<StuntEffect> getEffect() {
				try {
					if (_outer.TargetCondition == null) return null;
					return (IJSAPI<StuntEffect>)_outer.TargetCondition.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setEffect(IJSAPI<StuntEffect> val) {
				try {
					_outer.Effect = JSContextHelper.Instance.GetAPIOrigin(val);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}
			
			public SkillBattleMapProperty getBattleMapSkillProperty() {
				try {
					return _outer.BattleMapProperty;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return new SkillBattleMapProperty();
				}
			}

			public void setBattleMapSkillProperty(SkillBattleMapProperty val) {
				try {
					_outer.BattleMapProperty = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public StuntSituationLimit getSituationLimit() {
				try {
					return _outer.SituationLimit;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return new StuntSituationLimit();
				}
			}

			public void setSituationLimit(StuntSituationLimit val) {
				try {
					_outer.SituationLimit = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}
			
			public void setCustomData(object value) {
				_outer._customData = value;
			}

			public object getCustomData() {
				return _outer._customData;
			}

			public Stunt Origin(JSContextHelper proof) {
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

		private string _name = "";
		private string _description = "";
		private Character _belong = null;
		private Condition _usingCondition = null;
		private Condition _targetCondition = null;
		private int _targetMaxCount = 1;
		private StuntEffect _stuntEffect;
		private readonly List<Trigger> _triggersInvoking;
		private SkillBattleMapProperty _battleMapProperty;
		private StuntSituationLimit _situationLimit;
		private readonly List<SkillType> _overcome = new List<SkillType>();
		private readonly List<SkillType> _evade = new List<SkillType>();
		private readonly List<SkillType> _defend = new List<SkillType>();
		private object _customData = null;

		public Stunt(StuntEffect effect, string name = "", string description = "") {
			if (effect == null) throw new ArgumentNullException(nameof(effect));
			if (effect.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(effect));
			_stuntEffect = effect;
			effect.SetBelong(this);
			_name = name ?? throw new ArgumentNullException(nameof(name));
			_description = description ?? throw new ArgumentNullException(nameof(description));
			_battleMapProperty = SkillBattleMapProperty.INIT;
			_triggersInvoking = new List<Trigger>();
			_apiObj = new JSAPI(this);
		}

		protected override string BaseID => "Stunt";

		public override string Name { get => _name; set => _name = value ?? throw new ArgumentNullException(nameof(value)); }
		public override string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(value)); }
		public Character Belong => _belong;
		public Condition UsingCondition {
			get => _usingCondition;
			set {
				if (value != null) {
					if (value.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(value));
					if (_usingCondition != null) _usingCondition.SetBelong(null);
					_usingCondition = value;
					value.SetBelong(this);
				} else {
					_usingCondition = value;
				}
			}
		}
		public Condition TargetCondition {
			get => _targetCondition;
			set {
				if (value != null) {
					if (value.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(value));
					if (_targetCondition != null) _targetCondition.SetBelong(null);
					_targetCondition = value;
					value.SetBelong(this);
				} else {
					_targetCondition = value;
				}
			}
		}
		public int TargetMaxCount { get => _targetMaxCount; set => _targetMaxCount = value; }
		public StuntEffect Effect {
			get => _stuntEffect;
			set {
				if (value == null) throw new ArgumentNullException(nameof(value));
				if (value.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(value));
				_stuntEffect.SetBelong(null);
				_stuntEffect = value;
				value.SetBelong(this);
			}
		}
		public SkillBattleMapProperty BattleMapProperty { get => _battleMapProperty; set => _battleMapProperty = value; }
		public StuntSituationLimit SituationLimit { get => _situationLimit; set => _situationLimit = value; }
		public List<SkillType> Overcome => _overcome;
		public List<SkillType> Evade => _evade;
		public List<SkillType> Defend => _defend;

		public bool CanResistSkillWithoutDMCheck(SkillType initiativeUsing, CharacterAction action) {
			List<SkillType> resistTable;
			switch (action) {
				case CharacterAction.CREATE_ASPECT:
					resistTable = _evade;
					break;
				case CharacterAction.ATTACK:
					resistTable = _defend;
					break;
				case CharacterAction.HINDER:
					resistTable = _overcome;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(action));
			}
			return resistTable.Contains(initiativeUsing);
		}

		public void CopyResistTable(SkillType skillType) {
			if (SkillType.DEFEND.TryGetValue(skillType, out var defend)) _defend.AddRange(defend);
			if (SkillType.EVADE.TryGetValue(skillType, out var evade)) _evade.AddRange(evade);
			if (SkillType.OVERCOME.TryGetValue(skillType, out var overcome)) _overcome.AddRange(overcome);
		}

		public void BindEvent(string eventID) {
			var trigger = new Trigger(eventID, new Command(() => {
				if (_belong == null) return;
				var situation = new Situation() {
					IsTriggerInvoking = true, EventID = eventID,
					IsInStoryScene = false, IsInitiative = false,
					InitiativeSS = null, InitiativeBS = null,
					PassivesSS = null, PassivesBS = null,
					Action = 0,
					InitiativeSkillType = null
				};
				if (_usingCondition != null) {
					_usingCondition.Situation = situation;
					if (!_usingCondition.Judge()) return;
				}
				_stuntEffect.Situation = situation;
				_stuntEffect.TakeEffect((success, message) => {
					if (success) {
						foreach (Player player in Game.Players) {
							player.Client.SkillChecker.DisplayUsingStunt(_belong, this);
						}
						Game.DM.Client.SkillChecker.DisplayUsingStunt(_belong, this);
					}
				});
			}));
			_triggersInvoking.Add(trigger);
		}

		public void SetBelong(Character belong) {
			_belong = belong;
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}

		public override void SetContext(IJSContext context) { }
	}
}
