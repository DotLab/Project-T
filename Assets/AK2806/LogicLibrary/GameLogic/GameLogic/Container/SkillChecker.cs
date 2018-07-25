using GameLogic.CharacterSystem;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Container
{
    public sealed class SkillChecker : IJSContextProvider
    {
        private sealed class API : IJSAPI<SkillChecker>
        {
            private readonly SkillChecker _outer;

            public API(SkillChecker outer)
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

        private readonly API _apiObj;

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
            PASSIVE_ASPECT
        }

        private static Range FAIL;
        private static Range TIE;
        private static Range SUCCEED;
        private static Range SUCCEED_WITH_STYLE;

        public static readonly Dictionary<SkillType, List<SkillType>> Overcome = new Dictionary<SkillType, List<SkillType>>();
        public static readonly Dictionary<SkillType, List<SkillType>> Evade = new Dictionary<SkillType, List<SkillType>>();
        public static readonly Dictionary<SkillType, List<SkillType>> Defend = new Dictionary<SkillType, List<SkillType>>();
        
        static SkillChecker()
        {
            FAIL = new Range(float.NegativeInfinity, 0);
            TIE = new Range(0, 0);
            TIE.highOpen = false;
            SUCCEED = new Range(0, 3);
            SUCCEED.lowOpen = true;
            SUCCEED_WITH_STYLE = new Range(3, float.PositiveInfinity);

            Overcome.Add(SkillType.Athletics, new List<SkillType>(new SkillType[] { SkillType.Athletics }));
            Overcome.Add(SkillType.Burglary, new List<SkillType>(new SkillType[] { SkillType.Burglary }));
            Overcome.Add(SkillType.Contacts, new List<SkillType>(new SkillType[] { SkillType.Contacts }));
            Overcome.Add(SkillType.Crafts, new List<SkillType>(new SkillType[] { SkillType.Crafts }));
            Overcome.Add(SkillType.Deceive, new List<SkillType>(new SkillType[] { SkillType.Deceive }));
            Overcome.Add(SkillType.Drive, new List<SkillType>(new SkillType[] { SkillType.Drive }));
            Overcome.Add(SkillType.Empathy, new List<SkillType>(new SkillType[] { SkillType.Empathy }));
            Overcome.Add(SkillType.Fight, new List<SkillType>(new SkillType[] { SkillType.Fight }));
            Overcome.Add(SkillType.Investigate, new List<SkillType>(new SkillType[] { SkillType.Investigate }));
            Overcome.Add(SkillType.Lore, new List<SkillType>(new SkillType[] { SkillType.Lore }));
            Overcome.Add(SkillType.Notice, new List<SkillType>(new SkillType[] { SkillType.Notice }));
            Overcome.Add(SkillType.Physique, new List<SkillType>(new SkillType[] { SkillType.Physique }));
            Overcome.Add(SkillType.Provoke, new List<SkillType>(new SkillType[] { SkillType.Provoke }));
            Overcome.Add(SkillType.Rapport, new List<SkillType>(new SkillType[] { SkillType.Rapport }));
            Overcome.Add(SkillType.Resources, new List<SkillType>(new SkillType[] { SkillType.Resources }));
            Overcome.Add(SkillType.Shoot, new List<SkillType>(new SkillType[] { SkillType.Shoot }));
            Overcome.Add(SkillType.Stealth, new List<SkillType>(new SkillType[] { SkillType.Stealth }));
            Overcome.Add(SkillType.Will, new List<SkillType>(new SkillType[] { SkillType.Will }));

            Evade.Add(SkillType.Athletics, new List<SkillType>(new SkillType[] { SkillType.Athletics }));
            Evade.Add(SkillType.Burglary, new List<SkillType>(new SkillType[] { SkillType.Burglary }));
            Evade.Add(SkillType.Contacts, new List<SkillType>(new SkillType[] { SkillType.Contacts }));
            Evade.Add(SkillType.Crafts, new List<SkillType>(new SkillType[] { SkillType.Crafts }));
            Evade.Add(SkillType.Deceive, new List<SkillType>(new SkillType[] { SkillType.Deceive }));
            Evade.Add(SkillType.Drive, new List<SkillType>(new SkillType[] { SkillType.Drive }));
            Evade.Add(SkillType.Empathy, new List<SkillType>(new SkillType[] { SkillType.Empathy }));
            Evade.Add(SkillType.Fight, new List<SkillType>(new SkillType[] { SkillType.Fight }));
            Evade.Add(SkillType.Investigate, new List<SkillType>(new SkillType[] { SkillType.Investigate }));
            Evade.Add(SkillType.Lore, new List<SkillType>(new SkillType[] { SkillType.Lore }));
            Evade.Add(SkillType.Notice, new List<SkillType>(new SkillType[] { SkillType.Notice }));
            Evade.Add(SkillType.Physique, new List<SkillType>(new SkillType[] { SkillType.Physique }));
            Evade.Add(SkillType.Provoke, new List<SkillType>(new SkillType[] { SkillType.Provoke }));
            Evade.Add(SkillType.Rapport, new List<SkillType>(new SkillType[] { SkillType.Rapport }));
            Evade.Add(SkillType.Resources, new List<SkillType>(new SkillType[] { SkillType.Resources }));
            Evade.Add(SkillType.Shoot, new List<SkillType>(new SkillType[] { SkillType.Shoot }));
            Evade.Add(SkillType.Stealth, new List<SkillType>(new SkillType[] { SkillType.Stealth }));
            Evade.Add(SkillType.Will, new List<SkillType>(new SkillType[] { SkillType.Will }));

            Defend.Add(SkillType.Athletics, new List<SkillType>(new SkillType[] { SkillType.Fight, SkillType.Shoot }));
            Defend.Add(SkillType.Physique, new List<SkillType>(new SkillType[] { SkillType.Fight, SkillType.Shoot }));
            Defend.Add(SkillType.Fight, new List<SkillType>(new SkillType[] { SkillType.Fight }));
            Defend.Add(SkillType.Will, new List<SkillType>(new SkillType[] { SkillType.Provoke }));
        }
        
        public static void SetShifts(Range fail, Range tie, Range succeed, Range succeedWithStyle)
        {
            FAIL = fail;
            TIE = tie;
            SUCCEED = succeed;
            SUCCEED_WITH_STYLE = succeedWithStyle;
        }

        private static readonly SkillChecker _skillChecker = new SkillChecker();
        public static SkillChecker Instance => _skillChecker;

        private Character _initiative;
        private Character _passive;
        private CharacterAction _action;
        private Skill _initiativeSkill;
        private Skill _passiveSkill;
        private int _initiativePoint;
        private int _passivePoint;
        private Action<CheckResult> _initiativeCallback;
        private Action<CheckResult> _passiveCallback;

        private CheckerState _state;

        private SkillChecker()
        {
            _apiObj = new API(this);
        }

        public void StartCheck(
            Character initiative, Character passive, CharacterAction action,
            Action<CheckResult> initiativeCallback, Action<CheckResult> passiveCallback
            )
        {
            _initiativeSkill = null;
            _passiveSkill = null;
            _initiativePoint = 0;
            _passivePoint = 0;
            if (_state != CheckerState.IDLE) throw new InvalidOperationException("Already in checking state.");
            _initiative = initiative ?? throw new ArgumentNullException(nameof(initiative));
            _passive = passive ?? throw new ArgumentNullException(nameof(passive));
            _action = action;
            _initiativeCallback = initiativeCallback ?? throw new ArgumentNullException(nameof(initiativeCallback));
            _passiveCallback = passiveCallback ?? throw new ArgumentNullException(nameof(passiveCallback));
            _state = CheckerState.INITIATIVE_SKILL;
        }

        public void CancelCheck()
        {
            if (_state == CheckerState.INITIATIVE_SKILL)
            {
                _initiativeCallback(CheckResult.CANCEL);
                _passiveCallback(CheckResult.CANCEL);
                _state = CheckerState.IDLE;
            }
            throw new InvalidOperationException("Cannot cancel at this time.");
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
            if (_state != CheckerState.INITIATIVE_ASPECT || _state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("Cannot make checking over.");
            int delta = _initiativePoint - _passivePoint;
            if (FAIL.InRange(delta))
            {
                _initiativeCallback(CheckResult.FAIL);
                if (SUCCEED_WITH_STYLE.InRange(-delta))
                {
                    _passiveCallback(CheckResult.SUCCEED_WITH_STYLE);
                }
                else
                {
                    _passiveCallback(CheckResult.SUCCEED);
                }
            }
            else if (TIE.InRange(delta))
            {
                _initiativeCallback(CheckResult.TIE);
                _passiveCallback(CheckResult.TIE);
            }
            else if (SUCCEED.InRange(delta))
            {
                _initiativeCallback(CheckResult.SUCCEED);
                _passiveCallback(CheckResult.FAIL);
            }
            else if (SUCCEED_WITH_STYLE.InRange(delta))
            {
                _initiativeCallback(CheckResult.SUCCEED_WITH_STYLE);
                _passiveCallback(CheckResult.FAIL);
            }
            else
            {
                _initiativeCallback(CheckResult.TIE);
                _passiveCallback(CheckResult.TIE);
            }
            _state = CheckerState.IDLE;
        }

        public bool CanResistSkill(SkillType initiative, SkillType resist)
        {
            Dictionary<SkillType, List<SkillType>> resistTable;
            switch (_action)
            {
                case CharacterAction.CREATE_ASPECT:
                    resistTable = Evade;
                    break;
                case CharacterAction.ATTACK:
                    resistTable = Defend;
                    break;
                case CharacterAction.HINDER:
                    resistTable = Overcome;
                    break;
                default:
                    return false;
            }
            if (resistTable.TryGetValue(initiative, out List<SkillType> resistable)) return resistable.Contains(resist);
            else return false;
        }

        public bool CanUseSkill(SkillType skillType)
        {
            switch (_action)
            {
                case CharacterAction.CREATE_ASPECT:
                    return true;
                case CharacterAction.ATTACK:
                    return skillType.CanAttack;
                case CharacterAction.HINDER:
                    return true;
                default:
                    return false;
            }
        }
        
        public void InitiativeUseSkill(SkillType skillType)
        {
            
        }

        public void InitiativeUseStunt(Stunt stunt)
        {

        }
        
        public void InitiativeUseAspect(Aspect aspect)
        {

        }

        public void PassiveUseSkill(SkillType skillType)
        {

        }

        public void PassiveUseStunt(Stunt stunt)
        {

        }

        public void PassiveUseAspect(Aspect aspect)
        {

        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
