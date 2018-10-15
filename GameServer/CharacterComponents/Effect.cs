using GameServer.EventSystem;
using GameServer.Core;
using GameServer.Core.ScriptSystem;
using System;
using GameUtil;
using System.Collections.Generic;

namespace GameServer.CharacterComponents {
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

			public IJSAPI<Playground.StoryComponent.SceneObject> getInitiativeSS() {
				try {
					if (_outer.InitiativeSS == null) return null;
					return (IJSAPI<Playground.StoryComponent.SceneObject>)_outer.InitiativeSS.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Playground.BattleComponent.SceneObject> getInitiativeBS() {
				try {
					if (_outer.InitiativeBS == null) return null;
					return (IJSAPI<Playground.BattleComponent.SceneObject>)_outer.InitiativeBS.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Playground.StoryComponent.SceneObject>[] getTargetsSS() {
				try {
					if (_outer.PassivesSS == null) return null;
					var ret = new IJSAPI<Playground.StoryComponent.SceneObject>[_outer.PassivesSS.Length];
					for (int i = 0; i < _outer.PassivesSS.Length; ++i) {
						ret[i] = (IJSAPI<Playground.StoryComponent.SceneObject>)_outer.PassivesSS[i].GetContext();
					}
					return ret;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Playground.BattleComponent.SceneObject>[] getTargetsBS() {
				try {
					if (_outer.PassivesBS == null) return null;
					var ret = new IJSAPI<Playground.BattleComponent.SceneObject>[_outer.PassivesBS.Length];
					for (int i = 0; i < _outer.PassivesBS.Length; ++i) {
						ret[i] = (IJSAPI<Playground.BattleComponent.SceneObject>)_outer.PassivesBS[i].GetContext();
					}
					return ret;
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
			
			public IJSAPI<SkillType> getInitiativeSkillType() {
				try {
					if (_outer.InitiativeSkillType == null) return null;
					return (IJSAPI<SkillType>)_outer.InitiativeSkillType.GetContext();
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
		private Playground.StoryComponent.SceneObject _initiativeSS;
		private Playground.BattleComponent.SceneObject _initiativeBS;
		private Playground.StoryComponent.SceneObject[] _passivesSS;
		private Playground.BattleComponent.SceneObject[] _passivesBS;
		private CharacterAction _action;
		private SkillType _initiativeSkillType;

		public bool IsTriggerInvoking { get => _isTriggerInvoking; set => _isTriggerInvoking = value; }
		public string EventID { get => _eventID; set => _eventID = value; }
		public bool IsInStoryScene { get => _isInStoryScene; set => _isInStoryScene = value; }
		public bool IsInitiative { get => _isInitiative; set => _isInitiative = value; }
		public Playground.StoryComponent.SceneObject InitiativeSS { get => _initiativeSS; set => _initiativeSS = value; }
		public Playground.BattleComponent.SceneObject InitiativeBS { get => _initiativeBS; set => _initiativeBS = value; }
		public Playground.StoryComponent.SceneObject[] PassivesSS { get => _passivesSS; set => _passivesSS = value; }
		public Playground.BattleComponent.SceneObject[] PassivesBS { get => _passivesBS; set => _passivesBS = value; }
		public CharacterAction Action { get => _action; set => _action = value; }
		public SkillType InitiativeSkillType { get => _initiativeSkillType; set => _initiativeSkillType = value; }

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

	public sealed class StuntEffect : IStuntProperty {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<StuntEffect> {
			private readonly StuntEffect _outer;

			public JSAPI(StuntEffect outer) {
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

			public Action<bool, string> getCompleteFunc() {
				try {
					return _outer._resultCallback;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public StuntEffect Origin(JSContextHelper proof) {
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

		public Stunt Belong => _belong;
		public Situation Situation { get => _situation; set => _situation = value; }
		
		public void SetBelong(Stunt belong) {
			_belong = belong;
		}
		
		public StuntEffect(Command command) {
			_command = command ?? throw new ArgumentNullException(nameof(command));
			_apiObj = new JSAPI(this);
		}

		public void TakeEffect(Action<bool, string> resultCallback) {
			_resultCallback = resultCallback;
			JSEngineManager.Engine.SynchronizeContext("$__this__", this);
			_command.DoAction();
			JSEngineManager.Engine.RemoveContext("$__this__");
		}
		
		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

	public sealed class ExtraEffect : Trigger, IExtraProperty {
		#region Javascript API class
		private new class JSAPI : Trigger.JSAPI, IJSAPI<ExtraEffect> {
			private readonly ExtraEffect _outer;

			public JSAPI(ExtraEffect outer) :
				base(outer) {
				_outer = outer;
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

			ExtraEffect IJSAPI<ExtraEffect>.Origin(JSContextHelper proof) {
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
		
		private Extra _belong = null;
		
		public Extra Belong => _belong;
		
		public void SetBelong(Extra belong) {
			_belong = belong;
		}
		
		public ExtraEffect(string boundEventID, Command command) :
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
