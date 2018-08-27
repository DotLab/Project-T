using GameLib.EventSystem;
using GameLib.Core;
using GameLib.Core.ScriptSystem;
using System;

namespace GameLib.CharacterSystem {
	public sealed class InitiativeEffect : Command, IStuntProperty {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<InitiativeEffect> {
			private readonly InitiativeEffect _outer;

			public JSAPI(InitiativeEffect outer) {
				_outer = outer;
			}

			public IJSAPI<Stunt> getBelongStunt() {
				try {
					if (_outer.Belong != null) return (IJSAPI<Stunt>)_outer.Belong.GetContext();
					else return null;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public InitiativeEffect Origin(JSContextHelper proof) {
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

		private Stunt _belong = null;

		public Stunt Belong => _belong;

		public void SetBelong(Stunt belong) {
			_belong = belong;
		}

		public InitiativeEffect(Action action) : this(false, action, null) { }

		public InitiativeEffect(string jscode) : this(true, null, jscode) { }

		public InitiativeEffect(bool javascript, Action action, string jscode) :
			base(javascript, action, jscode) {
			_apiObj = new JSAPI(this);
		}

		public override void DoAction() {
			JSEngineManager.Engine.SynchronizeContext("$__belongStunt__", _belong);
			base.DoAction();
			JSEngineManager.Engine.RemoveContext("$__belongStunt__");
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

	public sealed class PassiveEffect : Trigger, IStuntProperty, IExtraProperty {
		#region Javascript API class
		private new class JSAPI : Trigger.JSAPI, IJSAPI<PassiveEffect> {
			private readonly PassiveEffect _outer;

			public JSAPI(PassiveEffect outer) :
				base(outer) {
				_outer = outer;
			}

			public IJSAPI<Stunt> getBelongStunt() {
				try {
					if (_outer.BelongStunt != null) return (IJSAPI<Stunt>)_outer.BelongStunt.GetContext();
					else return null;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Extra> getBelongExtra() {
				try {
					if (_outer.BelongExtra != null) return (IJSAPI<Extra>)_outer.BelongExtra.GetContext();
					else return null;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			PassiveEffect IJSAPI<PassiveEffect>.Origin(JSContextHelper proof) {
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

		private Stunt _belongStunt = null;
		private Extra _belongExtra = null;

		Stunt IAttachable<Stunt>.Belong => _belongStunt;
		Extra IAttachable<Extra>.Belong => _belongExtra;
		public Stunt BelongStunt => _belongStunt;
		public Extra BelongExtra => _belongExtra;

		public void SetBelong(Stunt belong) {
			_belongStunt = belong;
		}

		public void SetBelong(Extra belong) {
			_belongExtra = belong;
		}
		
		public PassiveEffect(string boundEventID, Command command) :
			base(boundEventID, command) {
			_apiObj = new JSAPI(this);
		}

		public override void Notify() {
			JSEngineManager.Engine.SynchronizeContext("$__belongStunt__", _belongStunt);
			JSEngineManager.Engine.SynchronizeContext("$__belongExtra__", _belongExtra);
			base.Notify();
			JSEngineManager.Engine.RemoveContext("$__belongStunt__");
			JSEngineManager.Engine.RemoveContext("$__belongExtra__");
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}
	}
}
