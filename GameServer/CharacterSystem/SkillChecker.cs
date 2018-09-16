using GameServer.Core;
using GameUtil;
using System;
using System.Collections.Generic;

namespace GameServer.CharacterSystem {
	public sealed class SkillChecker {
		public enum CheckerState {
			IDLE,
			INITIATIVE_SKILL,
			PASSIVE_SKILL,
			INITIATIVE_ASPECT,
			PASSIVE_ASPECT,
			WAIT_FOR_ENDCHECK
		}

		private static Range FAIL;
		private static Range TIE;
		private static Range SUCCEED;
		private static Range SUCCEED_WITH_STYLE;

		private static readonly Dictionary<SkillType, List<SkillType>> _OVERCOME = new Dictionary<SkillType, List<SkillType>>();
		private static readonly Dictionary<SkillType, List<SkillType>> _EVADE = new Dictionary<SkillType, List<SkillType>>();
		private static readonly Dictionary<SkillType, List<SkillType>> _DEFEND = new Dictionary<SkillType, List<SkillType>>();

		public static Dictionary<SkillType, List<SkillType>> OVERCOME => _OVERCOME;
		public static Dictionary<SkillType, List<SkillType>> EVADE => _EVADE;
		public static Dictionary<SkillType, List<SkillType>> DEFEND => _DEFEND;

