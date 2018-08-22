using GameLogic.Core;
using GameLogic.Container.StoryComponent;
using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using GameLogic.CharacterSystem;
using GameLogic.Container;
using GameLogic.Core.DataSystem;

namespace GameLogic.ClientComponents
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
        private bool _isUsing = false;

        public SkillCheckPanel(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(CheckerSkillSelectedMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(CheckerAspectSelectedMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(CheckerStuntSelectedMessage.MESSAGE_TYPE, this);
        }

        public override void MessageReceived(Message message)
        {
            try
            {
                if (!_isUsing || _position == ClientPosition.OBSERVER || MainLogic.DM.DMClient.DMCheckDialog.IsChecking) return;
                if (message.MessageType == CheckerSkillSelectedMessage.MESSAGE_TYPE)
                {
                    CheckerSkillSelectedMessage skillSelectedMessage = (CheckerSkillSelectedMessage)message;
                    if (SkillType.SkillTypes.TryGetValue(skillSelectedMessage.skillTypeID, out SkillType skillType))
                    {
                        this.OnSelectSkill(skillType);
                    }
                }
                else if (message.MessageType == CheckerStuntSelectedMessage.MESSAGE_TYPE)
                {
                    CheckerStuntSelectedMessage stuntSelectedMessage = (CheckerStuntSelectedMessage)message;
                    this.OnSelectStunt(stuntSelectedMessage.stuntID);
                }
                else if (message.MessageType == CheckerAspectSelectedMessage.MESSAGE_TYPE)
                {
                    Aspect result = null;
                    CheckerAspectSelectedMessage aspectSelectedMessage = (CheckerAspectSelectedMessage)message;
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
            catch (Exception e)
            {
                Logger.WriteLine(e.Message);
            }
        }

        private void OnSelectSkill(SkillType skillType)
        {
            if (_position == ClientPosition.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_SKILL_OR_STUNT)
            {
                StorySceneContainer.Instance.InitiativeUseSkill(skillType, false, false);
            }
            else if (_position == ClientPosition.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL_OR_STUNT)
            {
                StorySceneContainer.Instance.PassiveUseSkill(skillType, false, false);
            }
        }

        private void OnSelectStunt(string stuntID)
        {
            if (_position == ClientPosition.INITIATIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.INITIATIVE_SKILL_OR_STUNT)
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
            else if (_position == ClientPosition.PASSIVE && SkillChecker.Instance.State == SkillChecker.CheckerState.PASSIVE_SKILL_OR_STUNT)
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

        public void Open(Character initiative, Character passive, ClientPosition clientState)
        {
            if (_isUsing) return;
            SkillCheckPanelShowMessage message = new SkillCheckPanelShowMessage();
            message.initiativeCharacterID = initiative.ID;
            message.initiativeView = initiative.View;
            message.passiveCharacterID = passive.ID;
            message.passiveView = passive.View;
            message.playerState = (int)(_position = clientState);
            _connection.SendMessage(message);
            _isUsing = true;
        }

        public void Close()
        {
            if (!_isUsing) return;
            SkillCheckPanelHideMessage message = new SkillCheckPanelHideMessage();
            _connection.SendMessage(message);
            _position = ClientPosition.OBSERVER;
            _isUsing = false;
        }
        
        public void NotifyInitiativeSelectSkillOrStunt(SkillChecker.CharacterAction action, Character initiative, Character passive)
        {
            if (!_isUsing) return;
            var message = new StorySceneCheckerNotifyInitiativeSelectSkillOrStuntMessage();
            message.initiativeCharacterID = initiative.ID;
            message.passiveCharacterID = passive.ID;
            message.action = (int)action;
            _connection.SendMessage(message);
        }
        
        public void NotifyPassiveSelectSkillOrStunt(SkillChecker.CharacterAction action, Character passive, Character initiative, SkillType initiativeSkillType)
        {
            if (!_isUsing) return;
            var message = new StorySceneCheckerNotifyPassiveSelectSkillOrStuntMessage();
            message.passiveCharacterID = passive.ID;
            message.initiativeCharacterID = initiative.ID;
            message.initiativeSkillType = new SkillTypeDescription(initiativeSkillType);
            message.action = (int)action;
            _connection.SendMessage(message);
        }

        public void NotifyInitiativeCheckSelectAspect(Character initiative)
        {
            if (!_isUsing) return;
            var message = new StorySceneCheckerNotifySelectAspectMessage();
            message.isInitiative = true;
            message.characterID = initiative.ID;
            _connection.SendMessage(message);
        }

        public void NotifyPassiveCheckSelectAspect(Character passive)
        {
            if (!_isUsing) return;
            var message = new StorySceneCheckerNotifySelectAspectMessage();
            message.isInitiative = false;
            message.characterID = passive.ID;
            _connection.SendMessage(message);
        }

        public void DisplayDicePoint(bool isInitiative, int[] dicePoints)
        {
            if (!_isUsing) return;
            DisplayDicePointsMessage message = new DisplayDicePointsMessage();
            message.userID = isInitiative ? SkillChecker.Instance.Initiative.Controller.Id : SkillChecker.Instance.Passive.Controller.Id;
            message.dicePoints = dicePoints;
            _connection.SendMessage(message);
        }

        public void UpdateSumPoint(bool isInitiative, int point)
        {
            if (!_isUsing) return;
            StorySceneCheckerUpdateSumPointMessage message = new StorySceneCheckerUpdateSumPointMessage();
            message.isInitiative = isInitiative;
            message.point = point;
            _connection.SendMessage(message);
        }

        public void DisplaySkillReady(bool isInitiative, SkillType skillType, bool bigone)
        {
            if (!_isUsing) return;
            StorySceneCheckerDisplaySkillReadyMessage message = new StorySceneCheckerDisplaySkillReadyMessage();
            message.isInitiative = isInitiative;
            message.skillTypeID = skillType.ID;
            message.bigone = bigone;
            _connection.SendMessage(message);
        }

        public void DisplayUsingAspect(bool isInitiative, Aspect aspect)
        {
            if (!_isUsing) return;
            StorySceneCheckerDisplayUsingAspectMessage message = new StorySceneCheckerDisplayUsingAspectMessage();
            message.isInitiative = isInitiative;
            message.characterID = aspect.Belong.ID;
            message.aspectID = aspect.ID;
            _connection.SendMessage(message);
        }

    }

    public sealed class TextBox : ClientComponent
    {
        public override void MessageReceived(Message message)
        {
            TextSelectedMessage selectedMessage = (TextSelectedMessage)message;
            this.OnSelectItem(selectedMessage.selection);
        }

        private void OnSelectItem(int selection)
        {

        }

        public TextBox(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(TextSelectedMessage.MESSAGE_TYPE, this);
        }

        public void AppendParagraph(string text)
        {
            TextBoxAddParagraphMessage message = new TextBoxAddParagraphMessage();
            message.text = text;
            _connection.SendMessage(message);
        }

        public void AppendSelectableParagraph(string text, int selection)
        {
            TextBoxAddSelectionMessage message = new TextBoxAddSelectionMessage();
            message.text = text;
            message.selectionCode = selection;
            _connection.SendMessage(message);
        }

        public void Clear()
        {
            TextBoxClearMessage message = new TextBoxClearMessage();
            _connection.SendMessage(message);
        }

        public void SetCharacterView(CharacterView view)
        {
            TextBoxSetPortraitMessage message = new TextBoxSetPortraitMessage();
            message.view = view;
            _connection.SendMessage(message);
        }

        public void SetCharacterViewEffect(CharacterViewEffect effect)
        {
            TextBoxPortraitEffectMessage message = new TextBoxPortraitEffectMessage();
            message.effect = effect;
            _connection.SendMessage(message);
        }

        public void SetPortraitStyle(PortraitStyle portrait)
        {
            TextBoxPortraitStyleMessage message = new TextBoxPortraitStyleMessage();
            message.style = portrait;
            _connection.SendMessage(message);
        }

    }

    public class StoryScene : ClientComponent
    {
        protected readonly TextBox _textBox;
        protected readonly SkillCheckPanel _skillCheckPanel;
        protected bool _isUsing = false;

        public TextBox TextBox => _textBox;
        public SkillCheckPanel SkillCheckPanel => _skillCheckPanel;

        public override void MessageReceived(Message message) { }

        protected StoryScene(Connection connection, User owner) :
            base(connection, owner)
        {
            _textBox = new TextBox(connection, owner);
            _skillCheckPanel = new SkillCheckPanel(connection, owner);
        }

        public void Open()
        {
            if (_isUsing) return;
            _isUsing = true;
        }

        public void Close()
        {
            if (!_isUsing) return;
            _skillCheckPanel.Close();
            _isUsing = false;
        }

        public void Reset()
        {
            StorySceneResetMessage message = new StorySceneResetMessage();
            _connection.SendMessage(message);
        }

        public void AddPlayerCharacter(Character character)
        {
            StorySceneAddPlayerCharacterMessage message = new StorySceneAddPlayerCharacterMessage();

            _connection.SendMessage(message);
        }

        public void RemovePlayerCharacter(Character character)
        {
            StorySceneRemovePlayerCharacterMessage message = new StorySceneRemovePlayerCharacterMessage();

            _connection.SendMessage(message);
        }
        
        public void AddObject(IStoryObject obj)
        {
            StorySceneObjectAddMessage message = new StorySceneObjectAddMessage();
            message.objID = obj.ID;
            message.view = obj.CharacterRef.View;
            _connection.SendMessage(message);
        }

        public void RemoveObject(string objID)
        {
            StorySceneObjectRemoveMessage message = new StorySceneObjectRemoveMessage();
            message.objID = objID;
            _connection.SendMessage(message);
        }

        public void RemoveObject(IStoryObject obj)
        {
            this.RemoveObject(obj.ID);
        }
        
        public void TransformObject(IStoryObject obj, Layout to)
        {
            StorySceneObjectTransformMessage message = new StorySceneObjectTransformMessage();
            message.objID = obj.ID;
            message.to = to;
            _connection.SendMessage(message);
        }
        
        public void SetObjectViewEffect(IStoryObject obj, CharacterViewEffect effect)
        {
            StorySceneObjectViewEffectMessage message = new StorySceneObjectViewEffectMessage();
            message.objID = obj.ID;
            message.effect = effect;
            _connection.SendMessage(message);
        }

        public void SetObjectPortraitStyle(IStoryObject obj, PortraitStyle portrait)
        {
            StorySceneObjectPortraitStyleMessage message = new StorySceneObjectPortraitStyleMessage();
            message.objID = obj.ID;
            message.portrait = portrait;
            _connection.SendMessage(message);
        }

        public void TransformCamera(Layout to)
        {
            StorySceneCameraTransformMessage message = new StorySceneCameraTransformMessage();
            message.to = to;
            _connection.SendMessage(message);
        }

        public void SetCameraEffect(CameraEffect effect)
        {
            StorySceneCameraEffectMessage message = new StorySceneCameraEffectMessage();
            message.effect = effect;
            _connection.SendMessage(message);
        }
    }

    public sealed class DMStoryScene : StoryScene
    {
        public DMStoryScene(Connection connection, DM owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(StorySceneNextActionMessage.MESSAGE_TYPE, this);
        }

        public override void MessageReceived(Message message)
        {
            if (!_isUsing) return;
            this.OnNextAction();
        }

        private void OnNextAction()
        {

        }

    }

    public sealed class PlayerStoryScene : StoryScene
    {
        public PlayerStoryScene(Connection connection, Player owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(StorySceneObjectActionMessage.MESSAGE_TYPE, this);
        }
        
        public override void MessageReceived(Message message)
        {
            if (!_isUsing) return;
            StorySceneObjectActionMessage objectMessage = (StorySceneObjectActionMessage)message;
            switch (objectMessage.action)
            {
                case StorySceneObjectActionMessage.PlayerAction.INTERACT:
                    this.OnInteract(objectMessage.objID);
                    break;
                case StorySceneObjectActionMessage.PlayerAction.CREATE_ASPECT:
                    this.OnCreateAspect(objectMessage.objID);
                    break;
                case StorySceneObjectActionMessage.PlayerAction.ATTACK:
                    this.OnAttack(objectMessage.objID);
                    break;
                case StorySceneObjectActionMessage.PlayerAction.HINDER:
                    this.OnHinder(objectMessage.objID);
                    break;
                default:
                    return;
            }
        }

        private void OnInteract(string objID)
        {

        }

        private void OnCreateAspect(string objID)
        {

        }

        private void OnAttack(string objID)
        {

        }

        private void OnHinder(string objID)
        {

        }


    }

}
