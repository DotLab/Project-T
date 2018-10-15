using GameServer.CharacterComponents;
using GameServer.Core;
using GameServer.Core.ScriptSystem;
using GameServer.EventSystem;
using GameServer.EventSystem.Events;
using GameUtil;
using System;
using System.Collections.Generic;

namespace GameServer.Playground {
	public sealed class SkillChecker : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<SkillChecker> {
			private readonly SkillChecker _outer;

			public JSAPI(SkillChecker outer) {
				_outer = outer;
			}
			/*
			public void currentPassiveUseSkillWithStuntComplete(IJSAPI<SkillType> skillType, Action<bool, string> completeFunc) {
				currentPassiveUseSkillWithStuntComplete(skillType, completeFunc, false, true, 0, null);
			}

			public void currentPassiveUseSkillWithStuntComplete(
				IJSAPI<SkillType> skillType, Action<bool, string> completeFunc,
				bool skipDMCheck, bool bigone, int extraPoint, int[] fixedDicePoints
				) {
				try {
					var origin_skillType = JSContextHelper.Instance.GetAPIOrigin(skillType);
					int[] dicePoints;
					if (!skipDMCheck) {
						if (!SkillType.CanResistSkillWithoutDMCheck(_outer.InitiativeSkillType, origin_skillType, _outer.CheckingAction)) {
							bool result = Game.DM.DMClient.RequireDMCheck(_outer.CurrentPassive.Controller,
							_outer.CurrentPassive.Name + "对" + _outer.Initiative.Name + "使用" + _outer.CurrentPassive.GetSkill(origin_skillType).Name + ",可以吗？");
							if (result) {
								_outer.CurrentPassiveUseSkill(origin_skillType);
								dicePoints = _outer.CurrentPassiveRollDice(fixedDicePoints);
								_outer.PassiveExtraPoint = extraPoint;
								foreach (Player player in Game.Players) {
									player.Client.BattleScene.DisplayDicePoints(_outer.CurrentPassive.CharacterRef.Controller, dicePoints);
									player.Client.BattleScene.DisplaySkillReady(_outer.CurrentPassive, origin_skillType, bigone);
									player.Client.BattleScene.UpdateSumPoint(_outer.CurrentPassive, _outer.GetPassivePoint());
								}
								Game.DM.Client.BattleScene.DisplayDicePoints(_outer.CurrentPassive.CharacterRef.Controller, dicePoints);
								Game.DM.Client.BattleScene.DisplaySkillReady(_outer.CurrentPassive, origin_skillType, bigone);
								Game.DM.Client.BattleScene.UpdateSumPoint(_outer.CurrentPassive, _outer.GetPassivePoint());
								_outer.PassiveSkillSelectionOver();
								completeFunc(true, "");
								_outer.Initiative.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectAspect();
							} else {
								completeFunc(false, "DM拒绝了你的选择");
							}
							return;
						}
					}
					_outer.PassiveSelectSkill(origin_skillType);
					dicePoints = _outer.PassiveRollDice(fixedDicePoints);
					_outer.PassiveExtraPoint = extraPoint;
					foreach (Player player in Game.Players) {
						player.Client.BattleScene.DisplayDicePoints(_outer.CurrentPassive.CharacterRef.Controller, dicePoints);
						player.Client.BattleScene.DisplaySkillReady(_outer.CurrentPassive, origin_skillType, bigone);
						player.Client.BattleScene.UpdateSumPoint(_outer.CurrentPassive, _outer.GetPassivePoint());
					}
					Game.DM.Client.BattleScene.DisplayDicePoints(_outer.CurrentPassive.CharacterRef.Controller, dicePoints);
					Game.DM.Client.BattleScene.DisplaySkillReady(_outer.CurrentPassive, origin_skillType, bigone);
					Game.DM.Client.BattleScene.UpdateSumPoint(_outer.CurrentPassive, _outer.GetPassivePoint());
					_outer.PassiveSkillSelectionOver();
					completeFunc(true, "");
					_outer.Initiative.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectAspect();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}
			*/
			public SkillChecker Origin(JSContextHelper proof) {
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

		private enum CheckerState {
			IDLE,
			READY,
			PASSIVE_RESIST,
			INITIATIVE_ASPECT,
			PASSIVE_ASPECT
		}

		private static Range FAIL;
		private static Range TIE;
		private static Range SUCCEED;
		private static Range SUCCEED_WITH_STYLE;

		static SkillChecker() {
			FAIL = new Range(float.NegativeInfinity, 0);
			TIE = new Range(0, 0);
			TIE.highOpen = false;
			SUCCEED = new Range(0, 3);
			SUCCEED.lowOpen = true;
			SUCCEED_WITH_STYLE = new Range(3, float.PositiveInfinity);
		}

		public static void SetShifts(Range fail, Range tie, Range succeed, Range succeedWithStyle) {
			FAIL = fail;
			TIE = tie;
			SUCCEED = succeed;
			SUCCEED_WITH_STYLE = succeedWithStyle;
		}

		private static readonly SkillChecker _instance = new SkillChecker();
		public static SkillChecker Instance => _instance;

		private Character _initiative;
		private List<Character> _passives;
		private Character _currentPassive;
		private CharacterAction _checkingAction;
		private Action _allCheckOverCallback;
		private Action _onceCheckStartCallback;
		private Action<CheckResult, CheckResult, int> _onceCheckOverCallback;

		private SkillType _initiativeSkillType;
		private int[] _initiativeRollPoints;
		private int _initiativeExtraPoint;
		
		private SkillType _currentInitiativeSkillType;
		private SkillType _currentPassiveSkillType;
		private int[] _currentInitiativeRollPoints;
		private int[] _currentPassiveRollPoints;
		private int _currentInitiativeExtraPoint;
		private int _currentPassiveExtraPoint;

		private CheckerState _state = CheckerState.IDLE;
		
		private readonly List<Aspect> _usedAspects = new List<Aspect>();

		public Character Initiative => _initiative;
		public List<Character> Passives => _passives;
		public Character CurrentPassive => _currentPassive;
		public CharacterAction CheckingAction => _checkingAction;

		public SkillType InitiativeSkillType => _currentInitiativeSkillType;
		public SkillType PassiveSkillType => _currentPassiveSkillType;
		public int[] InitiativeRollPoints => _currentInitiativeRollPoints;
		public int[] PassiveRollPoints => _currentPassiveRollPoints;

		public int InitiativeExtraPoint { get => _currentInitiativeExtraPoint; set => _currentInitiativeExtraPoint = value; }
		public int PassiveExtraPoint { get => _currentPassiveExtraPoint; set => _currentPassiveExtraPoint = value; }

		public bool IsChecking => _state != CheckerState.IDLE;

		private SkillChecker() {
			_passives = new List<Character>();
			_apiObj = new JSAPI(this);
		}

		public int GetInitiativePoint() {
			var rollPoint = 0;
			foreach (int point in _currentInitiativeRollPoints) rollPoint += point;
			return (_currentInitiativeSkillType != null ? _initiative.GetSkill(_currentInitiativeSkillType).Level : 0) + rollPoint + _currentInitiativeExtraPoint;
		}

		public int GetPassivePoint() {
			var rollPoint = 0;
			foreach (int point in _currentPassiveRollPoints) rollPoint += point;
			return (_currentPassiveSkillType != null ? _currentPassive.GetSkill(_currentPassiveSkillType).Level : 0) + rollPoint + _currentPassiveExtraPoint;
		}

		public bool CanInitiativeUseSkill(Character initiative, SkillType skillType, CharacterAction action) {
			if (initiative == null) throw new ArgumentNullException(nameof(initiative));
			if (skillType == null) throw new ArgumentNullException(nameof(skillType));
			return SkillType.CanInitiativeUseSkill(initiative, skillType, action);
		}

		public void InitiativeUseSkill(
			Action<bool> dmCheckCallback,
			Character initiative, IEnumerable<Character> passives,
			CharacterAction action, SkillType skillType,
			Action allCheckOver, Action onceCheckStart, Action<CheckResult, CheckResult, int> onceCheckOver,
			int extraPoint = 0, int[] fixedDicePoints = null
			) {
			if (dmCheckCallback == null) throw new ArgumentNullException(nameof(dmCheckCallback));
			if (action == CharacterAction.CREATE_ASPECT) {
				bool result = Game.DM.DMClient.RequireDMCheck(initiative.Controller,
					initiative.Name + "想使用" + initiative.GetSkill(skillType).Name + ",可以吗？");
				dmCheckCallback(result);
				if (result) {
					initiative.Controller.Client.SkillChecker.NotifyInitiativeActionAccepted();
					InitiativeUseSkillWithoutDMCheck(initiative, passives, action, skillType, () => { }, () => { }, (iResult, pResult, delta) => { });
				} else {
					initiative.Controller.Client.SkillChecker.NotifyInitiativeActionFailure("DM拒绝了你选择的技能");
				}
			} else {
				dmCheckCallback(true);
				initiative.Controller.Client.SkillChecker.NotifyInitiativeActionAccepted();
				InitiativeUseSkillWithoutDMCheck(initiative, passives, action, skillType, () => { }, () => { }, (iResult, pResult, delta) => { });
			}
		}

		public void InitiativeUseSkillWithoutDMCheck(
			Character initiative, IEnumerable<Character> passives,
			CharacterAction action, SkillType skillType,
			Action allCheckOver, Action onceCheckStart, Action<CheckResult, CheckResult, int> onceCheckOver,
			int extraPoint = 0, int[] fixedDicePoints = null
			) {
			if (_state != CheckerState.IDLE) throw new InvalidOperationException("Already in checking state.");
			if (passives == null) throw new ArgumentNullException(nameof(passives));
			bool hasElement = false;
			foreach (var obj in passives) {
				hasElement = true;
				break;
			}
			if (!hasElement) throw new ArgumentException("There is no passive character for checking.", nameof(passives));
			_initiative = initiative ?? throw new ArgumentNullException(nameof(initiative));
			_initiativeSkillType = skillType ?? throw new ArgumentNullException(nameof(skillType));
			if (!CanInitiativeUseSkill(_initiative, _initiativeSkillType, _checkingAction)) throw new InvalidOperationException("This skill cannot use in attack situation.");
			var skill = _initiative.GetSkill(skillType);
			if (skill.TargetMaxCount != -1) {
				int targetCount = 0;
				foreach (var passive in passives) ++targetCount;
				if (targetCount > skill.TargetMaxCount) throw new InvalidOperationException("Targets count is more than limit!");
			}
			_initiativeRollPoints = fixedDicePoints ?? FateDice.Roll();
			_initiativeExtraPoint = extraPoint;
			_allCheckOverCallback = allCheckOver ?? throw new ArgumentNullException(nameof(allCheckOver));
			_onceCheckStartCallback = onceCheckStart ?? throw new ArgumentNullException(nameof(onceCheckStart));
			_onceCheckOverCallback = onceCheckOver ?? throw new ArgumentNullException(nameof(onceCheckOver));
			_checkingAction = action;
			_passives.Clear();
			_passives.AddRange(passives);
			_passives.Reverse();
			_currentPassive = null;
			foreach (Player player in Game.Players) {
				player.Client.SkillChecker.StartCheck(initiative, _initiativeSkillType, action, passives);
				player.Client.DisplayDicePoints(initiative.Controller, _initiativeRollPoints);
			}
			Game.DM.Client.SkillChecker.StartCheck(initiative, _initiativeSkillType, action, passives);
			Game.DM.Client.DisplayDicePoints(initiative.Controller, _initiativeRollPoints);
			_state = CheckerState.READY;
			NextPassiveCheck();
		}

		public bool CanInitiativeUseStunt(Character initiative, Stunt stunt, Situation usingConditionSituation) {
			if (initiative == null) throw new ArgumentNullException(nameof(initiative));
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if (usingConditionSituation == null) throw new ArgumentNullException(nameof(usingConditionSituation));
			if (stunt.Belong != initiative) return false;
			if (usingConditionSituation.Action == 0 && !stunt.SituationLimit.canUseDirectly) return false;
			else if (usingConditionSituation.Action != 0 && (stunt.SituationLimit.usableSituation & usingConditionSituation.Action) == 0) return false;
			if (stunt.UsingCondition == null) return true;
			stunt.UsingCondition.Situation = usingConditionSituation;
			return stunt.UsingCondition.Judge();
		}

		public bool CanInitiativeUseStuntOnCharacter(Character target, Stunt stunt, Situation targetConditionSituation) {
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if (stunt.TargetCondition == null) return true;
			stunt.TargetCondition.Situation = targetConditionSituation;
			return stunt.TargetCondition.Judge();
		}

		public void InitiativeUseStunt(
			Character initiative, Stunt stunt,
			Situation situation, Action<bool> resultCallback
			) {
			if (initiative == null) throw new ArgumentNullException(nameof(initiative));
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if (situation == null) throw new ArgumentNullException(nameof(situation));
			if (stunt.Belong != initiative) throw new ArgumentException("This stunt is not belong to Initiative Character.", nameof(stunt));
			if (!CanInitiativeUseStunt(initiative, stunt, situation)) throw new InvalidOperationException("Cannot use this stunt.");
			if (stunt.TargetMaxCount != -1) {
				if (situation.IsInStoryScene) {
					if (situation.PassivesSS.Length > stunt.TargetMaxCount) throw new InvalidOperationException("Targets count is more than limit!");
					foreach (var target in situation.PassivesSS) {
						if (!CanInitiativeUseStuntOnCharacter(target.CharacterRef, stunt, situation)) throw new InvalidOperationException("Cannot use this stunt on the target.");
					}
				} else {
					if (situation.PassivesBS.Length > stunt.TargetMaxCount) throw new InvalidOperationException("Targets count is more than limit!");
					foreach (var target in situation.PassivesBS) {
						if (!CanInitiativeUseStuntOnCharacter(target.CharacterRef, stunt, situation)) throw new InvalidOperationException("Cannot use this stunt on the target.");
					}
				}
			}
			stunt.Effect.Situation = situation;
			stunt.Effect.TakeEffect((success, message) => {
				if (success) {
					foreach (Player player in Game.Players) {
						player.Client.SkillChecker.DisplayUsingStunt(initiative, stunt);
					}
					Game.DM.Client.SkillChecker.DisplayUsingStunt(initiative, stunt);
					initiative.Controller.Client.SkillChecker.NotifyInitiativeActionAccepted();
				} else {
					initiative.Controller.Client.SkillChecker.NotifyInitiativeActionFailure(message);
				}
				resultCallback(success);
			});
		}
		
		public void NextPassiveCheck() {
			if (_state != CheckerState.READY) throw new InvalidOperationException("It's not in the correct checking state.");
			if (_passives.Count <= 0 || _initiative.Destroyed) {
				foreach (Player player in Game.Players) {
					player.Client.SkillChecker.EndCheck();
				}
				Game.DM.Client.SkillChecker.EndCheck();
				_state = CheckerState.IDLE;
				_allCheckOverCallback();
				return;
			}
			_currentInitiativeSkillType = _initiativeSkillType;
			_currentInitiativeRollPoints = (int[])_initiativeRollPoints.Clone();
			_currentInitiativeExtraPoint = _initiativeExtraPoint;
			_currentPassive = _passives[_passives.Count - 1];
			_passives.RemoveAt(_passives.Count - 1);
			_currentPassiveSkillType = null;
			_currentPassiveRollPoints = null;
			_currentPassiveExtraPoint = 0;
			_usedAspects.Clear();
			_onceCheckStartCallback();
			_state = CheckerState.PASSIVE_RESIST;
			int sumPoint = GetInitiativePoint();
			foreach (Player player in Game.Players) {
				player.Client.SkillChecker.Position = Client.SkillCheckerProxy.ClientPosition.OBSERVER;
				player.Client.SkillChecker.DisplayUsingSkill(true, _initiativeSkillType);
				player.Client.SkillChecker.UpdateSumPoint(true, sumPoint);
				player.Client.SkillChecker.CheckNextone(_currentPassive);
			}
			Game.DM.Client.SkillChecker.Position = Client.SkillCheckerProxy.ClientPosition.OBSERVER;
			Game.DM.Client.SkillChecker.DisplayUsingSkill(true, _initiativeSkillType);
			Game.DM.Client.SkillChecker.UpdateSumPoint(true, sumPoint);
			Game.DM.Client.SkillChecker.CheckNextone(_currentPassive);
			_initiative.Controller.Client.SkillChecker.Position = Client.SkillCheckerProxy.ClientPosition.INITIATIVE;
			_currentPassive.Controller.Client.SkillChecker.Position = Client.SkillCheckerProxy.ClientPosition.PASSIVE;
			_currentPassive.Controller.Client.SkillChecker.NotifyPassiveSelectAction();
		}

		public void ForceEndCheck(CheckResult initiativeResult, CheckResult passiveResult, int delta) {
			if (_state == CheckerState.IDLE) throw new InvalidOperationException("Skill checking is not working.");
			_state = CheckerState.READY;
			ApplyCheckResult(initiativeResult, passiveResult, delta);
			foreach (Player player in Game.Players) {
				player.Client.SkillChecker.EndCheck();
			}
			Game.DM.Client.SkillChecker.EndCheck();
			_state = CheckerState.IDLE;
			_allCheckOverCallback();
		}

		public void EndOnceCheck() {
			if (_state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("Cannot make checking over.");
			CheckResult initiativeResult;
			CheckResult passiveResult;
			int delta = this.GetInitiativePoint() - this.GetPassivePoint();
			if (FAIL.InRange(delta)) {
				initiativeResult = CheckResult.FAIL;
				if (SUCCEED_WITH_STYLE.InRange(-delta)) {
					passiveResult = CheckResult.SUCCEED_WITH_STYLE;
				} else {
					passiveResult = CheckResult.SUCCEED;
				}
			} else if (TIE.InRange(delta)) {
				initiativeResult = CheckResult.TIE;
				passiveResult = CheckResult.TIE;
			} else if (SUCCEED.InRange(delta)) {
				initiativeResult = CheckResult.SUCCEED;
				passiveResult = CheckResult.FAIL;
			} else if (SUCCEED_WITH_STYLE.InRange(delta)) {
				initiativeResult = CheckResult.SUCCEED_WITH_STYLE;
				passiveResult = CheckResult.FAIL;
			} else {
				initiativeResult = CheckResult.TIE;
				passiveResult = CheckResult.TIE;
			}
			_state = CheckerState.READY;
			ApplyCheckResult(initiativeResult, passiveResult, delta);
			NextPassiveCheck();
		}

		private void ApplyCheckResult(CheckResult initiativeResult, CheckResult passiveResult, int delta) {
			foreach (Player player in Game.Players) {
				player.Client.SkillChecker.DisplayCheckResult(initiativeResult, passiveResult, delta);
			}
			Game.DM.Client.SkillChecker.DisplayCheckResult(initiativeResult, passiveResult, delta);

			foreach (var aspect in _usedAspects) {
				if (aspect.BenefitTimes <= 0 && aspect.PersistenceType == PersistenceType.Temporary) {
					aspect.Benefiter = null;
					var owner = aspect.Belong;
					var consequence = aspect as Consequence;
					if (consequence != null) {
						owner.Consequences.Remove(consequence);
					} else {
						owner.Aspects.Remove(aspect);
					}
				}
			}

			var initiativeSkill = _initiative.GetSkill(_initiativeSkillType);
			var aspectCreatedEvent = new OnceCheckOverAspectCreatedEvent();
			var aspectCreatedEventInfo = new OnceCheckOverAspectCreatedEvent.EventInfo() {
				from = (IJSAPI<Character>)_initiative.GetContext(),
				to = (IJSAPI<Character>)_currentPassive.GetContext(),
				action = _checkingAction,
				from_checkResult = initiativeResult,
				to_checkResult = passiveResult,
				pointDelta = delta
			};
			var causeDamageEvent = new OnceCheckOverCauseDamageEvent();
			var causeDamageEventInfo = new OnceCheckOverCauseDamageEvent.EventInfo() {
				from = (IJSAPI<Character>)_initiative.GetContext(),
				to = (IJSAPI<Character>)_currentPassive.GetContext(),
				action = _checkingAction,
				from_checkResult = initiativeResult,
				to_checkResult = passiveResult,
				pointDelta = delta
			};
			if (_checkingAction == CharacterAction.CREATE_ASPECT) {
				switch (initiativeResult) {
					case CheckResult.TIE: {
							var boost = new Aspect();
							boost.PersistenceType = PersistenceType.Temporary;
							boost.Name = "受到" + _initiative.Name + "的" + initiativeSkill.Name + "影响";
							boost.Benefiter = _initiative;
							boost.BenefitTimes = 1;
							_currentPassive.Aspects.Add(boost);
							aspectCreatedEventInfo.createdAspect = (IJSAPI<Aspect>)boost.GetContext();
							aspectCreatedEvent.Info = aspectCreatedEventInfo;
							GameEventBus.Instance.Publish(aspectCreatedEvent);
						}
						break;
					case CheckResult.SUCCEED: {
							var aspect = new Aspect();
							aspect.PersistenceType = PersistenceType.Common;
							aspect.Name = "受到" + _initiative.Name + "的" + initiativeSkill.Name + "影响";
							aspect.Benefiter = _initiative;
							aspect.BenefitTimes = 1;
							_currentPassive.Aspects.Add(aspect);
							aspectCreatedEventInfo.createdAspect = (IJSAPI<Aspect>)aspect.GetContext();
							aspectCreatedEvent.Info = aspectCreatedEventInfo;
							GameEventBus.Instance.Publish(aspectCreatedEvent);
						}
						break;
					case CheckResult.SUCCEED_WITH_STYLE: {
							var aspect = new Aspect();
							aspect.PersistenceType = PersistenceType.Common;
							aspect.Name = "受到" + _initiative.Name + "的" + initiativeSkill.Name + "影响";
							aspect.Benefiter = _initiative;
							aspect.BenefitTimes = 2;
							_currentPassive.Aspects.Add(aspect);
							aspectCreatedEventInfo.createdAspect = (IJSAPI<Aspect>)aspect.GetContext();
							aspectCreatedEvent.Info = aspectCreatedEventInfo;
							GameEventBus.Instance.Publish(aspectCreatedEvent);
						}
						break;
					default:
						break;
				}
			} else if (_checkingAction == CharacterAction.ATTACK) {
				switch (initiativeResult) {
					case CheckResult.TIE: {
							var boost = new Aspect();
							boost.PersistenceType = PersistenceType.Temporary;
							boost.Name = "受到" + _initiative.Name + "的" + initiativeSkill.Name + "攻击";
							boost.Benefiter = _initiative;
							boost.BenefitTimes = 1;
							_currentPassive.Aspects.Add(boost);
							aspectCreatedEventInfo.createdAspect = (IJSAPI<Aspect>)boost.GetContext();
							aspectCreatedEvent.Info = aspectCreatedEventInfo;
							GameEventBus.Instance.Publish(aspectCreatedEvent);
						}
						break;
					case CheckResult.SUCCEED: {
							_currentPassive.Damage(delta, initiativeSkill.DamageMental, _initiative, "使用" + initiativeSkill.Name + "发动攻击");
							causeDamageEventInfo.damage = delta;
							causeDamageEventInfo.mental = initiativeSkill.DamageMental;
							causeDamageEvent.Info = causeDamageEventInfo;
							GameEventBus.Instance.Publish(causeDamageEvent);
						}
						break;
					case CheckResult.SUCCEED_WITH_STYLE: {
							int determin = _initiative.Controller.Client.RequireDetermin("降低一点伤害来换取一个增益吗？", new string[] { "是", "否" });
							int damage = delta;
							if (determin == 1) {
								var boost = new Aspect();
								boost.PersistenceType = PersistenceType.Temporary;
								boost.Name = "受到" + _initiative.Name + "的" + initiativeSkill.Name + "攻击";
								boost.Benefiter = _initiative;
								boost.BenefitTimes = 1;
								_currentPassive.Aspects.Add(boost);
								damage -= 1;
								aspectCreatedEventInfo.createdAspect = (IJSAPI<Aspect>)boost.GetContext();
								aspectCreatedEvent.Info = aspectCreatedEventInfo;
								GameEventBus.Instance.Publish(aspectCreatedEvent);
							}
							_currentPassive.Damage(damage, initiativeSkill.DamageMental, _initiative, "使用" + initiativeSkill.Name + "发动攻击");
							causeDamageEventInfo.damage = damage;
							causeDamageEventInfo.mental = initiativeSkill.DamageMental;
							causeDamageEvent.Info = causeDamageEventInfo;
							GameEventBus.Instance.Publish(causeDamageEvent);
						}
						break;
					default:
						break;
				}
			}

			_onceCheckOverCallback(initiativeResult, passiveResult, delta);

			var checkOverEvent = new OnceCheckOverEvent();
			checkOverEvent.Info = new OnceCheckOverEvent.EventInfo() {
				initiative = (IJSAPI<Character>)_initiative.GetContext(),
				initiativeSkillType = (IJSAPI<SkillType>)this.InitiativeSkillType.GetContext(),
				initiativeRollPoints = this.InitiativeRollPoints,
				initiativeExtraPoint = this.InitiativeExtraPoint,
				passive = (IJSAPI<Character>)_currentPassive.GetContext(),
				passiveSkillType = (IJSAPI<SkillType>)this.PassiveSkillType.GetContext(),
				passiveRollPoints = this.PassiveRollPoints,
				passiveExtraPoint = this.PassiveExtraPoint,
				action = this.CheckingAction,
				initiativeCheckResult = initiativeResult,
				passiveCheckResult = passiveResult,
				pointDelta = delta
			};
			GameEventBus.Instance.Publish(checkOverEvent);
		}

		public bool CanCurrentPassiveUseSkill(SkillType skillType) {
			if (_state != CheckerState.PASSIVE_RESIST) throw new InvalidOperationException("State incorrect.");
			if (skillType == null) throw new ArgumentNullException(nameof(skillType));
			return SkillType.CanPassiveUseSkill(_currentPassive, skillType, _checkingAction);
		}

		public bool CanCurrentPassiveResistSkillWithoutDMCheck(SkillType initiativeUsing, SkillType resist, CharacterAction action) {
			if (_state != CheckerState.PASSIVE_RESIST) throw new InvalidOperationException("State incorrect.");
			if (initiativeUsing == null) throw new ArgumentNullException(nameof(initiativeUsing));
			if (resist == null) throw new ArgumentNullException(nameof(resist));
			return SkillType.CanResistSkillWithoutDMCheck(initiativeUsing, resist, action);
		}

		public void CurrentPassiveUseSkill(SkillType skillType) {
			if (_state != CheckerState.PASSIVE_RESIST) throw new InvalidOperationException("State incorrect.");
			if (skillType == null) throw new ArgumentNullException(nameof(skillType));
			if (!CanCurrentPassiveUseSkill(skillType)) throw new InvalidOperationException("Cannot use this skill.");
			if (CanCurrentPassiveResistSkillWithoutDMCheck(_currentInitiativeSkillType, skillType, _checkingAction)) {
				_currentPassiveSkillType = skillType;
				_currentPassiveRollPoints = FateDice.Roll();
				foreach (Player player in Game.Players) {
					player.Client.DisplayDicePoints(_currentPassive.Controller, _currentPassiveRollPoints);
					player.Client.SkillChecker.DisplayUsingSkill(false, skillType);
					player.Client.SkillChecker.UpdateSumPoint(false, GetPassivePoint());
				}
				Game.DM.Client.DisplayDicePoints(_currentPassive.Controller, _currentPassiveRollPoints);
				Game.DM.Client.SkillChecker.DisplayUsingSkill(false, skillType);
				Game.DM.Client.SkillChecker.UpdateSumPoint(false, GetPassivePoint());
				_state = CheckerState.INITIATIVE_ASPECT;
				_currentPassive.Controller.Client.SkillChecker.NotifyPassiveActionAccepted();
				_initiative.Controller.Client.SkillChecker.NotifyInitiativeSelectAspect();
			} else {
				bool result = Game.DM.DMClient.RequireDMCheck(_currentPassive.Controller,
					_currentPassive.Name + "对" + _initiative.Name + "使用" + _currentPassive.GetSkill(skillType).Name + ",可以吗？");
				if (result) {
					_currentPassiveSkillType = skillType;
					_currentPassiveRollPoints = FateDice.Roll();
					foreach (Player player in Game.Players) {
						player.Client.DisplayDicePoints(_currentPassive.Controller, _currentPassiveRollPoints);
						player.Client.SkillChecker.DisplayUsingSkill(false, skillType);
						player.Client.SkillChecker.UpdateSumPoint(false, GetPassivePoint());
					}
					Game.DM.Client.DisplayDicePoints(_currentPassive.Controller, _currentPassiveRollPoints);
					Game.DM.Client.SkillChecker.DisplayUsingSkill(false, skillType);
					Game.DM.Client.SkillChecker.UpdateSumPoint(false, GetPassivePoint());
					_state = CheckerState.INITIATIVE_ASPECT;
					_currentPassive.Controller.Client.SkillChecker.NotifyPassiveActionAccepted();
					_initiative.Controller.Client.SkillChecker.NotifyInitiativeSelectAspect();
				} else {
					_currentPassive.Controller.Client.SkillChecker.NotifyPassiveActionFailure("DM拒绝了你选择的技能");
				}
			}
		}

		public bool CanCurrentPassiveUseStunt(Stunt stunt, Situation situation) {
			if (_state != CheckerState.PASSIVE_RESIST) throw new InvalidOperationException("State incorrect.");
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if (situation == null) throw new ArgumentNullException(nameof(situation));
			if ((stunt.SituationLimit.resistableSituation & _checkingAction) != 0) {
				bool canUse = true;
				if (stunt.UsingCondition != null) {
					stunt.UsingCondition.Situation = situation;
					canUse &= stunt.UsingCondition.Judge();
				}
				if (stunt.TargetCondition != null && canUse) {
					stunt.TargetCondition.Situation = situation;
					canUse &= stunt.TargetCondition.Judge();
				}
				return canUse;
			} else return false;
		}

		public void CurrentPassiveUseStunt(Stunt stunt, Situation situation, Action<bool> resultCallback) {
			if (_state != CheckerState.PASSIVE_RESIST) throw new InvalidOperationException("State incorrect.");
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if (situation == null) throw new ArgumentNullException(nameof(situation));
			if (stunt.Belong != _currentPassive) throw new ArgumentException("This stunt is not belong to passive character.", nameof(stunt));
			if (!CanCurrentPassiveUseStunt(stunt, situation)) throw new InvalidOperationException("Cannot use this stunt.");
			stunt.Effect.Situation = situation;
			stunt.Effect.TakeEffect((success, message) => {
				if (success) {
					foreach (Player player in Game.Players) {
						player.Client.SkillChecker.DisplayUsingStunt(_currentPassive, stunt);
					}
					Game.DM.Client.SkillChecker.DisplayUsingStunt(_currentPassive, stunt);
					_currentPassive.Controller.Client.SkillChecker.NotifyPassiveActionAccepted();
				} else {
					_currentPassive.Controller.Client.SkillChecker.NotifyPassiveActionFailure(message);
				}
				resultCallback(success);
			});
		}

		public bool CanInitiativeUseAspect(Aspect aspect) {
			if ((aspect.Benefiter != _initiative || (aspect.Benefiter != null && !aspect.Benefiter.IsPartyWith(_initiative))) && _initiative.FatePoint - 1 < 0)
				return false;
			if (aspect.Benefiter != null && aspect.BenefitTimes > 0
				&& aspect.Benefiter != _initiative && !aspect.Benefiter.IsPartyWith(_initiative)
				&& aspect.PersistenceType == PersistenceType.Temporary)
				return false;
			return true;
		}
		
		public void InitiativeUseAspect(Aspect aspect, bool reroll) {
			if (aspect == null) throw new ArgumentNullException(nameof(aspect));
			if (!CanInitiativeUseAspect(aspect)) {
				_initiative.Controller.Client.SkillChecker.NotifyInitiativeSelectAspectFailure("你不能使用这个特征");
				return;
			}
			var owner = CharacterManager.Instance.FindCharacterByID(aspect.Belong.ID);
			foreach (Player player in Game.Players) {
				player.Client.SkillChecker.DisplayUsingAspect(true, owner, aspect);
			}
			Game.DM.Client.SkillChecker.DisplayUsingAspect(true, owner, aspect);
			bool result = Game.DM.DMClient.RequireDMCheck(_initiative.Controller,
				_initiative.Name + "想使用" + aspect.Belong.Name + "的特征“" + aspect.Name + "”可以吗？");
			if (result) {
				if (_state != CheckerState.INITIATIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
				if ((aspect.Benefiter != _initiative || (aspect.Benefiter != null && !aspect.Benefiter.IsPartyWith(_initiative))) && _initiative.FatePoint - 1 < 0)
					throw new InvalidOperationException("Fate points are not enough.");
				if (aspect.Benefiter != null && aspect.BenefitTimes > 0) {
					if (aspect.Benefiter == _initiative || aspect.Benefiter.IsPartyWith(_initiative)) {
						--aspect.BenefitTimes;
					} else {
						if (aspect.PersistenceType == PersistenceType.Temporary) throw new InvalidOperationException("This boost is not for you.");
						++aspect.Benefiter.FatePoint;
						--_initiative.FatePoint;
					}
				} else {
					++_currentPassive.FatePoint;
					--_initiative.FatePoint;
				}
				_usedAspects.Add(aspect);
				if (reroll) _currentInitiativeRollPoints = FateDice.Roll();
				else {
					_currentInitiativeExtraPoint += 2;
				}
				foreach (Player player in Game.Players) {
					if (reroll) player.Client.DisplayDicePoints(_initiative.Controller, _currentInitiativeRollPoints);
					player.Client.SkillChecker.UpdateSumPoint(true, GetInitiativePoint());
				}
				if (reroll) Game.DM.Client.DisplayDicePoints(_initiative.Controller, _currentInitiativeRollPoints);
				Game.DM.Client.SkillChecker.UpdateSumPoint(true, GetInitiativePoint());
				_initiative.Controller.Client.SkillChecker.NotifyInitiativeSelectAspectComplete();
			} else {
				_initiative.Controller.Client.SkillChecker.NotifyInitiativeSelectAspectFailure("DM拒绝了你选择的特征");
			}
		}

		public void InitiativeAspectSelectionOver() {
			if (_state != CheckerState.INITIATIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
			_initiative.Controller.Client.SkillChecker.NotifyInitiativeSelectAspectOver();
			_state = CheckerState.PASSIVE_ASPECT;
			_currentPassive.Controller.Client.SkillChecker.NotifyPassiveSelectAspect();
		}

		public bool CanCurrentPassiveUseAspect(Aspect aspect) {
			if ((aspect.Benefiter != _currentPassive || (aspect.Benefiter != null && !aspect.Benefiter.IsPartyWith(_currentPassive))) && _currentPassive.FatePoint - 1 < 0)
				return false;
			if (aspect.Benefiter != null && aspect.BenefitTimes > 0
				&& aspect.Benefiter != _currentPassive && !aspect.Benefiter.IsPartyWith(_currentPassive)
				&& aspect.PersistenceType == PersistenceType.Temporary)
				return false;
			return true;
		}

		public void CurrentPassiveUseAspect(Aspect aspect, bool reroll) {
			if (aspect == null) throw new ArgumentNullException(nameof(aspect));
			if (!CanCurrentPassiveUseAspect(aspect)) {
				_currentPassive.Controller.Client.SkillChecker.NotifyPassiveSelectAspectFailure("你不能使用这个特征");
				return;
			}
			var owner = CharacterManager.Instance.FindCharacterByID(aspect.Belong.ID);
			foreach (Player player in Game.Players) {
				player.Client.SkillChecker.DisplayUsingAspect(false, owner, aspect);
			}
			Game.DM.Client.SkillChecker.DisplayUsingAspect(false, owner, aspect);
			bool result = Game.DM.DMClient.RequireDMCheck(_currentPassive.Controller,
				_currentPassive.Name + "想使用" + aspect.Belong.Name + "的特征“" + aspect.Name + "”可以吗？");
			if (result) {
				if (_state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
				if ((aspect.Benefiter != _currentPassive || (aspect.Benefiter != null && !aspect.Benefiter.IsPartyWith(_currentPassive))) && _currentPassive.FatePoint - 1 < 0)
					throw new InvalidOperationException("Fate points are not enough.");
				if (aspect.Benefiter != null && aspect.BenefitTimes > 0) {
					if (aspect.Benefiter == _currentPassive || aspect.Benefiter.IsPartyWith(_currentPassive)) {
						--aspect.BenefitTimes;
					} else {
						if (aspect.PersistenceType == PersistenceType.Temporary) throw new InvalidOperationException("This boost is not for you.");
						++aspect.Benefiter.FatePoint;
						--_currentPassive.FatePoint;
					}
				} else {
					++_initiative.FatePoint;
					--_currentPassive.FatePoint;
				}
				_usedAspects.Add(aspect);
				if (reroll) _currentPassiveRollPoints = FateDice.Roll();
				else {
					_currentPassiveExtraPoint += 2;
				}
				foreach (Player player in Game.Players) {
					if (reroll) player.Client.DisplayDicePoints(_currentPassive.Controller, _currentPassiveRollPoints);
					player.Client.SkillChecker.UpdateSumPoint(false, GetPassivePoint());
				}
				if (reroll) Game.DM.Client.DisplayDicePoints(_currentPassive.Controller, _currentPassiveRollPoints);
				Game.DM.Client.SkillChecker.UpdateSumPoint(false, GetPassivePoint());
				_currentPassive.Controller.Client.SkillChecker.NotifyPassiveSelectAspectComplete();
			} else {
				_currentPassive.Controller.Client.SkillChecker.NotifyPassiveSelectAspectFailure("DM拒绝了你选择的特征");
			}
		}

		public void CurrentPassiveAspectSelectionOver() {
			if (_state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
			_currentPassive.Controller.Client.SkillChecker.NotifyPassiveSelectAspectOver();
			EndOnceCheck();
		}
		
		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}
}
