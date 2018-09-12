using GameServer.Core;
using GameServer.Core.ScriptSystem;
using System;

namespace GameServer.CharacterSystem {
	public interface IExtraProperty : IAttachable<Extra> { }

	public class ExtraPropertyList<T> : AttachableList<Extra, T> where T : class, IExtraProperty {
		private new class JSAPI : AttachableList<Extra, T>.JSAPI, IJSAPI<ExtraPropertyList<T>> {
			private readonly ExtraPropertyList<T> _outer;

			public JSAPI(ExtraPropertyList<T> outer) : base(outer) {
				_outer = outer;
			}

			ExtraPropertyList<T> IJSAPI<ExtraPropertyList<T>>.Origin(JSContextHelper proof) {
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

		public ExtraPropertyList(Extra owner) : base(owner) {
			_apiObj = new JSAPI(this);
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}
	}

	public sealed class Extra : IIdentifiable, ICharacterProperty {
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
			
			public IJSAPI<Character> getBelong() {
				try {
					if (_outer.Belong != null) return (IJSAPI<Character>)_outer.Belong.GetContext();
					else return null;
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

			public bool isLongRangeWeapon() {
				try {
					return _outer.IsLongRangeWeapon;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public void setCustomData(object value) {
				_outer._customData = value;
			}

			public object getCustomData() {
				return _outer._customData;
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
		private readonly Character _item;
		private bool _isLongRangeWeapon;
		private bool _isVehicle;
		private readonly ExtraPropertyList<PassiveEffect> _passiveEffects;
		private object _customData = null;

		public Extra(Character item) {
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (item.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(item));
			_item = item;
			item.SetBelong(this);
			_passiveEffects = new ExtraPropertyList<PassiveEffect>(this);
			_apiObj = new JSAPI(this);
		}

		public string ID => _item.ID;
		public string Name { get => _item.Name; set => _item.Name = value; }
		public string Description { get => _item.Description; set => _item.Description = value; }
		public Character Belong => _belong;
		public Character Item => _item;
		public bool IsLongRangeWeapon { get => _isLongRangeWeapon; set => _isLongRangeWeapon = value; }
		public bool IsVehicle { get => _isVehicle; set => _isVehicle = value; }
		public ExtraPropertyList<PassiveEffect> Effects => _passiveEffects;

		public void SetBelong(Character belong) {
			_belong = belong;
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

}
