using GameLib.EventSystem;
using GameLib.Core;
using GameLib.Core.ScriptSystem;
using System;
using GameLib.Utilities;
using System.Collections.Generic;

namespace GameLib.CharacterSystem {
	public sealed class Situation : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<Situation> {
			private readonly Situation _outer;

			public JSAPI(Situation outer) {
				_outer = outer;
			}
			
			public Situation Origin(JSContextHelper proof) {
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
		
		private bool _isInStoryScene;
		private bool _isInitiative;
		private Container.StoryComponent.SceneObject _initiativeSS;
		private Container.BattleComponent.SceneObject _initiativeBS;
		private Container.StoryComponent.SceneObject _passiveSS;
		private Container.BattleComponent.SceneObject _passiveBS;
		private CharacterAction _action;
		private SkillType _initiativeSkillType;
		private List<Container.BattleComponent.SceneObject> _targetsBS;

		public bool IsInStoryScene { get => _isInStoryScene; set => _isInStoryScene = value; }
		public bool IsInitiative { get => _isInitiative; set => _isInitiative = value; }
		public Container.StoryComponent.SceneObject InitiativeSS { get => _initiativeSS; set => _initiativeSS = value; }
		public Container.BattleComponent.SceneObject InitiativeBS { get => _initiativeBS; set => _initiativeBS = value; }
		public Container.StoryComponent.SceneObject PassiveSS { get => _passiveSS; set => _passiveSS = value; }
		public Container.BattleComponent.SceneObject PassiveBS { get => _passiveBS; set => _passiveBS = value; }
		public CharacterAction Action { get => _action; set => _action = value; }
		public SkillType InitiativeSkillType { get => _initiativeSkillType; set => _initiativeSkillType = value; }
		public List<Container.BattleComponent.SceneObject> TargetsBS { get => _targetsBS; set => _targetsBS = value; }

		public Situation() {
			_apiObj = new JSAPI(this);
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

	public sealed class Condition : IStuntProperty {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<Condition> {
			private readonly Condition _outer;

			public JSAPI(Condition outer) {
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

			public Condition Origin(JSContextHelper proof) {
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
		private readonly Command _statement;
		private Situation _situation = null;
		private bool _result = true;

		public Stunt Belong => _belong;
		public Situation Situation { get => _situation; set => _situation = value; }
		public bool Result { get => _result; set => _result = value; }
		
		public void SetBelong(Stunt belong) {
			_belong = belong;
		}
		
		public Condition(Command statement) {
			_statement = statement ?? throw new ArgumentNullException(nameof(statement));
			_apiObj = new JSAPI(this);
		}
		
		public bool Judge() {
			JSEngineManager.Engine.SynchronizeContext("$__this__", this);
			_statement.DoAction();
			JSEngineManager.Engine.RemoveContext("$__this__");
			return _result;
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

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
		private Situation _situation = null;

		public Stunt Belong => _belong;
		public Situation Situation { get => _situation; set => _situation = value; }

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
			JSEngineManager.Engine.SynchronizeContext("$__this__", this);
			base.DoAction();
			JSEngineManager.Engine.RemoveContext("$__this__");
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
			JSEngineManager.Engine.SynchronizeContext("$__this__", this);
			base.Notify();
			JSEngineManager.Engine.RemoveContext("$__this__");
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}
	}
}
