using GameLogic.Campaign;
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
            PASSIVE_ASPECT
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

        private static readonly SkillChecker _skillChecker = new SkillChecker();
        public static SkillChecker Instance => _skillChecker;

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

        private CheckerState _state;
        
        public Character Initiative => _initiative;
        public Character Passive => _passive;
        public CharacterAction Action => _action;

        public SkillType InitiativeSkillType => _initiativeSkillType;
        public SkillType PassiveSkillType => _passiveSkillType;

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
            foreach (Player player in MainLogic.Players)
            {
                if (initiative.Controller == player) player.Client.SkillCheckDialog.Show(initiative, passive, Client.SkillCheckDialog.ClientState.INITIATIVE);
                else if (passive.Controller == player) player.Client.SkillCheckDialog.Show(initiative, passive, Client.SkillCheckDialog.ClientState.PASSIVE);
                else player.Client.SkillCheckDialog.Show(initiative, passive, Client.SkillCheckDialog.ClientState.OBSERVER);
            }
            if (initiative.Controller == null) MainLogic.DM.Client.SkillCheckDialog.Show(initiative, passive, Client.SkillCheckDialog.ClientState.INITIATIVE);
            else if (passive.Controller == null) MainLogic.DM.Client.SkillCheckDialog.Show(initiative, passive, Client.SkillCheckDialog.ClientState.PASSIVE);
            else MainLogic.DM.Client.SkillCheckDialog.Show(initiative, passive, Client.SkillCheckDialog.ClientState.OBSERVER);
        }

        public void CancelCheck()
        {
            if (_state != CheckerState.INITIATIVE_SKILL) throw new InvalidOperationException("Cannot cancel at this time.");
            foreach (User user in MainLogic.Players)
            {
                user.AsPlayer.Client.SkillCheckDialog.HideWithResult(CheckResult.CANCEL);
            }
            MainLogic.DM.AsDM.Client.SkillCheckDialog.HideWithResult(CheckResult.CANCEL);
            _initiativeCallback(CheckResult.CANCEL);
            _passiveCallback(CheckResult.CANCEL);
            _state = CheckerState.IDLE;
        }

        public void ForceEndCheck(CheckResult initiativeResult, CheckResult passiveResult)
        {
            if (_state == CheckerState.IDLE) throw new InvalidOperationException("Skill checking is not working.");
            foreach (Player player in MainLogic.Players)
            {
                if (_initiative.Controller == player) player.Client.SkillCheckDialog.HideWithResult(initiativeResult);
                else if (_passive.Controller == player) player.Client.SkillCheckDialog.HideWithResult(passiveResult);
                else player.Client.SkillCheckDialog.HideWithResult(CheckResult.CANCEL);
            }
            if (_initiative.Controller == null) MainLogic.DM.Client.SkillCheckDialog.HideWithResult(initiativeResult);
            else if (_passive.Controller == null) MainLogic.DM.Client.SkillCheckDialog.HideWithResult(passiveResult);
            else MainLogic.DM.Client.SkillCheckDialog.HideWithResult(CheckResult.CANCEL);
            _initiativeCallback(initiativeResult);
            _passiveCallback(passiveResult);
            _state = CheckerState.IDLE;
        }
        
        public void EndCheck()
        {
            if (_state != CheckerState.INITIATIVE_ASPECT || _state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("Cannot make checking over.");
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
            foreach (Player player in MainLogic.Players)
            {
                if (_initiative.Controller == player) player.Client.SkillCheckDialog.HideWithResult(initiativeResult);
                else if (_passive.Controller == player) player.Client.SkillCheckDialog.HideWithResult(passiveResult);
                else player.Client.SkillCheckDialog.HideWithResult(CheckResult.CANCEL);
            }
            if (_initiative.Controller == null) MainLogic.DM.Client.SkillCheckDialog.HideWithResult(initiativeResult);
            else if (_passive.Controller == null) MainLogic.DM.Client.SkillCheckDialog.HideWithResult(passiveResult);
            else MainLogic.DM.Client.SkillCheckDialog.HideWithResult(CheckResult.CANCEL);
            _initiativeCallback(initiativeResult);
            _passiveCallback(passiveResult);
            _state = CheckerState.IDLE;
        }

        public void InitiativeUseSkill(SkillType skillType, bool stuntDo, int[] fixedDicePoints = null)
        {
            if (_state != CheckerState.INITIATIVE_SKILL) throw new InvalidOperationException("Is not in the correct state.");
            if (stuntDo)
            {
                this.InitiativeSkillTakeEffect(skillType, true, fixedDicePoints);
                _state = CheckerState.PASSIVE_SKILL;
            }
            else if (_action == CharacterAction.CREATE_ASPECT || _action == CharacterAction.HINDER)
            {
                MainLogic.DM.Client.DMCheckDialog.RequestCheck(_initiative.Name + "对" + _passive.Name + "使用" + skillType.Name + ",可以吗？",
                    result =>
                    {
                        if (result) this.InitiativeSkillTakeEffect(skillType, false, fixedDicePoints);
                        _state = CheckerState.PASSIVE_SKILL;
                    });
            }
            else if (_action == CharacterAction.ATTACK)
            {
                SkillProperty skillProperty = _initiative.GetSkillProperty(skillType);
                if (!skillProperty.canAttack) throw new ArgumentException("This skill cannot use in attack situation.", nameof(skillType));
                this.InitiativeSkillTakeEffect(skillType, false, fixedDicePoints);
                _state = CheckerState.PASSIVE_SKILL;
            }
        }

        public void InitiativeSkillTakeEffect(SkillType skillType, bool bigone, int[] fixedDicePoints = null)
        {
            int[] dicePoints = fixedDicePoints ?? this.RollDice();
            _initiativeRollPoint = 0;
            foreach (int point in dicePoints) _initiativeRollPoint += point;
            _initiativeSkillType = skillType;
            foreach (Player player in MainLogic.Players)
            {
                player.Client.SkillCheckDialog.DisplayDicePoint(true, dicePoints);
                player.Client.SkillCheckDialog.DisplaySkillReady(true, skillType, bigone);
                player.Client.SkillCheckDialog.UpdateSumPoint(true, this.InitiativePoint);
            }
            MainLogic.DM.Client.SkillCheckDialog.DisplayDicePoint(true, dicePoints);
            MainLogic.DM.Client.SkillCheckDialog.DisplaySkillReady(true, skillType, bigone);
            MainLogic.DM.Client.SkillCheckDialog.UpdateSumPoint(true, this.InitiativePoint);
        }
        
        public void InitiativeUseStunt(Stunt stunt)
        {
            if (_state != CheckerState.INITIATIVE_SKILL) throw new InvalidOperationException("Is not in the correct state.");
            if (stunt.Belong != _initiative) throw new ArgumentException("This stunt is not belong to initiative character.", nameof(stunt));
            if (stunt.NeedDMCheck)
            {
                MainLogic.DM.Client.DMCheckDialog.RequestCheck(_initiative.Name + "对" + _passive.Name + "使用" + stunt.Name + ",可以吗？",
                    result =>
                    {
                        if (result)
                        {
                            _initiativeSkillType = stunt.BoundSkillType;
                            stunt.InitiativeEffect.DoAction();
                        }
                    });
            }
            else
            {
                _initiativeSkillType = stunt.BoundSkillType;
                stunt.InitiativeEffect.DoAction();
            }
        }

        public void InitiativeSkipUsingAspects()
        {
            if (_state != CheckerState.INITIATIVE_ASPECT) throw new InvalidOperationException("Is not in the correct state.");
            _state = CheckerState.PASSIVE_ASPECT;
        }

        public void InitiativeUseAspect(Aspect aspect, bool reroll)
        {
            if (_state != CheckerState.INITIATIVE_ASPECT) throw new InvalidOperationException("Is not in the correct state.");
            if (aspect.Benefit != _initiative && _initiative.FatePoint - 1 < 0) throw new InvalidOperationException("Fate points are not enough.");
            foreach (Player player in MainLogic.Players)
            {
                player.Client.SkillCheckDialog.DisplayUsingAspect(true, aspect);
            }
            MainLogic.DM.Client.SkillCheckDialog.DisplayUsingAspect(true, aspect);
            MainLogic.DM.Client.DMCheckDialog.RequestCheck(_initiative.Name + "想使用" + aspect.Belong.Name + "的" + aspect.Name + "可以吗？",
                result =>
                {
                    if (result)
                    {
                        if (aspect.Benefit != null && aspect.Benefit != _initiative)
                        {
                            ++aspect.Benefit.FatePoint;
                        }
                        else if (aspect.Benefit == null)
                        {
                            ++_passive.FatePoint;
                        }
                        --_initiative.FatePoint;
                        if (reroll) this.InitiativeSkillTakeEffect(_initiativeSkillType, false);
                        else
                        {
                            _initiativeExtraPoint += 2;
                            foreach (Player player in MainLogic.Players)
                            {
                                player.Client.SkillCheckDialog.DisplaySkillReady(true, _initiativeSkillType, true);
                                player.Client.SkillCheckDialog.UpdateSumPoint(true, this.InitiativePoint);
                            }
                            MainLogic.DM.Client.SkillCheckDialog.DisplaySkillReady(true, _initiativeSkillType, true);
                            MainLogic.DM.Client.SkillCheckDialog.UpdateSumPoint(true, this.InitiativePoint);
                        }
                    }
                });
        }
        
        public void PassiveUseSkill(SkillType skillType, bool stuntDo, int[] fixedDicePoints = null)
        {
            if (_state != CheckerState.PASSIVE_SKILL) throw new InvalidOperationException("Is not in the correct state.");
            if (stuntDo)
            {
                this.PassiveSkillTakeEffect(skillType, true, fixedDicePoints);
                _state = CheckerState.INITIATIVE_ASPECT;
            }
            else
            {
                SkillProperty skillProperty = _passive.GetSkillProperty(skillType);
                if (_action == CharacterAction.ATTACK && !skillProperty.canDefend) throw new ArgumentException("This skill cannot use in attack situation.", nameof(skillType));
                if (CanResistSkillWithoutDMCheck(_initiativeSkillType, skillType, _action))
                {
                    this.PassiveSkillTakeEffect(skillType, false, fixedDicePoints);
                    _state = CheckerState.INITIATIVE_ASPECT;
                }
                else
                {
                    MainLogic.DM.Client.DMCheckDialog.RequestCheck(_passive.Name + "对" + _passive.Name + "使用" + skillType.Name + ",可以吗？",
                        result =>
                        {
                            if (result)
                            {
                                this.PassiveSkillTakeEffect(skillType, false, fixedDicePoints);
                                _state = CheckerState.INITIATIVE_ASPECT;
                            }
                        });
                }
            }
        }

        public void PassiveSkillTakeEffect(SkillType skillType, bool bigone, int[] fixedDicePoints = null)
        {
            int[] dicePoints = fixedDicePoints ?? this.RollDice();
            _passiveRollPoint = 0;
            foreach (int point in dicePoints) _passiveRollPoint += point;
            _passiveSkillType = skillType;
            foreach (Player player in MainLogic.Players)
            {
                player.Client.SkillCheckDialog.DisplayDicePoint(false, dicePoints);
                player.Client.SkillCheckDialog.DisplaySkillReady(false, skillType, bigone);
                player.Client.SkillCheckDialog.UpdateSumPoint(false, this.PassivePoint);
            }
            MainLogic.DM.Client.SkillCheckDialog.DisplayDicePoint(false, dicePoints);
            MainLogic.DM.Client.SkillCheckDialog.DisplaySkillReady(false, skillType, bigone);
            MainLogic.DM.Client.SkillCheckDialog.UpdateSumPoint(false, this.PassivePoint);
        }

        public void PassiveUseStunt(Stunt stunt)
        {
            if (_state != CheckerState.PASSIVE_SKILL) throw new InvalidOperationException("Is not in the correct state.");
            if (stunt.Belong != _passive) throw new ArgumentException("This stunt is not belong to passive character.", nameof(stunt));
            if (stunt.NeedDMCheck)
            {
                MainLogic.DM.Client.DMCheckDialog.RequestCheck(_passive.Name + "对" + _initiative.Name + "使用" + stunt.Name + ",可以吗？",
                    result =>
                    {
                        if (result)
                        {
                            _passiveSkillType = stunt.BoundSkillType;
                            stunt.InitiativeEffect.DoAction();
                        }
                    });
            }
            else
            {
                _passiveSkillType = stunt.BoundSkillType;
                stunt.InitiativeEffect.DoAction();
            }
        }

        public void PassiveSkipUsingAspects()
        {
            if (_state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("Is not in the correct state.");
            this.EndCheck();
        }

        public void PassiveUseAspect(Aspect aspect, bool reroll)
        {
            if (_state != CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("Is not in the correct state.");
            if (aspect.Benefit != _passive && _passive.FatePoint - 1 < 0) throw new InvalidOperationException("Fate points are not enough.");
            foreach (Player player in MainLogic.Players)
            {
                player.Client.SkillCheckDialog.DisplayUsingAspect(false, aspect);
            }
            MainLogic.DM.Client.SkillCheckDialog.DisplayUsingAspect(false, aspect);
            MainLogic.DM.Client.DMCheckDialog.RequestCheck(_passive.Name + "想使用" + aspect.Belong.Name + "的" + aspect.Name + "可以吗？",
                result =>
                {
                    if (result)
                    {
                        if (aspect.Benefit != null && aspect.Benefit != _passive)
                        {
                            ++aspect.Benefit.FatePoint;
                        }
                        else if (aspect.Benefit == null)
                        {
                            ++_initiative.FatePoint;
                        }
                        --_passive.FatePoint;
                        if (reroll) this.PassiveSkillTakeEffect(_passiveSkillType, false);
                        else
                        {
                            _passiveExtraPoint += 2;
                            foreach (Player player in MainLogic.Players)
                            {
                                player.Client.SkillCheckDialog.DisplaySkillReady(false, _passiveSkillType, true);
                                player.Client.SkillCheckDialog.UpdateSumPoint(false, this.PassivePoint);
                            }
                            MainLogic.DM.Client.SkillCheckDialog.DisplaySkillReady(false, _passiveSkillType, true);
                            MainLogic.DM.Client.SkillCheckDialog.UpdateSumPoint(false, this.PassivePoint);
                        }
                    }
                });
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
