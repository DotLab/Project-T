using GameLib.Core;
using GameLib.Core.ScriptSystem;
using GameLib.Utilities;
using System;

namespace GameLib.CharacterSystem {
	public interface IStuntProperty : IAttachable<Stunt> { }

	public class StuntPropertyList<T> : AttachableList<Stunt, T> where T : class, IStuntProperty {
		public StuntPropertyList(Stunt owner) : base(owner) { }
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
					if (_outer.Belong != null) return (IJSAPI<Character>)_outer.Belong.GetContext();
					else return null;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setInitiativeEffect(IJSAPI<InitiativeEffect> effect) {
				try {
					_outer.Effect = JSContextHelper.Instance.GetAPIOrigin(effect);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<InitiativeEffect> getInitiativeEffect() {
				try {
					return (IJSAPI<InitiativeEffect>)_outer.Effect.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
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
		private Func<Character, Character, CharacterAction, SkillType, SkillType, bool, bool> _condition = null;
		private InitiativeEffect _initiativeEffect;
		private readonly StuntPropertyList<PassiveEffect> _passiveEffects;
		private SkillType _boundSkillType;
		private SkillProperty _skillProperty;
		private readonly bool _needDMCheck;

		public Stunt(InitiativeEffect effect, SkillType boundSkillType, bool needDMCheck = true, string name = "", string description = "") {
			if (effect == null) throw new ArgumentNullException(nameof(effect));
			if (effect.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(effect));
			_initiativeEffect = effect;
			effect.SetBelong(this);
			_boundSkillType = boundSkillType ?? throw new ArgumentNullException(nameof(boundSkillType));
			_skillProperty = _boundSkillType.Property;
			_needDMCheck = needDMCheck;
			_name = name ?? throw new ArgumentNullException(nameof(name));
			_description = description ?? throw new ArgumentNullException(nameof(description));
			_passiveEffects = new StuntPropertyList<PassiveEffect>(this);
			_apiObj = new JSAPI(this);
		}

		protected override string BaseID => "Stunt";

		public override string Name { get => _name; set => _name = value ?? throw new ArgumentNullException(nameof(value)); }
		public override string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(value)); }
		public Character Belong => _belong;
		public Func<Character, Character, CharacterAction, SkillType, SkillType, bool, bool> Condition { get => _condition; set => _condition = value; }
		public InitiativeEffect Effect {
			get => _initiativeEffect;
			set {
				if (value == null) throw new ArgumentNullException(nameof(value));
				if (value.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(value));
				_initiativeEffect.SetBelong(null);
				_initiativeEffect = value;
				value.SetBelong(this);
			}
		}
		public StuntPropertyList<PassiveEffect> PassiveEffects => _passiveEffects;
		public SkillType BoundSkillType { get => _boundSkillType; set => _boundSkillType = value ?? throw new ArgumentNullException(nameof(value)); }
		public SkillProperty SkillProperty { get => _skillProperty; set => _skillProperty = value; }
		public bool NeedDMCheck => _needDMCheck;

		public void SetBelong(Character belong) {
			_belong = belong;
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}

		public override void SetContext(IJSContext context) { }
	}
}
