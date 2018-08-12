﻿using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.CharacterSystem
{
    public sealed class SkillChecker : IJSContextProvider
    {
        #region Javascript API class
        private sealed class JSAPI : IJSAPI<SkillChecker>
        {
            private readonly SkillChecker _outer;

            public JSAPI(SkillChecker outer)
            {
                _outer = outer;
            }

            public SkillChecker Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly JSAPI _apiObj;

        public enum CharacterAction
        {
            CREATE_ASPECT,
            ATTACK,
            HINDER
        }

        public enum CheckResult
        {
            CANCEL,
            FAIL,
            TIE,
            SUCCEED,
            SUCCEED_WITH_STYLE
        }

        public enum CheckerState
        {
            IDLE,
            INITIATIVE_SKILL,
            PASSIVE_SKILL,
            INITIATIVE_ASPECT,
            PASSIVE_ASPECT,
            END
        }

        private static Range FAIL;
        private static Range TIE;
        private static Range SUCCEED;
        private static Range SUCCEED_WITH_STYLE;

        private static readonly Dictionary<SkillType, List<SkillType>> OVERCOME = new Dictionary<SkillType, List<SkillType>>();
        private static readonly Dictionary<SkillType, List<SkillType>> EVADE = new Dictionary<SkillType, List<SkillType>>();
        private static readonly Dictionary<SkillType, List<SkillType>> DEFEND = new Dictionary<SkillType, List<SkillType>>();
        
        static SkillChecker()
        {
            FAIL = new Range(float.NegativeInfinity, 0);
            TIE = new Range(0, 0);
            TIE.highOpen = false;
            SUCCEED = new Range(0, 3);
            SUCCEED.lowOpen = true;
            SUCCEED_WITH_STYLE = new Range(3, float.PositiveInfinity);

            OVERCOME.Add(SkillType.Athletics, new List<SkillType>(new SkillType[] { SkillType.Athletics }));
            OVERCOME.Add(SkillType.Burglary, new List<SkillType>(new SkillType[] { SkillType.Burglary }));
            OVERCOME.Add(SkillType.Contacts, new List<SkillType>(new SkillType[] { SkillType.Contacts }));
            OVERCOME.Add(SkillType.Crafts, new List<SkillType>(new SkillType[] { SkillType.Crafts }));
            OVERCOME.Add(SkillType.Deceive, new List<SkillType>(new SkillType[] { SkillType.Deceive }));
            OVERCOME.Add(SkillType.Drive, new List<SkillType>(new SkillType[] { SkillType.Drive }));
            OVERCOME.Add(SkillType.Empathy, new List<SkillType>(new SkillType[] { SkillType.Empathy }));
            OVERCOME.Add(SkillType.Fight, new List<SkillType>(new SkillType[] { SkillType.Fight }));
            OVERCOME.Add(SkillType.Investigate, new List<SkillType>(new SkillType[] { SkillType.Investigate }));
            OVERCOME.Add(SkillType.Lore, new List<SkillType>(new SkillType[] { SkillType.Lore }));
            OVERCOME.Add(SkillType.Notice, new List<SkillType>(new SkillType[] { SkillType.Notice }));
            OVERCOME.Add(SkillType.Physique, new List<SkillType>(new SkillType[] { SkillType.Physique }));
            OVERCOME.Add(SkillType.Provoke, new List<SkillType>(new SkillType[] { SkillType.Provoke }));
            OVERCOME.Add(SkillType.Rapport, new List<SkillType>(new SkillType[] { SkillType.Rapport }));
            OVERCOME.Add(SkillType.Resources, new List<SkillType>(new SkillType[] { SkillType.Resources }));
            OVERCOME.Add(SkillType.Shoot, new List<SkillType>(new SkillType[] { SkillType.Shoot }));
            OVERCOME.Add(SkillType.Stealth, new List<SkillType>(new SkillType[] { SkillType.Stealth }));
            OVERCOME.Add(SkillType.Will, new List<SkillType>(new SkillType[] { SkillType.Will }));

            EVADE.Add(SkillType.Athletics, new List<SkillType>(new SkillType[] { SkillType.Athletics }));
            EVADE.Add(SkillType.Burglary, new List<SkillType>(new SkillType[] { SkillType.Burglary }));
            EVADE.Add(SkillType.Contacts, new List<SkillType>(new SkillType[] { SkillType.Contacts }));
            EVADE.Add(SkillType.Crafts, new List<SkillType>(new SkillType[] { SkillType.Crafts }));
            EVADE.Add(SkillType.Deceive, new List<SkillType>(new SkillType[] { SkillType.Deceive }));
            EVADE.Add(SkillType.Drive, new List<SkillType>(new SkillType[] { SkillType.Drive }));
            EVADE.Add(SkillType.Empathy, new List<SkillType>(new SkillType[] { SkillType.Empathy }));
            EVADE.Add(SkillType.Fight, new List<SkillType>(new SkillType[] { SkillType.Fight }));
            EVADE.Add(SkillType.Investigate, new List<SkillType>(new SkillType[] { SkillType.Investigate }));
            EVADE.Add(SkillType.Lore, new List<SkillType>(new SkillType[] { SkillType.Lore }));
            EVADE.Add(SkillType.Notice, new List<SkillType>(new SkillType[] { SkillType.Notice }));
            EVADE.Add(SkillType.Physique, new List<SkillType>(new SkillType[] { SkillType.Physique }));
            EVADE.Add(SkillType.Provoke, new List<SkillType>(new SkillType[] { SkillType.Provoke }));
            EVADE.Add(SkillType.Rapport, new List<SkillType>(new SkillType[] { SkillType.Rapport }));
            EVADE.Add(SkillType.Resources, new List<SkillType>(new SkillType[] { SkillType.Resources }));
            EVADE.Add(SkillType.Shoot, new List<SkillType>(new SkillType[] { SkillType.Shoot }));
            EVADE.Add(SkillType.Stealth, new List<SkillType>(new SkillType[] { SkillType.Stealth }));
            EVADE.Add(SkillType.Will, new List<SkillType>(new SkillType[] { SkillType.Will }));

            DEFEND.Add(SkillType.Athletics, new List<SkillType>(new SkillType[] { SkillType.Fight, SkillType.Shoot }));
            DEFEND.Add(SkillType.Physique, new List<SkillType>(new SkillType[] { SkillType.Fight, SkillType.Shoot }));
            DEFEND.Add(SkillType.Fight, new List<SkillType>(new SkillType[] { SkillType.Fight }));
            DEFEND.Add(SkillType.Will, new List<SkillType>(new SkillType[] { SkillType.Provoke }));
        }
        
        public static void SetShifts(Range fail, Range tie, Range succeed, Range succeedWithStyle)
        {
            FAIL = fail;
            TIE = tie;
            SUCCEED = succeed;
            SUCCEED_WITH_STYLE = succeedWithStyle;
        }

        private static readonly SkillChecker _instance = new SkillChecker();
        public static SkillChecker Instance => _instance;

        private Character _initiative;
        private Character _passive;
        private CharacterAction _action;
        private Action<CheckResult> _initiativeCallback;
        private Action<CheckResult> _passiveCallback;

        private SkillType _initiativeSkillType;
        private SkillType _passiveSkillType;
        private int _initiativeRollPoint;
        private int _passiveRollPoint;

        private int _initiativeExtraPoint;
        private int _passiveExtraPoint;

        private CheckerState _state = CheckerState.IDLE;
        
        public Character Initiative => _initiative;
        public Character Passive => _passive;
        public CharacterAction Action => _action;

        public SkillType InitiativeSkillType => _initiativeSkillType;
        public SkillType PassiveSkillType => _passiveSkillType;
        public int InitiativeRollPoint => _initiativeRollPoint;
        public int PassiveRollPoint => _passiveRollPoint;

        public int InitiativeExtraPoint { get => _initiativeExtraPoint; set => _initiativeExtraPoint = value; }
        public int PassiveExtraPoint { get => _passiveExtraPoint; set => _passiveExtraPoint = value; }

        public int InitiativePoint => (_initiativeSkillType != null ? _initiative.GetSkillProperty(_initiativeSkillType).level : 0) + _initiativeRollPoint + _initiativeExtraPoint;
        public int PassivePoint => (_initiativeSkillType != null ? _initiative.GetSkillProperty(_initiativeSkillType).level : 0) + _passiveRollPoint + _passiveExtraPoint;

        public CheckerState State => _state;

        private SkillChecker()
        {
            _apiObj = new JSAPI(this);
        }
        
        public static bool CanResistSkillWithoutDMCheck(SkillType initiativeUsing, SkillType resist, CharacterAction action)
        {
            Dictionary<SkillType, List<SkillType>> resistTable;
            switch (action)
            {
                case CharacterAction.CREATE_ASPECT:
                    resistTable = EVADE;
                    break;
                case CharacterAction.ATTACK:
                    resistTable = DEFEND;
                    break;
                case CharacterAction.HINDER:
                    resistTable = OVERCOME;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }
            if (resistTable.TryGetValue(resist, out List<SkillType> initiativeSkills)) return initiativeSkills.Contains(initiativeUsing);
            else return false;
        }
        
        public int[] RollDice()
        {
            return FateDice.Roll();
        }

        public void StartCheck(
            Character initiative, Character passive, CharacterAction action,
            Action<CheckResult> initiativeCallback, Action<CheckResult> passiveCallback
            )
        {
            if (_state != CheckerState.IDLE) throw new InvalidOperationException("Already in checking state.");
            _initiativeSkillType = null;
            _passiveSkillType = null;
            _initiativeRollPoint = 0;
            _passiveRollPoint = 0;
            _initiativeExtraPoint = 0;
            _passiveExtraPoint = 0;
            _initiative = initiative ?? throw new ArgumentNullException(nameof(initiative));
            _passive = passive ?? throw new ArgumentNullException(nameof(passive));
            _action = action;
            _initiativeCallback = initiativeCallback ?? throw new ArgumentNullException(nameof(initiativeCallback));
            _passiveCallback = passiveCallback ?? throw new ArgumentNullException(nameof(passiveCallback));
            _state = CheckerState.INITIATIVE_SKILL;
        }

        public void CancelCheck()
        {
            if (_state != CheckerState.INITIATIVE_SKILL) throw new InvalidOperationException("Cannot cancel at this time.");
            _initiativeCallback(CheckResult.CANCEL);
            _passiveCallback(CheckResult.CANCEL);
            _state = CheckerState.IDLE;
        }

        public void ForceEndCheck(CheckResult initiativeResult, CheckResult passiveResult)
        {
            if (_state == CheckerState.IDLE) throw new InvalidOperationException("Skill checking is not working.");
            _initiativeCallback(initiativeResult);
            _passiveCallback(passiveResult);
            _state = CheckerState.IDLE;
        }
        
        public void EndCheck()
        {
            if (_state != CheckerState.END) throw new InvalidOperationException("Cannot make checking over.");
            CheckResult initiativeResult;
            CheckResult passiveResult;
            int delta = this.InitiativePoint - this.PassivePoint;
            if (FAIL.InRange(delta))
            {
                initiativeResult = CheckResult.FAIL;
                if (SUCCEED_WITH_STYLE.InRange(-delta))
                {
                    passiveResult = CheckResult.SUCCEED_WITH_STYLE;
                }
                else
                {
                    passiveResult = CheckResult.SUCCEED;
                }
            }
            else if (TIE.InRange(delta))
            {
                initiativeResult = CheckResult.TIE;
                passiveResult = CheckResult.TIE;
            }
            else if (SUCCEED.InRange(delta))
            {
                initiativeResult = CheckResult.SUCCEED;
                passiveResult = CheckResult.FAIL;
            }
            else if (SUCCEED_WITH_STYLE.InRange(delta))
            {
                initiativeResult = CheckResult.SUCCEED_WITH_STYLE;
                passiveResult = CheckResult.FAIL;
            }
            else
            {
                initiativeResult = CheckResult.TIE;
                passiveResult = CheckResult.TIE;
            }
            _initiativeCallback(initiativeResult);
            _passiveCallback(passiveResult);
            _state = CheckerState.IDLE;
        }
        
        public void InitiativeSelectSkill(SkillType skillType)
        {
            if (_state != CheckerState.INITIATIVE_SKILL) throw new InvalidOperationException("State incorrect.");
            if (_action == CharacterAction.ATTACK)
            {
                SkillProperty skillProperty = _initiative.GetSkillProperty(skillType);
                if (!skillProperty.canAttack) throw new ArgumentException("This skill cannot use in attack situation.", nameof(skillType));
            }
            _initiativeSkillType = skillType;
        }
        
        public int[] InitiativeRollDice(int[] fixedDicePoints = null)
        {
            int[] dicePoints = fixedDicePoints ?? this.RollDice();
            _initiativeRollPoint = 0;
            foreach (int point in dicePoints) _initiativeRollPoint += point;
            return dicePoints;
        }

        public void InitiativeSkillSelectionOver()
        {
            if (_state != CheckerState.INITIATIVE_SKILL || _initiativeSkillType == null) throw new InvalidOperationException("State incorrect.");
            _state = CheckerState.PASSIVE_SKILL;
        }
        
        public void InitiativeAspectSelectionOver()
        {
            if (_state != CheckerState.INITIATIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
            _state = CheckerState.PASSIVE_ASPECT;
        }

        public void InitiativeSelectAspect(Aspect aspect, bool reroll)
        {
            if (_state != CheckerState.INITIATIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
            if (aspect.Benefit != _initiative && _initiative.FatePoint - 1 < 0) throw new InvalidOperationException("Fate points are not enough.");
            if (aspect.Benefit != null && aspect.Benefit != _initiative)
            {
                ++aspect.Benefit.FatePoint;
            }
            else if (aspect.Benefit == null)
            {
                ++_passive.FatePoint;
            }
            if (aspect.Benefit != _initiative) --_initiative.FatePoint;
            if (reroll) this.InitiativeRollDice();
            else _initiativeExtraPoint += 2;
        }
        
        public void PassiveSelectSkill(SkillType skillType)
        {
            if (_state != CheckerState.PASSIVE_SKILL) throw new InvalidOperationException("State incorrect.");
            if (_action == CharacterAction.ATTACK)
            {
                SkillProperty skillProperty = _passive.GetSkillProperty(skillType);
                if (!skillProperty.canDefend) throw new ArgumentException("This skill cannot use in attack situation.", nameof(skillType));
            }
            _passiveSkillType = skillType;
        }

        public int[] PassiveRollDice(int[] fixedDicePoints = null)
        {
            int[] dicePoints = fixedDicePoints ?? this.RollDice();
            _passiveRollPoint = 0;
            foreach (int point in dicePoints) _passiveRollPoint += point;
            return dicePoints;
        }

        public void PassiveSkillSelectionOver()
        {
            if (_state != CheckerState.PASSIVE_SKILL || _passiveSkillType == null) throw new InvalidOperationException("State incorrect.");
            _state = CheckerState.INITIATIVE_ASPECT;
        }
        
        public void PassiveAspectSelectionOver()
        {
            if (_state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
            _state = CheckerState.END;
        }

        public void PassiveSelectAspect(Aspect aspect, bool reroll)
        {
            if (_state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
            if (aspect.Benefit != _passive && _passive.FatePoint - 1 < 0) throw new InvalidOperationException("Fate points are not enough.");
            if (aspect.Benefit != null && aspect.Benefit != _passive)
            {
                ++aspect.Benefit.FatePoint;
            }
            else if (aspect.Benefit == null)
            {
                ++_initiative.FatePoint;
            }
            if (aspect.Benefit != _passive) --_passive.FatePoint;
            if (reroll) this.PassiveRollDice();
            else _passiveExtraPoint += 2;
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}