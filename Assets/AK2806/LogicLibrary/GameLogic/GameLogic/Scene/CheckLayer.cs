using GameLogic.CharacterSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Scene
{
    public sealed class CheckLayer
    {
        public enum CharacterAction
        {
            CREATE_ASPECT,
            ATTACK,
            HINDER
        }

        public static readonly Dictionary<SkillType, List<SkillType>> Overcome = new Dictionary<SkillType, List<SkillType>>();
        public static readonly Dictionary<SkillType, List<SkillType>> Evade = new Dictionary<SkillType, List<SkillType>>();
        public static readonly Dictionary<SkillType, List<SkillType>> Defend = new Dictionary<SkillType, List<SkillType>>();
        
        static CheckLayer()
        {
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

        private Character _initiative;
        private Character _passive;
        private CharacterAction _action;
        private Skill _initiativeSkill = null;
        private Skill _passiveSkill = null;
        private int _initiativePoint = 0;
        private int _passivePoint = 0;

        public CheckLayer(Character initiative, Character passive, CharacterAction action)
        {
            _initiative = initiative ?? throw new ArgumentNullException(nameof(initiative));
            _passive = passive ?? throw new ArgumentNullException(nameof(passive));
            _action = action;
        }
        
        private bool CanResistSkill(SkillType initiative, SkillType resist)
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

        private bool CanUseSkill(SkillType skillType)
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
        
        public bool InitiativeCanUseSkill(SkillType skillType)
        {
            return this.CanUseSkill(skillType);
        }

        public bool PassiveCanUseSkill(SkillType skillType)
        {
            if (_initiativeSkill != null) return this.CanResistSkill(_initiativeSkill.SkillType, skillType);
            else return false;
        }

        public void InitiativeUseSkill(SkillType skillType)
        {

        }
        
        public void InitiativeUseAspect(Aspect aspect)
        {

        }

        public void PassiveUseSkill(SkillType skillType)
        {

        }

        public void PassiveUseAspect(Aspect aspect)
        {

        }

        public void CheckInitiativeSkill(SkillType skillType, Action<bool> callback)
        {
            
        }

        public void CheckPassiveSkill(SkillType skillType, Action<bool> callback)
        {

        }

        public void CheckInitiativeAspect(IEnumerable<Aspect> aspects, Action<bool> callback)
        {

        }

        public void CheckPassiveAspect(IEnumerable<Aspect> aspects, Action<bool> callback)
        {

        }

    }
}
