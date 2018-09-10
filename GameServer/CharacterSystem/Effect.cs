using GameServer.EventSystem;
using GameServer.Core;
using GameServer.Core.ScriptSystem;
using System;
using GameUtil;
using System.Collections.Generic;

namespace GameServer.CharacterSystem {
	public sealed class Situation : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<Situation> {
			private readonly Situation _outer;

			public JSAPI(Situation outer) {
				_outer = outer;
			}

			public bool isTriggerInvoking() {
				try {
					return _outer.IsTriggerInvoking;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public string eventID() {
				try {
					return _outer.EventID;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public bool isInStoryScene() {
				try {
					return _outer.IsInStoryScene;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return true;
				}
			}

			public bool isInitiative() {
				try {
					return _outer.IsInitiative;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return true;
				}
			}

			public IJSAPI<Container.StoryComponent.SceneObject> getInitiativeSS() {
				try {
					if (_outer.InitiativeSS == null) return null;
					return (IJSAPI<Container.StoryComponent.SceneObject>)_outer.InitiativeSS.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Container.BattleComponent.SceneObject> getInitiativeBS() {
				try {
					if (_outer.InitiativeBS == null) return null;
					return (IJSAPI<Container.BattleComponent.SceneObject>)_outer.InitiativeBS.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Container.StoryComponent.SceneObject> getPassiveSS() {
				try {
					if (_outer.PassiveSS == null) return null;
					return (IJSAPI<Container.StoryComponent.SceneObject>)_outer.PassiveSS.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Container.BattleComponent.SceneObject> getPassiveBS() {
				try {
					if (_outer.PassiveBS == null) return null;
					return (IJSAPI<Container.BattleComponent.SceneObject>)_outer.PassiveBS.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public int getAction() {
				try {
					return (int)_outer.Action;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return 0;
				}
			}

			public bool isOnInteract() {
				try {
					return _outer.IsOnInteract;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public IJSAPI<SkillType> getInitiativeSkillType() {
				try {
					if (_outer.InitiativeSkillType == null) return null;
					return (IJSAPI<SkillType>)_outer.InitiativeSkillType.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}
			
			public IJSAPI<Container.BattleComponent.SceneObject>[] getTargetsBS() {
				try {
					if (_outer.TargetsBS == null) return null;
					var ret = new IJSAPI<Container.BattleComponent.SceneObject>[_outer.TargetsBS.Length];
					for (int i = 0; i < _outer.TargetsBS.Length; ++i) {
						ret[i] = (IJSAPI<Container.BattleComponent.SceneObject>)_outer.TargetsBS[i].GetContext();
					}
					return ret;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
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

		private bool _isTriggerInvoking;
		private string _eventID;
		private bool _isInStoryScene;
		private bool _isInitiative;
		private Container.StoryComponent.SceneObject _initiativeSS;
		private Container.BattleComponent.SceneObject _initiativeBS;
		private Container.StoryComponent.SceneObject _passiveSS;
		private Container.BattleComponent.SceneObject _passiveBS;
		private CharacterAction _action;
		private bool _isOnInteract;
		private SkillType _initiativeSkillType;
		private Container.BattleComponent.SceneObject[] _targetsBS;

		public bool IsTriggerInvoking { get => _isTriggerInvoking; set => _isTriggerInvoking = value; }
		public string EventID { get => _eventID; set => _eventID = value; }
		public bool IsInStoryScene { get => _isInStoryScene; set => _isInStoryScene = value; }
		public bool IsInitiative { get => _isInitiative; set => _isInitiative = value; }
		public Container.StoryComponent.SceneObject InitiativeSS { get => _initiativeSS; set => _initiativeSS = value; }
		public Container.BattleComponent.SceneObject InitiativeBS { get => _initiativeBS; set => _initiativeBS = value; }
		public Container.StoryComponent.SceneObject PassiveSS { get => _passiveSS; set => _passiveSS = value; }
		public Container.BattleComponent.SceneObject PassiveBS { get => _passiveBS; set => _passiveBS = value; }
		public CharacterAction Action { get => _action; set => _action = value; }
		public bool IsOnInteract { get => _isOnInteract; set => _isOnInteract = value; }
		public SkillType InitiativeSkillType { get => _initiativeSkillType; set => _initiativeSkillType = value; }
		public Container.BattleComponent.SceneObject[] TargetsBS { get => _targetsBS; set => _targetsBS = value; }

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
					if (_outer.Belong == null) return null;
					return (IJSAPI<Stunt>)_outer.Belong.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Situation> getSituation() {
				try {
					return (IJSAPI<Situation>)_outer.Situation.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public bool getResult() {
				try {
					return _outer.Result;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return true;
				}
			}

			public void setResult(bool val) {
				try {
					_outer.Result = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
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

	public sealed class InitiativeEffect : IStuntProperty {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<InitiativeEffect> {
			private readonly InitiativeEffect _outer;

			public JSAPI(InitiativeEffect outer) {
				_outer = outer;
			}

			public IJSAPI<Stunt> getBelongStunt() {
				try {
					if (_outer.Belong == null) return null;
					return (IJSAPI<Stunt>)_outer.Belong.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Situation> getSituation() {
				try {
					return (IJSAPI<Situation>)_outer.Situation.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void notifyResult(bool success, string message) {
				try {
					_outer.NotifyResult(success, message);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
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
		private readonly Command _command;
		private Situation _situation = null;
		private Action<bool, string> _resultCallback = null;
		private bool _performing = false;

		public Stunt Belong => _belong;
		public Situation Situation { get => _situation; set => _situation = value; }
		
		public void SetBelong(Stunt belong) {
			_belong = belong;
		}
		
		public InitiativeEffect(Command command) {
			_command = command ?? throw new ArgumentNullException(nameof(command));
			_apiObj = new JSAPI(this);
		}

		public void TakeEffect(Action<bool, string> resultCallback) {
			if (!_performing) {
				_resultCallback = resultCallback;
				_performing = true;
				JSEngineManager.Engine.SynchronizeContext("$__this__", this);
				_command.DoAction();
				JSEngineManager.Engine.RemoveContext("$__this__");
			}
		}

		private void NotifyResult(bool success, string message) {
			if (_performing && _resultCallback != null) {
				_resultCallback(success, message);
				_resultCallback = null;
				_performing = false;
			} else throw new InvalidOperationException("Stunt effect is not performing");
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
			if (_belongStunt != null) {
				if (_belongStunt.UsingCondition != null) {
					_belongStunt.UsingCondition.Situation = new Situation() {
						IsTriggerInvoking = true, EventID = _boundEventID,
						IsInStoryScene = false, IsInitiative = false,
						InitiativeSS = null, InitiativeBS = null,
						PassiveSS = null, PassiveBS = null,
						Action = 0, IsOnInteract = false,
						InitiativeSkillType = null, TargetsBS = null
					};
					if (!_belongStunt.UsingCondition.Judge()) return;
				}
			}
			JSEngineManager.Engine.SynchronizeContext("$__this__", this);
			base.Notify();
			JSEngineManager.Engine.RemoveContext("$__this__");
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}
	}
}