		static SkillChecker() {
			FAIL = new Range(float.NegativeInfinity, 0);
			TIE = new Range(0, 0);
			TIE.highOpen = false;
			SUCCEED = new Range(0, 3);
			SUCCEED.lowOpen = true;
			SUCCEED_WITH_STYLE = new Range(3, float.PositiveInfinity);

			_OVERCOME.Add(SkillType.Athletics, new List<SkillType>(new SkillType[] { SkillType.Athletics, SkillType.Physique }));
			_OVERCOME.Add(SkillType.Burglary, new List<SkillType>(new SkillType[] { SkillType.Burglary }));
			_OVERCOME.Add(SkillType.Contacts, new List<SkillType>(new SkillType[] { SkillType.Contacts, SkillType.Deceive, SkillType.Investigate }));
			_OVERCOME.Add(SkillType.Crafts, new List<SkillType>(new SkillType[] { SkillType.Crafts }));
			_OVERCOME.Add(SkillType.Deceive, new List<SkillType>(new SkillType[] { SkillType.Deceive, SkillType.Investigate, SkillType.Lore }));
			_OVERCOME.Add(SkillType.Drive, new List<SkillType>(new SkillType[] { SkillType.Drive }));
			_OVERCOME.Add(SkillType.Empathy, new List<SkillType>(new SkillType[] { SkillType.Deceive }));
			_OVERCOME.Add(SkillType.Fight, new List<SkillType>(new SkillType[] { SkillType.Fight }));
			_OVERCOME.Add(SkillType.Investigate, new List<SkillType>(new SkillType[] { SkillType.Deceive }));
			_OVERCOME.Add(SkillType.Lore, new List<SkillType>(new SkillType[] { SkillType.Deceive }));
			_OVERCOME.Add(SkillType.Notice, new List<SkillType>(new SkillType[] { SkillType.Stealth }));
			_OVERCOME.Add(SkillType.Physique, new List<SkillType>(new SkillType[] { SkillType.Physique, SkillType.Fight, SkillType.Shoot }));
			_OVERCOME.Add(SkillType.Provoke, new List<SkillType>(new SkillType[] { SkillType.Provoke }));
			_OVERCOME.Add(SkillType.Rapport, new List<SkillType>(new SkillType[] { SkillType.Rapport }));
			_OVERCOME.Add(SkillType.Resources, new List<SkillType>(new SkillType[] { SkillType.Resources }));
			_OVERCOME.Add(SkillType.Shoot, new List<SkillType>(new SkillType[] { }));
			_OVERCOME.Add(SkillType.Stealth, new List<SkillType>(new SkillType[] { SkillType.Notice, SkillType.Investigate }));
			_OVERCOME.Add(SkillType.Will, new List<SkillType>(new SkillType[] { SkillType.Provoke }));

			_EVADE.Add(SkillType.Athletics, new List<SkillType>(new SkillType[] { SkillType.Athletics, SkillType.Fight, SkillType.Shoot, SkillType.Physique }));
			_EVADE.Add(SkillType.Burglary, new List<SkillType>(new SkillType[] { SkillType.Burglary }));
			_EVADE.Add(SkillType.Contacts, new List<SkillType>(new SkillType[] { SkillType.Contacts, SkillType.Deceive }));
			_EVADE.Add(SkillType.Crafts, new List<SkillType>(new SkillType[] { SkillType.Crafts }));
			_EVADE.Add(SkillType.Deceive, new List<SkillType>(new SkillType[] { SkillType.Deceive, SkillType.Investigate, SkillType.Lore }));
			_EVADE.Add(SkillType.Drive, new List<SkillType>(new SkillType[] { SkillType.Drive }));
			_EVADE.Add(SkillType.Empathy, new List<SkillType>(new SkillType[] { SkillType.Deceive }));
			_EVADE.Add(SkillType.Fight, new List<SkillType>(new SkillType[] { SkillType.Fight }));
			_EVADE.Add(SkillType.Investigate, new List<SkillType>(new SkillType[] { SkillType.Deceive }));
			_EVADE.Add(SkillType.Lore, new List<SkillType>(new SkillType[] { SkillType.Deceive }));
			_EVADE.Add(SkillType.Notice, new List<SkillType>(new SkillType[] { SkillType.Stealth }));
			_EVADE.Add(SkillType.Physique, new List<SkillType>(new SkillType[] { SkillType.Physique, SkillType.Fight, SkillType.Shoot }));
			_EVADE.Add(SkillType.Provoke, new List<SkillType>(new SkillType[] { SkillType.Provoke }));
			_EVADE.Add(SkillType.Rapport, new List<SkillType>(new SkillType[] { SkillType.Rapport }));
			_EVADE.Add(SkillType.Resources, new List<SkillType>(new SkillType[] { SkillType.Resources }));
			_EVADE.Add(SkillType.Shoot, new List<SkillType>(new SkillType[] { }));
			_EVADE.Add(SkillType.Stealth, new List<SkillType>(new SkillType[] { SkillType.Notice, SkillType.Investigate }));
			_EVADE.Add(SkillType.Will, new List<SkillType>(new SkillType[] { SkillType.Provoke }));

			_DEFEND.Add(SkillType.Athletics, new List<SkillType>(new SkillType[] { SkillType.Fight, SkillType.Shoot }));
			_DEFEND.Add(SkillType.Physique, new List<SkillType>(new SkillType[] { SkillType.Fight, SkillType.Shoot }));
			_DEFEND.Add(SkillType.Fight, new List<SkillType>(new SkillType[] { SkillType.Fight }));
			_DEFEND.Add(SkillType.Will, new List<SkillType>(new SkillType[] { SkillType.Provoke }));
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
		private Character _passive;
		private CharacterAction _checkingAction;
		private Action<CheckResult, CheckResult, int> _checkOverCallback;

		private SkillType _initiativeSkillType;
		private SkillType _passiveSkillType;
		private int[] _initiativeRollPoints;
		private int[] _passiveRollPoints;

		private int _initiativeExtraPoint;
		private int _passiveExtraPoint;

		private CheckerState _state = CheckerState.IDLE;

		private readonly List<Aspect> _usedAspects = new List<Aspect>();

		public Character Initiative => _initiative;
		public Character Passive => _passive;
		public CharacterAction CheckingAction => _checkingAction;

		public SkillType InitiativeSkillType => _initiativeSkillType;
		public SkillType PassiveSkillType => _passiveSkillType;
		public int[] InitiativeRollPoints => _initiativeRollPoints;
		public int[] PassiveRollPoints => _passiveRollPoints;

		public int InitiativeExtraPoint { get => _initiativeExtraPoint; set => _initiativeExtraPoint = value; }
		public int PassiveExtraPoint { get => _passiveExtraPoint; set => _passiveExtraPoint = value; }

		public CheckerState State => _state;

		private SkillChecker() { }

		public static bool CanInitiativeUseSkillInAction(Character initiative, SkillType skillType, CharacterAction action) {
			var situationLimit = initiative.GetSkill(skillType).SituationLimit;
			if ((situationLimit.usableSituation & action) == 0) return false;
			else return true;
		}

		public static bool CanPassiveUseSkillInAction(Character passive, SkillType skillType, CharacterAction action) {
			var situationLimit = passive.GetSkill(skillType).SituationLimit;
			if ((situationLimit.resistableSituation & action) == 0) return false;
			else return true;
		}

		public static bool CanResistSkillWithoutDMCheck(SkillType initiativeUsing, SkillType resist, CharacterAction action) {
			Dictionary<SkillType, List<SkillType>> resistTable;
			switch (action) {
				case CharacterAction.CREATE_ASPECT:
					resistTable = _EVADE;
					break;
				case CharacterAction.ATTACK:
					resistTable = _DEFEND;
					break;
				case CharacterAction.HINDER:
					resistTable = _OVERCOME;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(action));
			}
			if (resistTable.TryGetValue(resist, out List<SkillType> initiativeSkills)) return initiativeSkills.Contains(initiativeUsing);
			else return false;
		}

		public int GetInitiativePoint() {
			var rollPoint = 0;
			foreach (int point in _initiativeRollPoints) rollPoint += point;
			return (_initiativeSkillType != null ? _initiative.GetSkill(_initiativeSkillType).Level : 0) + rollPoint + _initiativeExtraPoint;
		}

		public int GetPassivePoint() {
			var rollPoint = 0;
			foreach (int point in _passiveRollPoints) rollPoint += point;
			return (_passiveSkillType != null ? _passive.GetSkill(_passiveSkillType).Level : 0) + rollPoint + _passiveExtraPoint;
		}

