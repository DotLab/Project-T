using GameLib.Core;
using GameLib.Core.ScriptSystem;
using System;

namespace GameLib.CharacterSystem {
	public interface IExtraProperty : IAttachable<Extra> { }

	public class ExtraPropertyList<T> : AttachableList<Extra, T> where T : class, IExtraProperty {
		public ExtraPropertyList(Extra owner) : base(owner) { }
	}

	public sealed class Extra : AutogenIdentifiable, ICharacterProperty {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<Extra> {
			private readonly Extra _outer;

			public JSAPI(Extra outer) {
				_outer = outer;
			}

			public IJSAPI<Character> getItem() {
				try {
					return (IJSAPI<Character>)_outer.Item.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setItem(IJSAPI<Character> item) {
				try {
					_outer.Item = JSContextHelper.Instance.GetAPIOrigin(item);
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

			public void setCustomData(object value) {
				try {
					_outer.CustomData = value;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public object getCustomData() {
				try {
					return _outer.CustomData;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<ExtraPropertyList<PassiveEffect>> getPassiveEffectList() {
				try {
					return (IJSAPI<ExtraPropertyList<PassiveEffect>>)_outer.Effects.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public Extra Origin(JSContextHelper proof) {
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

		private Character _belong = null;
		private Character _item;
		private bool _isTool;
		private bool _isLongRangeWeapon;
		private bool _isVehicle;
		private object _customData = null;
		private readonly ExtraPropertyList<PassiveEffect> _passiveEffects;

		public Extra(Character item) {
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (item.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(item));
			_item = item;
			item.SetBelong(this);
			_passiveEffects = new ExtraPropertyList<PassiveEffect>(this);
			_apiObj = new JSAPI(this);
		}

		protected override string BaseID => "Extra";

		public override string Name { get => _item.Name; set => _item.Name = value; }
		public override string Description { get => _item.Description; set => _item.Description = value; }
		public Character Belong => _belong;
		public Character Item {
			get => _item;
			set {
				if (value == null) throw new ArgumentNullException(nameof(value));
				if (value.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(value));
				_item.SetBelong(null);
				_item = value;
				value.SetBelong(this);
			}
		}
		public bool IsTool { get => _isTool; set { _isTool = value; if (!value) _isLongRangeWeapon = _isVehicle = false; } }
		public bool IsLongRangeWeapon { get => _isLongRangeWeapon; set { _isLongRangeWeapon = value; if (value) _isTool = true; } }
		public bool IsVehicle { get => _isVehicle; set { _isVehicle = value; if (value) _isTool = true; } }
		public object CustomData { get => _customData; set => _customData = value; }
		public ExtraPropertyList<PassiveEffect> Effects => _passiveEffects;

		public void SetBelong(Character belong) {
			_belong = belong;
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}

		public override void SetContext(IJSContext context) { }
	}

}
