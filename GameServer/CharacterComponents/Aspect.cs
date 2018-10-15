using GameServer.Core;
using GameServer.Core.ScriptSystem;
using System;

namespace GameServer.CharacterComponents {
	public enum PersistenceType {
		Fixed = 0,
		Common = 1,
		Temporary = 2
	}

	public class Aspect : AutogenIdentifiable, ICharacterProperty {
		#region Javascript API class
		protected class JSAPI : IJSAPI<Aspect> {
			private readonly Aspect _outer;

			public JSAPI(Aspect outer) {
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

			public void setDescription(string value) {
				try {
					_outer.Description = value;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getPersistenceType() {
				try {
					return (int)_outer.PersistenceType;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setPersistenceType(int val) {
				try {
					_outer.PersistenceType = (PersistenceType)val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<Character> getBenefiter() {
				try {
					if (_outer.Benefiter == null) return null;
					return (IJSAPI<Character>)_outer.Benefiter.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setBenefiter(IJSAPI<Character> val) {
				try {
					if (val == null) _outer.Benefiter = null;
					else _outer.Benefiter = JSContextHelper.Instance.GetAPIOrigin(val);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getBenefitTimes() {
				try {
					return _outer.BenefitTimes;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setBenefitTimes(int val) {
				try {
					_outer.BenefitTimes = val;
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

			public Aspect Origin(JSContextHelper proof) {
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

		protected string _name = "";
		protected string _description = "";
		protected PersistenceType _persistenceType = PersistenceType.Common;
		protected Character _benefiter = null;
		protected int _benefitTimes = 0;
		protected Character _belong = null;

		public Aspect() {
			_apiObj = new JSAPI(this);
		}

		protected override string BaseID => "Aspect";

		public override string Name { get => _name; set => _name = value ?? throw new ArgumentNullException(nameof(value)); }
		public override string Description { get => _description; set => _description = value ?? throw new ArgumentNullException(nameof(value)); }
		public PersistenceType PersistenceType { get => _persistenceType; set => _persistenceType = value; }
		public Character Benefiter {
			get => _benefiter;
			set {
				_benefiter = value;
				if (_benefiter != null && _benefitTimes <= 0) _benefitTimes = 1;
				if (_benefiter == null && _benefitTimes > 0) _benefitTimes = 0;
			}
		}
		public int BenefitTimes {
			get => _benefitTimes;
			set {
				_benefitTimes = value;
				if (_benefitTimes <= 0) _benefiter = null;
			}
		}
		public Character Belong => _belong;

		public void SetBelong(Character belong) {
			_belong = belong;
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}

		public sealed override void SetContext(IJSContext context) { }
	}

	public sealed class Consequence : Aspect {
		#region Javascript API class
		private new class JSAPI : Aspect.JSAPI, IJSAPI<Consequence> {
			private readonly Consequence _outer;

			public JSAPI(Consequence outer) : base(outer) {
				_outer = outer;
			}

			public int getCounteractLevel() {
				try {
					return _outer.CounteractLevel;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setCounteractLevel(int val) {
				try {
					_outer.CounteractLevel = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public bool isMentalDamage() {
				try {
					return _outer.MentalDamage;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public void setMentalDamage(bool val) {
				try {
					_outer.MentalDamage = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			Consequence IJSAPI<Consequence>.Origin(JSContextHelper proof) {
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

		private int _counteractLevel = 0;
		private int _actualDamage = 0;
		private bool _mentalDamage = false;

		public int CounteractLevel {
			get => _counteractLevel;
			set {
				_counteractLevel = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Counteract level is less than 0.");
				if (_actualDamage > _counteractLevel) _actualDamage = _counteractLevel;
			}
		}
		public int ActualDamage { get => _actualDamage; set => _actualDamage = (value >= 0 && value <= _counteractLevel) ? value : throw new ArgumentOutOfRangeException(nameof(value)); }
		public bool MentalDamage { get => _mentalDamage; set => _mentalDamage = value; }

		protected override string BaseID => "Consequence";

		public Consequence() {
			_apiObj = new JSAPI(this);
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}

	}
}