		public void StartCheck(
			Character initiative, Character passive, CharacterAction action,
			Action<CheckResult, CheckResult, int> checkOverCallback
			) {
			if (_state != CheckerState.IDLE) throw new InvalidOperationException("Already in checking state.");
			_initiative = initiative ?? throw new ArgumentNullException(nameof(initiative));
			_passive = passive ?? throw new ArgumentNullException(nameof(passive));
			_checkOverCallback = checkOverCallback ?? throw new ArgumentNullException(nameof(checkOverCallback));
			_initiativeSkillType = null;
			_passiveSkillType = null;
			_initiativeRollPoints = null;
			_passiveRollPoints = null;
			_initiativeExtraPoint = 0;
			_passiveExtraPoint = 0;
			_checkingAction = action;
			_usedAspects.Clear();
			_state = CheckerState.INITIATIVE_SKILL;
		}

		public void ForceEndCheck(CheckResult initiativeResult, CheckResult passiveResult, int delta) {
			if (_state == CheckerState.IDLE) throw new InvalidOperationException("Skill checking is not working.");
			_state = CheckerState.IDLE;
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
			_checkOverCallback(initiativeResult, passiveResult, delta);
		}

		public void EndCheck() {
			if (_state != CheckerState.WAIT_FOR_ENDCHECK) throw new InvalidOperationException("Cannot make checking over.");
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
			_state = CheckerState.IDLE;
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
			_checkOverCallback(initiativeResult, passiveResult, delta);
		}

		public void InitiativeSelectSkill(SkillType skillType) {
			if (_state != CheckerState.INITIATIVE_SKILL) throw new InvalidOperationException("State incorrect.");
			if (!CanInitiativeUseSkillInAction(_initiative, skillType, _checkingAction)) throw new InvalidOperationException("This skill cannot use in attack situation.");
			_initiativeSkillType = skillType;
		}

		public int[] InitiativeRollDice(int[] fixedDicePoints = null) {
			_initiativeRollPoints = fixedDicePoints ?? FateDice.Roll();
			return _initiativeRollPoints;
		}

		public void InitiativeSkillSelectionOver() {
			if (_state != CheckerState.INITIATIVE_SKILL || _initiativeSkillType == null) throw new InvalidOperationException("State incorrect.");
			_state = CheckerState.PASSIVE_SKILL;
		}

		public void InitiativeAspectSelectionOver() {
			if (_state != CheckerState.INITIATIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
			_state = CheckerState.PASSIVE_ASPECT;
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

		public int[] InitiativeUseAspect(Aspect aspect, bool reroll) {
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
				++_passive.FatePoint;
				--_initiative.FatePoint;
			}
			_usedAspects.Add(aspect);
			if (reroll) return this.InitiativeRollDice();
			else {
				_initiativeExtraPoint += 2;
				return null;
			}
		}

		public void PassiveSelectSkill(SkillType skillType) {
			if (_state != CheckerState.PASSIVE_SKILL) throw new InvalidOperationException("State incorrect.");
			if (!CanPassiveUseSkillInAction(_passive, skillType, _checkingAction)) throw new InvalidOperationException("This skill cannot use in attack situation.");
			_passiveSkillType = skillType;
		}

		public int[] PassiveRollDice(int[] fixedDicePoints = null) {
			_passiveRollPoints = fixedDicePoints ?? FateDice.Roll();
			return _passiveRollPoints;
		}

		public void PassiveSkillSelectionOver() {
			if (_state != CheckerState.PASSIVE_SKILL || _passiveSkillType == null) throw new InvalidOperationException("State incorrect.");
			_state = CheckerState.INITIATIVE_ASPECT;
		}

		public void PassiveAspectSelectionOver() {
			if (_state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
			_state = CheckerState.WAIT_FOR_ENDCHECK;
		}

		public bool CanPassiveUseAspect(Aspect aspect) {
			if ((aspect.Benefiter != _passive || (aspect.Benefiter != null && !aspect.Benefiter.IsPartyWith(_passive))) && _passive.FatePoint - 1 < 0)
				return false;
			if (aspect.Benefiter != null && aspect.BenefitTimes > 0
				&& aspect.Benefiter != _passive && !aspect.Benefiter.IsPartyWith(_passive)
				&& aspect.PersistenceType == PersistenceType.Temporary)
				return false;
			return true;
		}

		public int[] PassiveUseAspect(Aspect aspect, bool reroll) {
			if (_state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
			if ((aspect.Benefiter != _passive || (aspect.Benefiter != null && !aspect.Benefiter.IsPartyWith(_passive))) && _passive.FatePoint - 1 < 0)
				throw new InvalidOperationException("Fate points are not enough.");
			if (aspect.Benefiter != null && aspect.BenefitTimes > 0) {
				if (aspect.Benefiter == _passive || aspect.Benefiter.IsPartyWith(_passive)) {
					--aspect.BenefitTimes;
				} else {
					if (aspect.PersistenceType == PersistenceType.Temporary) throw new InvalidOperationException("This boost is not for you.");
					++aspect.Benefiter.FatePoint;
					--_passive.FatePoint;
				}
			} else {
				++_initiative.FatePoint;
				--_passive.FatePoint;
			}
			_usedAspects.Add(aspect);
			if (reroll) return this.PassiveRollDice();
			else {
				_passiveExtraPoint += 2;
				return null;
			}
		}
	}
}
