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
    public sealed class SkillCheckPanel : ClientComponent
    {
        public enum ClientPosition
        {
            INITIATIVE,
            PASSIVE,
            OBSERVER
        }

        private ClientPosition _position = ClientPosition.OBSERVER;
        private bool _isChecking = false;

        public SkillCheckPanel(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(SkillSelectedMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(AspectSelectedMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(StuntSelectedMessage.MESSAGE_TYPE, this);
        }

        public override void MessageReceived(ulong timestamp, Message message)
        {
            if (!_isChecking || _position == ClientPosition.OBSERVER || MainLogic.DM.Client.DMCheckDialog.IsChecking) return;
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
            if (_position == ClientPosition.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_SKILL)
            {
                StorySceneContainer.Instance.InitiativeUseSkill(skillType, false, false);
            }
            else if (_position == ClientPosition.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL)
            {
                StorySceneContainer.Instance.PassiveUseSkill(skillType, false, false);
            }
        }

        private void OnSelectStunt(string stuntID)
        {
            if (_position == ClientPosition.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_SKILL)
            {
                foreach (Stunt stunt in SkillChecker.Instance.Initiative.Stunts)
                {
                    if (stunt.ID == stuntID)
                    {
                        StorySceneContainer.Instance.InitiativeUseStunt(stunt);
                        break;
                    }
                }
            }
            else if (_position == ClientPosition.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL)
            {
                foreach (Stunt stunt in SkillChecker.Instance.Passive.Stunts)
                {
                    if (stunt.ID == stuntID)
                    {
                        StorySceneContainer.Instance.PassiveUseStunt(stunt);
                        break;
                    }
                }
            }
        }

        private void OnSelectAspects(Aspect aspect, bool reroll)
        {
            if (_position == ClientPosition.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_ASPECT)
            {
                StorySceneContainer.Instance.InitiativeUseAspect(aspect, reroll);
            }
            else if (_position == ClientPosition.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_ASPECT)
            {
                StorySceneContainer.Instance.PassiveUseAspect(aspect, reroll);
            }
        }

        public void Show(Character initiative, Character passive, ClientPosition clientState)
        {
            if (_isChecking) return;
            SkillCheckPanelShowMessage message = new SkillCheckPanelShowMessage();
            message.initiativeCharacterID = initiative.ID;
            message.initiativeView = initiative.View;
            message.passiveCharacterID = passive.ID;
            message.passiveView = passive.View;
            message.playerState = (int)(_position = clientState);
            _connection.SendMessage(message);
            _isChecking = true;
        }

        public void Hide()
        {
            if (!_isChecking) return;
            SkillCheckPanelHideMessage message = new SkillCheckPanelHideMessage();
            _connection.SendMessage(message);
            _position = ClientPosition.OBSERVER;
            _isChecking = false;
        }

        public void DisplayDicePoint(bool isInitiative, int[] dicePoints)
        {
            if (!_isChecking) return;
            DisplayDicePointsMessage message = new DisplayDicePointsMessage();
            message.userID = isInitiative ?
                (SkillChecker.Instance.Initiative.Controller != null ? SkillChecker.Instance.Initiative.Controller.Id : MainLogic.DM.Id)
                : (SkillChecker.Instance.Passive.Controller != null ? SkillChecker.Instance.Passive.Controller.Id : MainLogic.DM.Id);
            message.dicePoints = dicePoints;
            _connection.SendMessage(message);
        }
        
        public void UpdateSumPoint(bool isInitiative, int point)
        {
            if (!_isChecking) return;
            SkillCheckPanelUpdateSumPointMessage message = new SkillCheckPanelUpdateSumPointMessage();
            message.isInitiative = isInitiative;
            message.point = point;
            _connection.SendMessage(message);
        }

        public void DisplaySkillReady(bool isInitiative, SkillType skillType, bool bigone)
        {
            if (!_isChecking) return;
            SkillCheckPanelDisplaySkillReadyMessage message = new SkillCheckPanelDisplaySkillReadyMessage();
            message.isInitiative = isInitiative;
            message.skillTypeID = skillType.ID;
            message.bigone = bigone;
            _connection.SendMessage(message);
        }

        public void DisplayUsingAspect(bool isInitiative, Aspect aspect)
        {
            if (!_isChecking) return;
            SkillCheckPanelDisplayUsingAspectMessage message = new SkillCheckPanelDisplayUsingAspectMessage();
            message.isInitiative = isInitiative;
            message.characterID = aspect.Belong.ID;
            message.aspectID = aspect.ID;
            _connection.SendMessage(message);
        }
        
    }
}
