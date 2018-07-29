using GameLogic.Campaign;
using GameLogic.CharacterSystem;
using GameLogic.Container;
using GameLogic.Container.StoryComponent;
using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public sealed class SkillCheckDialog : ClientComponent
    {
        public enum ClientState
        {
            INITIATIVE,
            PASSIVE,
            OBSERVER
        }

        private ClientState _state;

        public SkillCheckDialog(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(SkillSelectedMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(AspectSelectedMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(StuntSelectedMessage.MESSAGE_TYPE, this);
        }

        public override void MessageReceived(ulong timestamp, Message message)
        {
            if (_state == ClientState.OBSERVER || MainLogic.DM.Client.DMCheckDialog.IsChecking) return;
            if (message.MessageType == SkillSelectedMessage.MESSAGE_TYPE)
            {
                SkillSelectedMessage skillSelectedMessage = (SkillSelectedMessage)message;
                if (SkillType.SkillTypes.TryGetValue(skillSelectedMessage.skillTypeID, out SkillType skillType))
                {
                    this.OnSelectSkill(skillType);
                }
            }
            else if (message.MessageType == StuntSelectedMessage.MESSAGE_TYPE)
            {
                StuntSelectedMessage stuntSelectedMessage = (StuntSelectedMessage)message;
                this.OnSelectStunt(stuntSelectedMessage.stuntID);
            }
            else if (message.MessageType == AspectSelectedMessage.MESSAGE_TYPE)
            {
                Aspect result = null;
                AspectSelectedMessage aspectSelectedMessage = (AspectSelectedMessage)message;
                Character character = CharacterManager.Instance.FindCharacterOrItemRecursivelyByID(aspectSelectedMessage.characterID);
                if (character != null)
                {
                    foreach (Aspect aspect in character.Aspects)
                    {
                        if (aspect.ID == aspectSelectedMessage.aspectID)
                        {
                            result = aspect;
                            break;
                        }
                    }
                }
                this.OnSelectAspects(result, aspectSelectedMessage.reroll);
            }
        }

        private void OnSelectSkill(SkillType skillType)
        {
            if (_state == ClientState.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_SKILL)
            {
                SkillChecker.Instance.InitiativeUseSkill(skillType, false);
            }
            else if (_state == ClientState.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL)
            {
                SkillChecker.Instance.PassiveUseSkill(skillType, false);
            }
        }

        private void OnSelectStunt(string stuntID)
        {
            if (_state == ClientState.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_SKILL)
            {
                foreach (Stunt stunt in SkillChecker.Instance.Initiative.Stunts)
                {
                    if (stunt.ID == stuntID)
                    {
                        SkillChecker.Instance.InitiativeUseStunt(stunt);
                        break;
                    }
                }
            }
            else if (_state == ClientState.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL)
            {
                foreach (Stunt stunt in SkillChecker.Instance.Passive.Stunts)
                {
                    if (stunt.ID == stuntID)
                    {
                        SkillChecker.Instance.PassiveUseStunt(stunt);
                        break;
                    }
                }
            }
        }

        private void OnSelectAspects(Aspect aspect, bool reroll)
        {
            if (_state == ClientState.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_ASPECT)
            {
                SkillChecker.Instance.InitiativeUseAspect(aspect, reroll);
            }
            else if (_state == ClientState.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_ASPECT)
            {
                SkillChecker.Instance.PassiveUseAspect(aspect, reroll);
            }
        }

        public void Show(Character initiative, Character passive, ClientState clientState)
        {
            SkillCheckDialogShowMessage message = new SkillCheckDialogShowMessage();
            message.initiativeCharacterID = initiative.ID;
            message.initiativeView = initiative.View;
            message.passiveCharacterID = passive.ID;
            message.passiveView = passive.View;
            message.playerState = (int)(_state = clientState);
            _connection.SendMessage(message);
        }

        public void HideWithResult(SkillChecker.CheckResult checkResult)
        {
            SkillCheckDialogHideMessage message = new SkillCheckDialogHideMessage();
            message.checkResult = (int)checkResult;
            _connection.SendMessage(message);
        }

        public void DisplayDicePoint(bool isInitiative, int[] dicePoints)
        {

        }
        
        public void UpdateSumPoint(bool isInitiative, int point)
        {
            
        }

        public void DisplaySkillReady(bool isInitiative, SkillType skillType, bool bigone)
        {

        }

        public void DisplayUsingAspect(bool isInitiative, Aspect aspect)
        {

        }
        
    }
}
